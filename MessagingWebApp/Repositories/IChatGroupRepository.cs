using MessagingWebApp.Models.Domain;
using MessagingWebApp.Models.ViewModels;

namespace MessagingWebApp.Repositories
{
    public interface IChatGroupRepository
    {
        Task<ChatGroup> AddAsync(ChatGroup chatGroup);

        Task<ChatGroup?> GetAsync(Guid id);
        Task<ChatGroup?> GetAllAsync(Guid id);
        Task<IEnumerable<ChatGroup>> GetAllAsync();
        Task<ChatGroup?> UpdateAsync(ChatGroup chatGroup);
        Task<ChatGroup?> DeleteAsync(Guid id);
        Task<ChatGroup?> GetAllAsync(string toPhoneNumber, string fromPhoneNumber);
        Task<ChatGroup?> GetAllAsync(IEnumerable<User> users);
    }
}
