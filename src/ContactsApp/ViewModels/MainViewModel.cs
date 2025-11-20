using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ContactService _contactService;
        private readonly ImportExportService _importExportService;

        private ObservableCollection<Contact> _contacts;
        private Contact? _selectedContact;
        private string _searchText;
        private bool _isLoading;
        private string _statusMessage;
        private int _currentPage;
        private int _totalPages;
        private int _pageSize;
        private string _sortBy;
        private bool _sortAscending;

        public MainViewModel(ContactService contactService, ImportExportService importExportService)
        {
            _contactService = contactService;
            _importExportService = importExportService;

            _contacts = new ObservableCollection<Contact>();
            _searchText = string.Empty;
            _statusMessage = "Pronto";
            _currentPage = 1;
            _pageSize = 20;
            _sortBy = "FirstName";
            _sortAscending = true;

            // Initialize commands
            LoadContactsCommand = new RelayCommand(async _ => await LoadContactsAsync());
            SearchCommand = new RelayCommand(async _ => await SearchContactsAsync());
            AddContactCommand = new RelayCommand(_ => AddContact());
            EditContactCommand = new RelayCommand(_ => EditContact(), _ => SelectedContact != null);
            DeleteContactCommand = new RelayCommand(async _ => await DeleteContactAsync(), _ => SelectedContact != null);
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync());
            ExportCommand = new RelayCommand(async _ => await ExportContactsAsync());
            ImportCommand = new RelayCommand(async _ => await ImportContactsAsync());
            NextPageCommand = new RelayCommand(async _ => await NextPageAsync(), _ => CurrentPage < TotalPages);
            PreviousPageCommand = new RelayCommand(async _ => await PreviousPageAsync(), _ => CurrentPage > 1);
            SortCommand = new RelayCommand<string>(async sortBy => await SortContactsAsync(sortBy));

            // Load initial data
            _ = LoadContactsAsync();
        }

        #region Properties

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set => SetProperty(ref _contacts, value);
        }

        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (SetProperty(ref _selectedContact, value))
                {
                    ((RelayCommand)EditContactCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteContactCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    ((RelayCommand)NextPageCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PreviousPageCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    ((RelayCommand)NextPageCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    _ = LoadContactsAsync();
                }
            }
        }

        public string SortBy
        {
            get => _sortBy;
            set => SetProperty(ref _sortBy, value);
        }

        public bool SortAscending
        {
            get => _sortAscending;
            set => SetProperty(ref _sortAscending, value);
        }

        #endregion

        #region Commands

        public ICommand LoadContactsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddContactCommand { get; }
        public ICommand EditContactCommand { get; }
        public ICommand DeleteContactCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand SortCommand { get; }

        #endregion

        #region Methods

        private async Task LoadContactsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Carregando contatos...";

                var result = await _contactService.GetPaginatedContactsAsync(
                    CurrentPage, PageSize, SortBy, SortAscending);

                Contacts.Clear();
                foreach (var contact in result.Contacts)
                {
                    Contacts.Add(contact);
                }

                TotalPages = result.TotalPages;
                StatusMessage = $"Carregados {result.Contacts.Count} de {result.TotalCount} contatos (Página {CurrentPage}/{TotalPages})";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar contatos: {ex.Message}";
                MessageBox.Show($"Falha ao carregar contatos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchContactsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Buscando...";

                var results = await _contactService.SearchContactsAsync(SearchText);

                Contacts.Clear();
                foreach (var contact in results)
                {
                    Contacts.Add(contact);
                }

                StatusMessage = $"Encontrados {results.Count} contatos";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na busca: {ex.Message}";
                MessageBox.Show($"Falha na busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddContact()
        {
            var viewModel = new ContactFormViewModel(_contactService);
            var window = new ContactFormWindow(viewModel);
            
            if (window.ShowDialogWithResult() == true)
            {
                _ = LoadContactsAsync();
            }
        }

        private void EditContact()
        {
            if (SelectedContact == null)
                return;

            var viewModel = new ContactFormViewModel(_contactService, SelectedContact);
            var window = new ContactFormWindow(viewModel);
            
            if (window.ShowDialogWithResult() == true)
            {
                _ = LoadContactsAsync();
            }
        }

        private async Task DeleteContactAsync()
        {
            if (SelectedContact == null)
                return;

            var result = MessageBox.Show(
                $"Tem certeza que deseja excluir {SelectedContact.FirstName} {SelectedContact.LastName}?",
                "Confirmar Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                StatusMessage = "Excluindo contato...";

                var success = await _contactService.DeleteContactAsync(SelectedContact.Id);

                if (success)
                {
                    StatusMessage = "Contato excluído com sucesso";
                    await LoadContactsAsync();
                }
                else
                {
                    StatusMessage = "Falha ao excluir contato";
                    MessageBox.Show("Falha ao excluir contato", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao excluir: {ex.Message}";
                MessageBox.Show($"Falha ao excluir contato: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RefreshAsync()
        {
            SearchText = string.Empty;
            CurrentPage = 1;
            await LoadContactsAsync();
        }

        private async Task ExportContactsAsync()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = ".csv",
                    FileName = $"contacts_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() != true)
                    return;

                IsLoading = true;
                StatusMessage = "Exportando contatos...";

                var result = await _importExportService.ExportToCsvAsync(dialog.FileName);

                if (result.Success)
                {
                    StatusMessage = result.Message;
                    MessageBox.Show(result.Message, "Exportação Bem-sucedida", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    StatusMessage = "Falha na exportação";
                    MessageBox.Show(result.Message, "Falha na Exportação", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na exportação: {ex.Message}";
                MessageBox.Show($"Falha na exportação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ImportContactsAsync()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = ".csv"
                };

                if (dialog.ShowDialog() != true)
                    return;

                IsLoading = true;
                StatusMessage = "Importando contatos...";

                var result = await _importExportService.ImportFromCsvAsync(dialog.FileName, skipDuplicates: true);

                if (result.Success)
                {
                    StatusMessage = result.Message;
                    
                    var message = result.Message;
                    if (result.Errors.Any())
                    {
                        message += $"\n\nErrors:\n{string.Join("\n", result.Errors.Take(10))}";
                        if (result.Errors.Count > 10)
                            message += $"\n... and {result.Errors.Count - 10} more errors";
                    }

                    MessageBox.Show(message, "Importação Concluída", MessageBoxButton.OK, 
                        result.Errors.Any() ? MessageBoxImage.Warning : MessageBoxImage.Information);

                    await LoadContactsAsync();
                }
                else
                {
                    StatusMessage = "Falha na importação";
                    MessageBox.Show(result.Message, "Falha na Importação", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na importação: {ex.Message}";
                MessageBox.Show($"Falha na importação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadContactsAsync();
            }
        }

        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadContactsAsync();
            }
        }

        private async Task SortContactsAsync(string? sortBy)
        {
            if (string.IsNullOrEmpty(sortBy))
                return;

            if (SortBy == sortBy)
            {
                // Toggle sort direction
                SortAscending = !SortAscending;
            }
            else
            {
                SortBy = sortBy;
                SortAscending = true;
            }

            CurrentPage = 1;
            await LoadContactsAsync();
        }

        #endregion
    }
}
