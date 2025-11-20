using ContactsApp.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ContactsApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=contacts.db");
            }
        }
    }
}
