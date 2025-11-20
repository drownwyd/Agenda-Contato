using ContactsApp.Models;
using ContactsApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ContactsApp.ViewModels
{
    public class ContactFormViewModel : ViewModelBase
    {
        private readonly ContactService _contactService;
        private readonly Contact _originalContact;
        private bool _isEditMode;

        private int _id;
        private string _firstName;
        private string _lastName;
        private string _company;
        private string _primaryPhone;
        private string _secondaryPhone;
        private string _email;
        private string _address;
        private string _notes;
        private string _photoPath;
        private ObservableCollection<string> _validationErrors;
        private bool _isSaving;

        public ContactFormViewModel(ContactService contactService, Contact? contact = null)
        {
            _contactService = contactService;
            _originalContact = contact ?? new Contact();
            _isEditMode = contact != null && contact.Id > 0;

            // Initialize properties from contact
            _id = _originalContact.Id;
            _firstName = _originalContact.FirstName ?? string.Empty;
            _lastName = _originalContact.LastName ?? string.Empty;
            _company = _originalContact.Company ?? string.Empty;
            _primaryPhone = _originalContact.PrimaryPhone ?? string.Empty;
            _secondaryPhone = _originalContact.SecondaryPhone ?? string.Empty;
            _email = _originalContact.Email ?? string.Empty;
            _address = _originalContact.Address ?? string.Empty;
            _notes = _originalContact.Notes ?? string.Empty;
            _photoPath = _originalContact.PhotoPath ?? string.Empty;
            _validationErrors = new ObservableCollection<string>();

            // Initialize commands
            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => !IsSaving);
            CancelCommand = new RelayCommand(_ => Cancel());
            BrowsePhotoCommand = new RelayCommand(_ => BrowsePhoto());
            ClearPhotoCommand = new RelayCommand(_ => ClearPhoto(), _ => !string.IsNullOrEmpty(PhotoPath));
        }

        #region Properties

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                    ClearValidationErrors();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                    ClearValidationErrors();
            }
        }

        public string Company
        {
            get => _company;
            set
            {
                if (SetProperty(ref _company, value))
                    ClearValidationErrors();
            }
        }

        public string PrimaryPhone
        {
            get => _primaryPhone;
            set
            {
                if (SetProperty(ref _primaryPhone, value))
                    ClearValidationErrors();
            }
        }

        public string SecondaryPhone
        {
            get => _secondaryPhone;
            set
            {
                if (SetProperty(ref _secondaryPhone, value))
                    ClearValidationErrors();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    ClearValidationErrors();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (SetProperty(ref _address, value))
                    ClearValidationErrors();
            }
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                if (SetProperty(ref _photoPath, value))
                    ((RelayCommand)ClearPhotoCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> ValidationErrors
        {
            get => _validationErrors;
            set => SetProperty(ref _validationErrors, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (SetProperty(ref _isSaving, value))
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public bool DialogResult { get; private set; }

        public string WindowTitle => IsEditMode ? "Editar Contato" : "Adicionar Novo Contato";

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowsePhotoCommand { get; }
        public ICommand ClearPhotoCommand { get; }

        #endregion

        #region Methods

        private async System.Threading.Tasks.Task SaveAsync()
        {
            try
            {
                IsSaving = true;
                ClearValidationErrors();

                var contact = new Contact
                {
                    Id = Id,
                    FirstName = FirstName?.Trim() ?? string.Empty,
                    LastName = LastName?.Trim(),
                    Company = Company?.Trim(),
                    PrimaryPhone = PrimaryPhone?.Trim(),
                    SecondaryPhone = SecondaryPhone?.Trim(),
                    Email = Email?.Trim(),
                    Address = Address?.Trim(),
                    Notes = Notes?.Trim(),
                    PhotoPath = PhotoPath?.Trim()
                };

                (bool success, List<string> errors) result;

                if (IsEditMode)
                {
                    result = await _contactService.UpdateContactAsync(contact);
                }
                else
                {
                    result = await _contactService.AddContactAsync(contact);
                }

                if (result.success)
                {
                    DialogResult = true;
                    RequestClose?.Invoke();
                }
                else
                {
                    foreach (var error in result.errors)
                    {
                        ValidationErrors.Add(error);
                    }

                    MessageBox.Show(
                        $"Erros de validação:\n{string.Join("\n", result.errors)}",
                        "Erro de Validação",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Falha ao salvar contato: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            DialogResult = false;
            RequestClose?.Invoke();
        }

        private void BrowsePhoto()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Arquivos de imagem (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos os arquivos (*.*)|*.*",
                Title = "Selecionar Foto do Contato"
            };

            if (dialog.ShowDialog() == true)
            {
                PhotoPath = dialog.FileName;
            }
        }

        private void ClearPhoto()
        {
            PhotoPath = string.Empty;
        }

        private void ClearValidationErrors()
        {
            ValidationErrors.Clear();
        }

        public Action? RequestClose { get; set; }

        #endregion
    }
}
