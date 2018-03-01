using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BookSync.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public event EventHandler CloseDialog;

        public AboutViewModel(Page page) : base(page)
        {
        }

        Command closeCommand;
        public System.Windows.Input.ICommand CloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand =
                                        new Command(async () => await ExecuteCloseCommand()));
            }
        }

        public async Task ExecuteCloseCommand()
        {
            CloseDialog?.Invoke(null, new EventArgs());
        }

    }
}

