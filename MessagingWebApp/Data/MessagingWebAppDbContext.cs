using MessagingWebApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace MessagingWebApp.Data
{
    public class MessagingWebAppDbContext : DbContext
    {
        public MessagingWebAppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
