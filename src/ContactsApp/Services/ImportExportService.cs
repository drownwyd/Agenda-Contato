using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public class ImportExportService
    {
        private readonly ContactService _contactService;

        public ImportExportService(ContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Export contacts to CSV file
        /// </summary>
        public async Task<(bool Success, string Message)> ExportToCsvAsync(string filePath, List<Contact>? contacts = null)
        {
            try
            {
                // If no contacts provided, export all
                contacts ??= await _contactService.GetAllContactsAsync();

                if (!contacts.Any())
                    return (false, "No contacts to export");

                var csv = new StringBuilder();

                // Header
                csv.AppendLine("FirstName,LastName,Company,PrimaryPhone,SecondaryPhone,Email,Address,Notes,PhotoPath");

                // Data rows
                foreach (var contact in contacts)
                {
                    csv.AppendLine($"{EscapeCsvField(contact.FirstName)}," +
                                 $"{EscapeCsvField(contact.LastName)}," +
                                 $"{EscapeCsvField(contact.Company)}," +
                                 $"{EscapeCsvField(contact.PrimaryPhone)}," +
                                 $"{EscapeCsvField(contact.SecondaryPhone)}," +
                                 $"{EscapeCsvField(contact.Email)}," +
                                 $"{EscapeCsvField(contact.Address)}," +
                                 $"{EscapeCsvField(contact.Notes)}," +
                                 $"{EscapeCsvField(contact.PhotoPath)}");
                }

                await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
                return (true, $"Successfully exported {contacts.Count} contacts to {filePath}");
            }
            catch (Exception ex)
            {
                return (false, $"Export failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Import contacts from CSV file
        /// </summary>
        public async Task<(bool Success, string Message, int ImportedCount, List<string> Errors)> ImportFromCsvAsync(string filePath, bool skipDuplicates = true)
        {
            var errors = new List<string>();
            var importedCount = 0;

            try
            {
                if (!File.Exists(filePath))
                    return (false, "File not found", 0, new List<string> { "File does not exist" });

                var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

                if (lines.Length < 2)
                    return (false, "CSV file is empty or has no data rows", 0, new List<string> { "No data to import" });

                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var fields = ParseCsvLine(lines[i]);

                        if (fields.Length < 9)
                        {
                            errors.Add($"Line {i + 1}: Invalid format - expected 9 fields, got {fields.Length}");
                            continue;
                        }

                        var contact = new Contact
                        {
                            FirstName = fields[0],
                            LastName = fields[1],
                            Company = fields[2],
                            PrimaryPhone = fields[3],
                            SecondaryPhone = fields[4],
                            Email = fields[5],
                            Address = fields[6],
                            Notes = fields[7],
                            PhotoPath = fields[8]
                        };

                        var result = await _contactService.AddContactAsync(contact);

                        if (result.Success)
                        {
                            importedCount++;
                        }
                        else
                        {
                            var errorMsg = $"Line {i + 1} ({contact.FirstName} {contact.LastName}): {string.Join(", ", result.Errors)}";
                            
                            if (skipDuplicates && result.Errors.Any(e => e.Contains("already exists")))
                            {
                                // Silently skip duplicates if requested
                                continue;
                            }
                            
                            errors.Add(errorMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Line {i + 1}: {ex.Message}");
                    }
                }

                var message = $"Import completed. {importedCount} contacts imported successfully.";
                if (errors.Any())
                    message += $" {errors.Count} errors occurred.";

                return (true, message, importedCount, errors);
            }
            catch (Exception ex)
            {
                return (false, $"Import failed: {ex.Message}", importedCount, errors);
            }
        }

        /// <summary>
        /// Escape CSV field (handle commas, quotes, newlines)
        /// </summary>
        private string EscapeCsvField(string? field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }

        /// <summary>
        /// Parse CSV line handling quoted fields
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    // Check for escaped quote
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            // Add last field
            fields.Add(currentField.ToString());

            return fields.ToArray();
        }

        /// <summary>
        /// Validate CSV file format
        /// </summary>
        public async Task<(bool IsValid, string Message, int RowCount)> ValidateCsvFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return (false, "File not found", 0);

                var lines = await File.ReadAllLinesAsync(filePath);

                if (lines.Length < 2)
                    return (false, "CSV file is empty or has no data rows", 0);

                // Check header
                var header = lines[0].ToLower();
                var requiredFields = new[] { "firstname", "lastname", "company", "primaryphone", "secondaryphone", "email", "address", "notes", "photopath" };
                
                foreach (var field in requiredFields)
                {
                    if (!header.Contains(field))
                        return (false, $"Missing required column: {field}", 0);
                }

                // Count data rows
                var dataRowCount = lines.Length - 1;

                return (true, $"Valid CSV file with {dataRowCount} data rows", dataRowCount);
            }
            catch (Exception ex)
            {
                return (false, $"Validation failed: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Create a sample CSV template file
        /// </summary>
        public async Task<bool> CreateCsvTemplateAsync(string filePath)
        {
            try
            {
                var template = new StringBuilder();
                template.AppendLine("FirstName,LastName,Company,PrimaryPhone,SecondaryPhone,Email,Address,Notes,PhotoPath");
                template.AppendLine("John,Doe,Acme Corp,+1234567890,+0987654321,john.doe@example.com,\"123 Main St, City, State\",Sample contact,");
                template.AppendLine("Jane,Smith,Tech Inc,+1111111111,,jane.smith@example.com,\"456 Oak Ave, Town, State\",Another example,");

                await File.WriteAllTextAsync(filePath, template.ToString(), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
