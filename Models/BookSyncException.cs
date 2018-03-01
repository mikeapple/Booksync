using System;
namespace BookSync.Models
{
    public class BookSyncException : Exception
    {
        private BookSyncContact affectedContact;
        public BookSyncContact AffectedContact
        {
            get
            {
                return affectedContact;
            }
            set
            {
                affectedContact = value;
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        private Exception innerException;
        public Exception InnerException
        {
            get
            {
                return innerException;
            }
            set
            {
                innerException = value;
            }
        }
        public BookSyncException()
        {
        }
    }
}
