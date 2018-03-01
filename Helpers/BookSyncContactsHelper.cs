using System;
using System.Collections.Generic;
using BookSync.Models;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BookSync.Helpers
{
    public static class BookSyncContactsHelper
	{
        public static event EventHandler ContactsLoadedAndMatchced;
        public static event EventHandler ContactsSaved;
        public static event EventHandler<Exception> ContactLoadExceptionOccured;

        private static List<BookSyncContact> _booksyncContacts;
		public static List<BookSyncContact> BookSyncContacts
        {
            get
            {
                if (_booksyncContacts == null)
                    _booksyncContacts = new List<BookSyncContact>();
                
                return _booksyncContacts;
            }
            set
            {
                _booksyncContacts = value;
            }
        }

        private static List<PhoneContact> _phoneContacts;
		public static List<PhoneContact> PhoneContacts
        {
            get
            {
                return _phoneContacts;
            }
            set
            {
                _phoneContacts = value;
            }
        }

        private static List<FacebookUser> _facebookUsers;
		public static List<FacebookUser> FacebookUsers
        {
            get
            {
                return _facebookUsers;
            }
            set
            {
                _facebookUsers = value;
            }
        }

        private static bool _contactsLoaded = false;    

        public static bool HasCancelled { get; set; }

        public static event EventHandler ContactsLoaded;

        public static async Task LoadContacts()
        {
            try
            {


                if (_contactsLoaded)
                    return;

                _contactsLoaded = true;

                //get the contacts from the file system, deseralise the json
                BookSyncContacts = JsonConvert.DeserializeObject<List<BookSyncContact>>(Settings.BookSyncContactsSettings)
                                              ?? new List<BookSyncContact>();

                //now load the 2 other lists and match all 3 once we have them
                FacebookHelper.FacebooUsersLoaded += (sender, e) =>
                {
                    FacebookUsers = FacebookHelper.GetFacebookUsers();
                    ShouldMatchContacts();
                };

                PhoneContactHelper.PhoneContactsLoaded += (sender, e) =>
                {
                    PhoneContacts = PhoneContactHelper.GetPhoneContacts();
                    ShouldMatchContacts();
                };

                await FacebookHelper.LoadFacebookContacts();
                await PhoneContactHelper.LoadPhoneContacts();
            }
            catch(Exception ex)
            {
                ContactLoadExceptionOccured?.Invoke(null, ex);
            }
        }

        private static void ShouldMatchContacts()
        {
			if (FacebookUsers != null && PhoneContacts != null)
			{
                //TODO:// MOCK UP A LOT MORE USERS
				MatchContacts();
			}
        }

        private static void MatchContacts()
        {
            //DEBUG 
            //foreach (var item in BookSyncContacts)
            //{
            //    item.FacebookUserId = string.Empty;
            //}

           // BookSyncContacts.Clear();
           // SaveBookSyncContacts();


            //so having all the contacts we need to update the book sync contacts with the new ones and remove the deleted ones
            //then go through the contacts matched with fb users and update the pictures
            List<BookSyncContact> contactsToAdd = (from a in PhoneContacts
             where !BookSyncContacts.Contains(BookSyncContacts.FirstOrDefault(f => f.PhoneContactId == a.PhoneContactId))
                                                    select new BookSyncContact() {
													                PhoneContactId = a.PhoneContactId,
                                                                    PhoneFirstName = a.FirstName,
                                                                    PhoneLastName = a.LastName,
                                                                    PhoneImageBase64 = a.PhoneImageBase64
													            }).AsParallel().ToList();

			//Get all the booksync contacts that have been removed from the phone
            List<string> contactsToRemove = (from a in BookSyncContacts
											   where !PhoneContacts.Contains(PhoneContacts.FirstOrDefault(f => f.PhoneContactId == a.PhoneContactId))
											   select a.PhoneContactId).AsParallel().ToList();

            //remove the ones to remove
            BookSyncContacts.RemoveAll((obj) => contactsToRemove.Contains(obj.PhoneContactId));

            //add in the new ones
            BookSyncContacts.AddRange(contactsToAdd);

            //sort the list TODO:// check if this is actually needed as we should maybe insert at
            BookSyncContacts.Sort(new BookSyncContactComparer());

            //loop through each of the contacts we've got and update the fb image if it's matched
            Parallel.ForEach(BookSyncContacts.Where(w => !string.IsNullOrEmpty(w.FacebookUserId)), (BookSyncContact contact, ParallelLoopState state) =>
            {
                var matchedFbFriend = FacebookUsers.FirstOrDefault(f => f.ID == contact.FacebookUserId);
                if(matchedFbFriend != null)
                {
                    contact.FacebookImageSmallUrl = matchedFbFriend.ProfileImageSmallUrl;
                    contact.FacebookImageLargeUrl = matchedFbFriend.ProfileImageLargeUrl;
                }
            });

			//loop through all the contacts and update the phone details 
			Parallel.ForEach(BookSyncContacts, (BookSyncContact contact, ParallelLoopState state) =>
		   {
			   var phoneContact = PhoneContacts.FirstOrDefault(f => f.PhoneContactId == contact.PhoneContactId);
			   if (phoneContact != null)
			   {
				   contact.PhoneFirstName = phoneContact.FirstName;
				   contact.PhoneLastName = phoneContact.LastName;
				   contact.PhoneImageBase64 = phoneContact.PhoneImageBase64;
			   }
		   });

            //save the new collection of objects back to the settings to save it
            SaveBookSyncContacts();

            ContactsLoadedAndMatchced?.Invoke(null, new EventArgs());
		}

		public static async Task SaveBookSyncContacts()
		{
			Settings.BookSyncContactsSettings = JsonConvert.SerializeObject(BookSyncContacts);

            ContactsSaved?.Invoke(null, new EventArgs());
		}
    }

    /// <summary>
    /// Book sync contact comparer.
    /// </summary>
    public class BookSyncContactComparer : Comparer<BookSyncContact>
    {
        public override int Compare(BookSyncContact x, BookSyncContact y)
        {
            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }
    }
}
