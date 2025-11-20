using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ContactsApp.Data;
using ContactsApp.Repositories;
using ContactsApp.Services;
using ContactsApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ContactsApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Initialize database and services
        var dbContext = new AppDbContext();
        dbContext.Database.EnsureCreated(); // Ensure database is created

        var contactRepository = new ContactRepository(dbContext);
        var contactService = new ContactService(contactRepository);
        var importExportService = new ImportExportService(contactService);

        // Set DataContext to MainViewModel
        DataContext = new MainViewModel(contactService, importExportService);
    }
}