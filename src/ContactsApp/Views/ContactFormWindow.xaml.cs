using ContactsApp.ViewModels;
using System.Windows;

namespace ContactsApp.Views
{
    public partial class ContactFormWindow : Window
    {
        public ContactFormWindow(ContactFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Subscribe to RequestClose event
            viewModel.RequestClose += () => Close();
        }

        public bool? ShowDialogWithResult()
        {
            ShowDialog();
            return (DataContext as ContactFormViewModel)?.DialogResult;
        }
    }
}
