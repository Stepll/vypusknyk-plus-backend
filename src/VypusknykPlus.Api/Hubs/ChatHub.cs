using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService) => _chatService = chatService;

    public override async Task OnConnectedAsync()
    {
        if (IsAdmin())
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"admin:{GetCallerId()}");
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(long conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }

    public async Task SendMessage(long conversationId, string text)
    {
        var senderType = IsAdmin() ? ChatSenderType.Admin : ChatSenderType.User;
        var senderId = GetCallerId();

        var message = await _chatService.SaveMessageAsync(conversationId, senderType, senderId, text);
        var summary = await _chatService.GetConversationSummaryAsync(conversationId);

        await Clients.Group(GroupName(conversationId)).SendAsync("ReceiveMessage", message);
        await Clients.Group("admins").SendAsync("ConversationUpdated", summary);
    }

    public async Task MarkRead(long conversationId)
    {
        var readBy = IsAdmin() ? ChatSenderType.Admin : ChatSenderType.User;
        await _chatService.MarkMessagesReadAsync(conversationId, readBy);

        var summary = await _chatService.GetConversationSummaryAsync(conversationId);
        await Clients.Group("admins").SendAsync("ConversationUpdated", summary);
    }

    private static string GroupName(long conversationId) => $"conversation:{conversationId}";
    private bool IsAdmin() => Context.User?.IsInRole("Admin") ?? false;
    private long GetCallerId() => long.Parse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}
