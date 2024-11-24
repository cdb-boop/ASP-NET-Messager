using MessagingWebApp.Models.Domain;
using MessagingWebApp.Models.ViewModels;
using MessagingWebApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace MessagingWebApp.Controllers
{
    public class ConversationsController : Controller
    {
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly ICommunicationRepository _communicationRepository;

        private static bool IsPhoneNumber(string number)
        {
            var regex = new Regex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$"); // TODO: make constant
            return regex.Match(number).Success;
        }

        public ConversationsController(IChatGroupRepository chatGroupRepository, ICommunicationRepository communicationRepository)
        {
            _chatGroupRepository = chatGroupRepository;
            _communicationRepository = communicationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ChatGroup(Guid id)
        {
            var chatGroup = await _chatGroupRepository.GetAllAsync(id);
            if (chatGroup is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var returnUnreadMessages = chatGroup.UnreadMessages;
            chatGroup.UnreadMessages = 0;
            chatGroup = await _chatGroupRepository.UpdateAsync(chatGroup);
            if (chatGroup is null)
            {
                return RedirectToAction("Index", "Home");
            }
            chatGroup.UnreadMessages = returnUnreadMessages;

            return View(chatGroup);
        }

        [HttpPost]
        public async Task<IActionResult> ChatGroup(AddMessageRequest addMessageRequest)
        {
            if (addMessageRequest.GroupChatId is null)
            {
                // TODO: alert the user that the chat group id was null
                return RedirectToAction("Index", "Home");
            }
            var groupChatId = new Guid(addMessageRequest.GroupChatId);

            var groupChat = await _chatGroupRepository.GetAllAsync(groupChatId);
            if (groupChat is null)
            {
                // TODO: alert the user that the chat group was not found
                return RedirectToAction("Index", "Home");
            }

            var author = addMessageRequest.Author;
            if (!groupChat.Users.Any(user => user.PhoneNumber == author))
            {
                // TODO: alert the user that the user is not a member of the chat group
                return RedirectToAction("Index", "Home");
            }

            // TODO: what if this is an imposter or a fake number?

            if (groupChat.Users.Count != 2)
            {
                // TODO: alert user currently only support two users
                return RedirectToAction("Index", "Home");
            }
            string otherPhoneNumber = null;
            foreach (var user in groupChat.Users)
            {
                if (user.PhoneNumber != author)
                {
                    otherPhoneNumber = user.PhoneNumber;
                    break;
                }
            }
            if (otherPhoneNumber is null || otherPhoneNumber == "")
            {
                return RedirectToAction("Index", "Home");
            }

            var contentType = addMessageRequest.ContentType ?? "text";
            var content = addMessageRequest.Content;

            var messageResponse = await _communicationRepository.SendMessageAsync(
                addMessageRequest.Author,
                otherPhoneNumber,
                content
            );
            if (messageResponse is null)
            {
                // TODO: warn that message could not be sent
                return RedirectToAction("Index", "Home");
            }

            var newMessage = new Message
            {
                Author = author,
                SentDate = DateTime.Now,
                ContentType = contentType,
                Content = content,
                Reactions = [],
            };
            groupChat.Messages.Add(newMessage);
            groupChat = await _chatGroupRepository.UpdateAsync(groupChat);
            if (newMessage.Id == Guid.Empty) // TODO: does this behave like this?
            {
                // TODO: alert the user that the message failed to send
                return RedirectToAction("ChatGroup", new { id = groupChatId });
            }

            return RedirectToAction("ChatGroup", new { id = groupChatId });
        }

        [HttpGet]
        public IActionResult NewChatGroup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewChatGroup(CreateChatGroupRequest createChatGroupRequest)
        {
            var messageString = createChatGroupRequest.FirstMessage;
            var name = createChatGroupRequest.Name ?? "New Group Chat";
            var owner = createChatGroupRequest.Owner;
            var members = createChatGroupRequest.Members;

            if (messageString is null || messageString == "")
            {
                // TODO: warn user invalid message content
                return RedirectToAction("Index", "Home");
            }

            // TODO: what if owner is fake?
            // TODO: what if a member is invalid?

            char[] delimiterChars = { '\n', ',' };
            var phoneNumbers = members.Split(delimiterChars).ToList();
            if (phoneNumbers.Count != 1)
            {
                // TODO: warn user only can have one (free?) destination phone number
                return RedirectToAction("Index", "Home");
            }
            phoneNumbers = phoneNumbers.Prepend(owner).ToList(); // TODO: what if this is an imposter or a fake number?

            var users = new List<User>();
            foreach (var phoneNumber in phoneNumbers)
            {
                if (!IsPhoneNumber(phoneNumber))
                {
                    // TODO: warn user invalid phone number
                    return RedirectToAction("Index", "Home");
                }
                users.Add(
                    new User
                    {
                        PhoneNumber = phoneNumber,
                    }
                );
            }

            var messageResponse = await _communicationRepository.SendMessageAsync(
                    phoneNumbers[0],
                    phoneNumbers[1],
                    messageString
                );
            if (messageResponse is null)
            {
                // TODO: warn that message could not be sent
                return RedirectToAction("Index", "Home");
            }

            var message = new Message
            {
                Author = owner,
                SentDate = DateTime.Now,
                ContentType = "text", // TODO: detect it dynamically
                Content = messageString,
                Reactions = [],
            };

            var chatGroup = await _chatGroupRepository.GetAllAsync(users);
            if (chatGroup is not null)
            {
                // chat group already exists
                chatGroup.Messages.Add(message);
                chatGroup = await _chatGroupRepository.UpdateAsync(chatGroup);
            }
            else
            {
                // create new chat group
                chatGroup = new ChatGroup
                {
                    UnreadMessages = 0,
                    Users = users,
                    Messages = [message],
                    Name = name,
                    LastMessage = message.Content, // TODO: not text?
                    LastAuthor = owner,
                    Owner = users[0].PhoneNumber,
                };
                chatGroup = await _chatGroupRepository.AddAsync(chatGroup);
            }
            if (chatGroup is null || chatGroup.Id == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("ChatGroup", new { id = chatGroup.Id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateName(UpdateChatGroupNameRequest updateChatGroupNameRequest)
        {
            if (updateChatGroupNameRequest.Id is null)
            {
                // TODO: alert the user that the chat group id was null
                return RedirectToAction("Index", "Home");
            }
            var groupChatId = new Guid(updateChatGroupNameRequest.Id);

            var groupChat = await _chatGroupRepository.GetAllAsync(groupChatId);
            if (groupChat is null)
            {
                // TODO: alert the user that the chat group was not found
                return RedirectToAction("Index", "Home");
            }

            // TODO: only let owner change name

            var newName = updateChatGroupNameRequest.Name;
            if (newName is null || newName == "")
            {
                // TODO: alert the user that name cannot be null or empty
                return RedirectToAction("Index", "Home");
            }

            groupChat.Name = newName;
            await _chatGroupRepository.UpdateAsync(groupChat);

            return RedirectToAction("ChatGroup", new { id = groupChatId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveGroupChat(Guid id)
        {
            var chatGroup = await _chatGroupRepository.DeleteAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
