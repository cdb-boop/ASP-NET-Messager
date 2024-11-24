using System.ComponentModel.DataAnnotations;

namespace MessagingWebApp.Models.ViewModels
{
    public class UpdateChatGroupNameRequest
    {
        public string Id { get; set; }
        [Required]
        public required string Name { get; set; }
    }
}
