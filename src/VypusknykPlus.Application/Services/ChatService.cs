using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Chat;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _db;

    public ChatService(AppDbContext db) => _db = db;

    public async Task<ChatConversation> GetOrCreateConversationAsync(long userId)
    {
        var conversation = await _db.ChatConversations
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

        if (conversation is not null)
            return conversation;

        conversation = new ChatConversation
        {
            UserId = userId,
            LastMessageAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _db.ChatConversations.Add(conversation);
        await _db.SaveChangesAsync();
        return conversation;
    }

    public async Task<List<ChatConversationListItem>> GetConversationsAsync()
    {
        var conversations = await _db.ChatConversations
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();

        return conversations.Select(MapConversation).ToList();
    }

    public async Task<ChatConversationListItem> GetConversationSummaryAsync(long conversationId)
    {
        var conversation = await _db.ChatConversations
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId)
            ?? throw new KeyNotFoundException($"Чат {conversationId} не знайдено");

        return MapConversation(conversation);
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(long conversationId)
    {
        return await _db.ChatMessages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderBy(m => m.SentAt)
            .Select(m => MapMessage(m))
            .ToListAsync();
    }

    public async Task<ChatMessageDto> SaveMessageAsync(long conversationId, ChatSenderType senderType, long senderId, string text)
    {
        var now = DateTime.UtcNow;

        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderType = senderType,
            SenderId = senderId,
            Text = text,
            SentAt = now,
            IsRead = false,
            CreatedAt = now,
            UpdatedAt = now,
        };

        _db.ChatMessages.Add(message);

        var conversation = await _db.ChatConversations.FindAsync(conversationId)
            ?? throw new KeyNotFoundException($"Чат {conversationId} не знайдено");
        conversation.LastMessageAt = now;
        conversation.UpdatedAt = now;

        await _db.SaveChangesAsync();
        return MapMessage(message);
    }

    public async Task MarkMessagesReadAsync(long conversationId, ChatSenderType readBy)
    {
        // readBy = Admin → mark User messages as read (admin reads user's messages)
        // readBy = User  → mark Admin messages as read (user reads admin's messages)
        var senderType = readBy == ChatSenderType.Admin ? ChatSenderType.User : ChatSenderType.Admin;

        var unread = await _db.ChatMessages
            .Where(m => m.ConversationId == conversationId && m.SenderType == senderType && !m.IsRead && !m.IsDeleted)
            .ToListAsync();

        foreach (var m in unread)
        {
            m.IsRead = true;
            m.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    private static ChatConversationListItem MapConversation(ChatConversation c)
    {
        var lastMsg = c.Messages
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefault();

        var unreadCount = c.Messages.Count(m => m.SenderType == ChatSenderType.User && !m.IsRead && !m.IsDeleted);

        return new ChatConversationListItem
        {
            Id = c.Id,
            UserId = c.UserId,
            UserFullName = c.User.FullName,
            UserEmail = c.User.Email,
            LastMessage = lastMsg?.Text,
            LastMessageAt = lastMsg?.SentAt,
            UnreadCount = unreadCount,
            IsClosedByAdmin = c.IsClosedByAdmin,
        };
    }

    private static ChatMessageDto MapMessage(ChatMessage m) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderType = m.SenderType.ToString(),
        SenderId = m.SenderId,
        Text = m.Text,
        SentAt = m.SentAt,
        IsRead = m.IsRead,
    };
}
