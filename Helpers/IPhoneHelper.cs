using System;
using System.Collections.Generic;
using System.Threading;
using BookSync.Models;

namespace BookSync.Helpers
{
    /// <summary>
    /// Contains the interface to the methods to retrieve the phone contacts and set the contact images on the device
    /// </summary>
    public interface IPhoneHelper
    {
        event EventHandler PhoneContactsLoaded;
        event EventHandler<BookSyncContact> SyncSuccess;
        event EventHandler<BookSyncException> SyncError;

        void LoadPhoneContacts();
        List<PhoneContact> GetPhoneContacts();
        void SetPhonePicture(BookSyncContact contact, CancellationToken token);
    }
}
