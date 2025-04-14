using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.AI;
using Microsoft.JSInterop;

namespace PersonalWebsite.Components.Pages.Apps.Messages;

public sealed partial class MessageApp
{
    [CascadingParameter(Name = "WindowId")]
    public Guid? WindowId { get; set; }
    
    private bool IsInWindow => WindowId.HasValue;
    
    private readonly IChatClient _chatClient;
    private readonly IJSRuntime _jsRuntime;

    private string InputField { get; set; } = string.Empty;
    private string StreamingMessage { get; set; } = "";
    private bool IsTyping { get; set; } = false;
    
    private string? SelectedContactId { get; set; }
    
    private List<ContactModel> Contacts { get; set; } = new();
    private Dictionary<string, List<MessageModel>> ContactMessages { get; set; } = new();
    
    public MessageApp(IChatClient chatClient, IJSRuntime jsRuntime)
    {
        _chatClient = chatClient;
        _jsRuntime = jsRuntime;
        
        // Initialize contacts and messages
        InitializeMockData();
    }
    
    private void InitializeMockData()
    {
        // Create mock contacts
        Contacts =
        [
            new ContactModel
            {
                Id = "assistant",
                Name = "AI Assistant",
                AvatarEmoji = "🤖",
                LastMessage = "I'm here to help with any questions!",
                LastMessageTime = "10:15 AM",
                UnreadCount = 0
            },

            new ContactModel
            {
                Id = "alice",
                Name = "Alice Smith",
                AvatarEmoji = "👩",
                LastMessage = "Are we still meeting for coffee tomorrow?",
                LastMessageTime = "9:32 AM",
                UnreadCount = 2
            },

            new ContactModel
            {
                Id = "bob",
                Name = "Bob Johnson",
                AvatarEmoji = "👨",
                LastMessage = "Did you see the game last night?",
                LastMessageTime = "Yesterday",
                UnreadCount = 0
            },

            new ContactModel
            {
                Id = "carol",
                Name = "Carol Williams",
                AvatarEmoji = "👩‍💼",
                LastMessage = "I sent you the project files",
                LastMessageTime = "Yesterday",
                UnreadCount = 0
            },

            new ContactModel
            {
                Id = "david",
                Name = "David Brown",
                AvatarEmoji = "🧑‍💻",
                LastMessage = "Let's review the code changes",
                LastMessageTime = "Monday",
                UnreadCount = 0
            },

            new ContactModel
            {
                Id = "emma",
                Name = "Emma Davis",
                AvatarEmoji = "👩‍🎓",
                LastMessage = "Thanks for your help with the assignment!",
                LastMessageTime = "Sunday",
                UnreadCount = 0
            }
        ];
        
        // Create mock messages
        ContactMessages = new Dictionary<string, List<MessageModel>>
        {
            ["assistant"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "assistant",
                    Text = "Hello! How can I assist you today?",
                    Time = "10:12 AM",
                    IsFromMe = false,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "assistant",
                    Text = "I'm here to help with any questions!",
                    Time = "10:15 AM",
                    IsFromMe = false,
                    IsRead = true
                }
            ],
            ["alice"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "alice",
                    Text = "Hey, how's your day going?",
                    Time = "9:15 AM",
                    IsFromMe = true,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "alice",
                    Text = "Pretty good! Just finished that report.",
                    Time = "9:20 AM",
                    IsFromMe = false,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "alice",
                    Text = "Are we still meeting for coffee tomorrow?",
                    Time = "9:32 AM",
                    IsFromMe = false,
                    IsRead = false
                }
            ],
            ["bob"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "bob",
                    Text = "Did you see the game last night?",
                    Time = "Yesterday",
                    IsFromMe = false,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "bob",
                    Text = "Yeah, it was amazing! That last-minute goal was incredible.",
                    Time = "Yesterday",
                    IsFromMe = true,
                    IsRead = true
                }
            ],
            ["carol"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "carol",
                    Text = "I sent you the project files",
                    Time = "Yesterday",
                    IsFromMe = false,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "carol",
                    Text = "Thanks, I'll take a look at them soon.",
                    Time = "Yesterday",
                    IsFromMe = true,
                    IsRead = true
                }
            ],
            ["david"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "david",
                    Text = "Let's review the code changes",
                    Time = "Monday",
                    IsFromMe = false,
                    IsRead = true
                }
            ],
            ["emma"] =
            [
                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "emma",
                    Text = "Thanks for your help with the assignment!",
                    Time = "Sunday",
                    IsFromMe = false,
                    IsRead = true
                },

                new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = "emma",
                    Text = "No problem! Let me know if you need anything else.",
                    Time = "Sunday",
                    IsFromMe = true,
                    IsRead = true
                }
            ]
        };
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Contacts.Count > 0 && SelectedContactId == null)
            {
                SelectContact("assistant");
            }
            
            await ScrollToBottom();
        }
    }
    
    private void SelectContact(string contactId)
    {
        SelectedContactId = contactId;
        
        // Mark messages as read
        if (ContactMessages.TryGetValue(contactId, out var contactMessage))
        {
            foreach (var message in contactMessage)
            {
                message.IsRead = true;
            }
            
            // Update unread count
            var contact = Contacts.FirstOrDefault(c => c.Id == contactId);
            if (contact != null)
            {
                contact.UnreadCount = 0;
            }
        }
        
        StateHasChanged();
    }
    
    private List<MessageModel> GetMessagesForContact(string contactId)
    {
        return ContactMessages.GetValueOrDefault(contactId, []);
    }
    
    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(InputField) || SelectedContactId == null)
            return;

        var input = InputField.Trim();
        InputField = string.Empty;
        
        // Add user message
        AddMessage(new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            ContactId = SelectedContactId,
            Text = input,
            Time = DateTime.Now.ToString("h:mm tt"),
            IsFromMe = true,
            IsRead = true
        });
        
        StateHasChanged();
        await ScrollToBottom();
        
        // If chatting with assistant, generate AI response
        if (SelectedContactId == "assistant")
        {
            try
            {
                // Show typing indicator
                IsTyping = true;
                StateHasChanged();
                await ScrollToBottom();
                
                // Simulate response delay
                await Task.Delay(1500);
                
                // Choose a more conversational response
                var responses = new[]
                {
                    "I'm a simulated AI assistant. In a real app, I would generate a response based on your message.",
                    "That's an interesting point! In an actual implementation, I would analyze your message and provide a relevant response.",
                    "Thanks for your message! I'm currently in demo mode, but a real AI assistant would provide a helpful answer here.",
                    "I understand what you're asking. If this were a fully implemented system, I would connect to an AI model to generate a response.",
                    "Great question! In a production environment, I would use a language model to craft a detailed answer for you."
                };
                
                var random = new Random();
                var responseText = responses[random.Next(responses.Length)];
                
                // Hide typing indicator
                IsTyping = false;
                StateHasChanged();
                
                // Add AI response message
                AddMessage(new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = SelectedContactId,
                    Text = responseText,
                    Time = DateTime.Now.ToString("h:mm tt"),
                    IsFromMe = false,
                    IsRead = true
                });
                
                StateHasChanged();
                await ScrollToBottom();
            }
            catch (Exception)
            {
                // Hide typing indicator if there's an error
                IsTyping = false;
                StateHasChanged();
                
                // Add error message
                AddMessage(new MessageModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ContactId = SelectedContactId,
                    Text = "Sorry, I'm having trouble responding right now. Please try again later.",
                    Time = DateTime.Now.ToString("h:mm tt"),
                    IsFromMe = false,
                    IsRead = true
                });
                
                StateHasChanged();
                await ScrollToBottom();
            }
        }
    }
    
    private void AddMessage(MessageModel message)
    {
        if (!ContactMessages.ContainsKey(message.ContactId))
        {
            ContactMessages[message.ContactId] = new List<MessageModel>();
        }
        
        ContactMessages[message.ContactId].Add(message);
        
        // Update contact's last message
        var contact = Contacts.FirstOrDefault(c => c.Id == message.ContactId);
        if (contact != null)
        {
            contact.LastMessage = message.Text;
            contact.LastMessageTime = message.Time;
            
            // If this message is from the contact and not currently selected, increment unread
            if (!message.IsFromMe && SelectedContactId != message.ContactId)
            {
                contact.UnreadCount++;
            }
            
            // Move the contact to the top of the list if not already there
            if (Contacts.IndexOf(contact) > 0)
            {
                Contacts.Remove(contact);
                Contacts.Insert(0, contact);
            }
        }
    }
    
    private async Task ScrollToBottom()
    {
        string containerId = IsInWindow ? "messagesContainer" : "messagesContainerStandalone";
        await _jsRuntime.InvokeVoidAsync("scrollElementToBottom", containerId);
    }
    
    public class ContactModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string AvatarEmoji { get; set; } = "";
        public string LastMessage { get; set; } = "";
        public string LastMessageTime { get; set; } = "";
        public int UnreadCount { get; set; } = 0;
    }
    
    public class MessageModel
    {
        public string Id { get; set; } = "";
        public string ContactId { get; set; } = "";
        public string Text { get; set; } = "";
        public string Time { get; set; } = "";
        public bool IsFromMe { get; set; }
        public bool IsRead { get; set; }
    }
}