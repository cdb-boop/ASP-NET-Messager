using MessagingWebApp.Models.Domain;
using MessagingWebApp.Models.ViewModels;
using MessagingWebApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Mime;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace MessagingWebApp.Controllers
{
    public class SmsController : TwilioController
    {
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        public SmsController(IChatGroupRepository chatGroupRepository, IHubContext<ChatHub> hubContext)
        {
            _chatGroupRepository = chatGroupRepository;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<TwiMLResult> Index(SmsRequest request)
        {
            var fromPhoneNumber = request.From;
            var toPhoneNumber = request.To;
            ChatGroup? chatGroup = await _chatGroupRepository.GetAllAsync(fromPhoneNumber, toPhoneNumber);
            var message = new Message
            {
                Author = fromPhoneNumber,
                SentDate = DateTime.Now,
                ContentType = "text", // TODO: dynamically detect
                Content = request.Body,
                MessageRead = false,
                Reactions = []
            };
            if (chatGroup is not null)
            {
                chatGroup.Messages.Add(message);
                chatGroup.UnreadMessages += 1;
                chatGroup.LastMessage = message.Content;
                chatGroup.LastAuthor = fromPhoneNumber;

                chatGroup = await _chatGroupRepository.UpdateAsync(chatGroup);
                if (message.Id == Guid.Empty) // TODO: does the message value get properly updated?
                {
                    // TODO: failed to save message
                }
                else
                {
                    // notify client about new message
                    await SendAllMessageNotification($"{fromPhoneNumber}: {message.Content}");
                }
            }
            else
            {
                chatGroup = new ChatGroup
                {
                    UnreadMessages = 1,
                    Users = [
                        new User{ PhoneNumber = fromPhoneNumber },
                        new User{ PhoneNumber = toPhoneNumber },
                    ],
                    Messages = [message],
                    Name = message.Content.ToString()[..Math.Min(message.Content.Length, 50)],
                    LastMessage = message.Content,
                    LastAuthor = fromPhoneNumber,
                    Owner = fromPhoneNumber,
                };

                chatGroup = await _chatGroupRepository.AddAsync(chatGroup);
                if (chatGroup.Id == Guid.Empty)
                {
                    // TODO: failed to add group
                }
                else
                {
                    // TODO: notify user
                    await SendAllMessageNotification($"New chat from {fromPhoneNumber}: {message.Content}");
                }
            }
            return TwiML(new MessagingResponse());
        }

        public async Task SendNotification(string userId, string message)
        {
            // TODO: notify specific user(s) -> login?
            //await _hubContext.Clients.User(userId).SendAsync(message);

            await _hubContext.Clients.All.SendAsync(message);
        }

        public async Task SendAllMessageNotification(string message)
        {
            // TODO: remove this; should always send to a specific set of users
            await _hubContext.Clients.All.SendAsync("MessageNotification", message);
        }
    }
}
