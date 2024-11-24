using Azure;
using MessagingWebApp.Data;
using MessagingWebApp.Models.Domain;
using MessagingWebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Twilio.Types;

namespace MessagingWebApp.Repositories
{
    public class ChatGroupRepository : IChatGroupRepository
    {
        private readonly MessagingWebAppDbContext _messagingWebAppDpContext;

        public ChatGroupRepository(MessagingWebAppDbContext messagingWebAppDpContext)
        {
            _messagingWebAppDpContext = messagingWebAppDpContext;
        }

        public async Task<ChatGroup> AddAsync(ChatGroup chatGroup)
        {
            await _messagingWebAppDpContext.ChatGroups.AddAsync(chatGroup);
            await _messagingWebAppDpContext.SaveChangesAsync();
            return chatGroup;
        }

        public async Task<ChatGroup?> GetAsync(Guid id)
        {
            return await _messagingWebAppDpContext.ChatGroups
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ChatGroup?> GetAllAsync(Guid id)
        {
            return await _messagingWebAppDpContext.ChatGroups
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ChatGroup>> GetAllAsync()
        {
            return await _messagingWebAppDpContext.ChatGroups
                .Include(group => group.Users)
                .Include(group => group.Messages) // TODO: this is not efficient for just wanting previews
                .ToListAsync();
        }

        public async Task<ChatGroup?> UpdateAsync(ChatGroup chatGroup)
        {
            ChatGroup? dbChatGroup = await _messagingWebAppDpContext.ChatGroups
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .SingleOrDefaultAsync(x => x.Id == chatGroup.Id);
            if (dbChatGroup is not null)
            {
                dbChatGroup.UnreadMessages = chatGroup.UnreadMessages;
                dbChatGroup.Users = chatGroup.Users;
                dbChatGroup.Messages = chatGroup.Messages;
                dbChatGroup.Name = chatGroup.Name;
                dbChatGroup.LastMessage = chatGroup.LastMessage;
                dbChatGroup.LastAuthor = chatGroup.LastAuthor;
                dbChatGroup.Owner = chatGroup.Owner;
                await _messagingWebAppDpContext.SaveChangesAsync();
            }
            return dbChatGroup;
        }

        public async Task<ChatGroup?> DeleteAsync(Guid id)
        {
            ChatGroup? chatGroup = await _messagingWebAppDpContext.ChatGroups
                .Include(x => x.Users)
                .Include(group => group.Messages)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (chatGroup is not null)
            {
                _messagingWebAppDpContext.Remove(chatGroup);
                await _messagingWebAppDpContext.SaveChangesAsync();
            }
            return chatGroup;
        }

        public async Task<ChatGroup?> GetAllAsync(string toPhoneNumber, string fromPhoneNumber)
        {
            return await _messagingWebAppDpContext.ChatGroups
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .SingleOrDefaultAsync(x =>
                    x.Users.Any(x => x.PhoneNumber == fromPhoneNumber) &&
                    x.Users.Any(x => x.PhoneNumber == toPhoneNumber)
                );
        }

        public async Task<ChatGroup?> GetAllAsync(IEnumerable<User> users)
        {
            var userPhoneNumbers = users.Select(u => u.PhoneNumber).ToList();
            return await _messagingWebAppDpContext.ChatGroups
                .Include(chatGroup => chatGroup.Users)
                .Include(chatGroup => chatGroup.Messages)
                .SingleOrDefaultAsync(chatGroup =>
                    chatGroup.Users.Count() == userPhoneNumbers.Count &&
                    userPhoneNumbers.All(phoneNumber => 
                        chatGroup.Users.Any(chatGroupUser => chatGroupUser.PhoneNumber == phoneNumber)
                    )
                );
        }
    }
}
