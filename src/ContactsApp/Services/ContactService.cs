using ContactsApp.Models;
using ContactsApp.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public class ContactService
    {
        private readonly ContactRepository _repository;

        public ContactService(ContactRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all contacts
        /// </summary>
        public async Task<List<Contact>> GetAllContactsAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Get contact by ID
        /// </summary>
        public async Task<Contact?> GetContactByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid contact ID", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Search contacts by name, phone, email, or company
        /// </summary>
        public async Task<List<Contact>> SearchContactsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllContactsAsync();

            var allContacts = await _repository.GetAllAsync();
            var term = searchTerm.ToLower();

            return allContacts.Where(c =>
                (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.ToLower().Contains(term)) ||
                (!string.IsNullOrEmpty(c.LastName) && c.LastName.ToLower().Contains(term)) ||
                (!string.IsNullOrEmpty(c.Company) && c.Company.ToLower().Contains(term)) ||
                (!string.IsNullOrEmpty(c.PrimaryPhone) && c.PrimaryPhone.Contains(term)) ||
                (!string.IsNullOrEmpty(c.SecondaryPhone) && c.SecondaryPhone.Contains(term)) ||
                (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(term))
            ).ToList();
        }

        /// <summary>
        /// Add a new contact with validation
        /// </summary>
        public async Task<(bool Success, List<string> Errors)> AddContactAsync(Contact contact)
        {
            var validationErrors = ValidateContact(contact);
            if (validationErrors.Any())
                return (false, validationErrors);

            // Check for duplicate phone numbers
            var duplicateCheck = await CheckDuplicatePhoneAsync(contact.PrimaryPhone, contact.Id);
            if (duplicateCheck.IsDuplicate)
            {
                validationErrors.Add($"Número de telefone já existe para o contato: {duplicateCheck.ExistingContactName}");
                return (false, validationErrors);
            }

            contact.CreatedAt = DateTime.UtcNow;
            contact.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(contact);
            return (true, new List<string>());
        }

        /// <summary>
        /// Update an existing contact with validation
        /// </summary>
        public async Task<(bool Success, List<string> Errors)> UpdateContactAsync(Contact contact)
        {
            if (contact.Id <= 0)
                return (false, new List<string> { "ID de contato inválido" });

            var existingContact = await _repository.GetByIdAsync(contact.Id);
            if (existingContact == null)
                return (false, new List<string> { "Contato não encontrado" });

            var validationErrors = ValidateContact(contact);
            if (validationErrors.Any())
                return (false, validationErrors);

            // Check for duplicate phone numbers (excluding current contact)
            var duplicateCheck = await CheckDuplicatePhoneAsync(contact.PrimaryPhone, contact.Id);
            if (duplicateCheck.IsDuplicate)
            {
                validationErrors.Add($"Número de telefone já existe para o contato: {duplicateCheck.ExistingContactName}");
                return (false, validationErrors);
            }

            contact.UpdatedAt = DateTime.UtcNow;
            contact.CreatedAt = existingContact.CreatedAt; // Preserve original creation date

            await _repository.UpdateAsync(contact);
            return (true, new List<string>());
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        public async Task<bool> DeleteContactAsync(int id)
        {
            if (id <= 0)
                return false;

            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Validate contact data
        /// </summary>
        private List<string> ValidateContact(Contact contact)
        {
            var errors = new List<string>();
            var validationContext = new ValidationContext(contact);
            var validationResults = new List<ValidationResult>();

            // Use DataAnnotations validation
            if (!Validator.TryValidateObject(contact, validationContext, validationResults, true))
            {
                errors.AddRange(validationResults.Select(vr => vr.ErrorMessage ?? "Erro de validação"));
            }

            // Additional business rules
            if (string.IsNullOrWhiteSpace(contact.FirstName))
                errors.Add("Nome é obrigatório");

            if (!string.IsNullOrWhiteSpace(contact.Email) && !IsValidEmail(contact.Email))
                errors.Add("Formato de email inválido");

            if (!string.IsNullOrWhiteSpace(contact.PrimaryPhone) && !IsValidPhone(contact.PrimaryPhone))
                errors.Add("Formato de telefone principal inválido");

            if (!string.IsNullOrWhiteSpace(contact.SecondaryPhone) && !IsValidPhone(contact.SecondaryPhone))
                errors.Add("Formato de telefone secundário inválido");

            return errors.Distinct().ToList();
        }

        /// <summary>
        /// Check if phone number already exists
        /// </summary>
        private async Task<(bool IsDuplicate, string ExistingContactName)> CheckDuplicatePhoneAsync(string? phone, int excludeContactId = 0)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return (false, string.Empty);

            var allContacts = await _repository.GetAllAsync();
            var duplicate = allContacts.FirstOrDefault(c =>
                c.Id != excludeContactId &&
                (c.PrimaryPhone == phone || c.SecondaryPhone == phone)
            );

            if (duplicate != null)
            {
                var name = $"{duplicate.FirstName} {duplicate.LastName}".Trim();
                return (true, name);
            }

            return (false, string.Empty);
        }

        /// <summary>
        /// Validate email format
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate phone format (basic validation)
        /// </summary>
        private bool IsValidPhone(string phone)
        {
            // Remove common formatting characters
            var cleaned = new string(phone.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Phone should have at least 10 digits
            return cleaned.Length >= 10 && cleaned.Length <= 15;
        }

        /// <summary>
        /// Sort contacts by specified field
        /// </summary>
        public async Task<List<Contact>> GetSortedContactsAsync(string sortBy = "FirstName", bool ascending = true)
        {
            var contacts = await _repository.GetAllAsync();

            return sortBy.ToLower() switch
            {
                "firstname" => ascending
                    ? contacts.OrderBy(c => c.FirstName).ToList()
                    : contacts.OrderByDescending(c => c.FirstName).ToList(),
                "lastname" => ascending
                    ? contacts.OrderBy(c => c.LastName).ToList()
                    : contacts.OrderByDescending(c => c.LastName).ToList(),
                "company" => ascending
                    ? contacts.OrderBy(c => c.Company).ToList()
                    : contacts.OrderByDescending(c => c.Company).ToList(),
                "email" => ascending
                    ? contacts.OrderBy(c => c.Email).ToList()
                    : contacts.OrderByDescending(c => c.Email).ToList(),
                "createdat" => ascending
                    ? contacts.OrderBy(c => c.CreatedAt).ToList()
                    : contacts.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => ascending
                    ? contacts.OrderBy(c => c.FirstName).ToList()
                    : contacts.OrderByDescending(c => c.FirstName).ToList()
            };
        }

        /// <summary>
        /// Get paginated contacts
        /// </summary>
        public async Task<(List<Contact> Contacts, int TotalCount, int TotalPages)> GetPaginatedContactsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string sortBy = "FirstName",
            bool ascending = true)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;

            var allContacts = await GetSortedContactsAsync(sortBy, ascending);
            var totalCount = allContacts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedContacts = allContacts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (paginatedContacts, totalCount, totalPages);
        }
    }
}
