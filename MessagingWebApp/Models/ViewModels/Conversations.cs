using MessagingWebApp.Models.Domain;

namespace MessagingWebApp.Models.ViewModels
{
    public class Conversations
    {
        public IEnumerable<ChatGroup> ChatGroups { get; set; }
    }
}
