using VypusknykPlus.Application.DTOs.Chat;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public interface IChatService
{
    Task<ChatConversation> GetOrCreateConversationAsync(long userId);
    Task<List<ChatConversationListItem>> GetConversationsAsync();
    Task<ChatConversationListItem> GetConversationSummaryAsync(long conversationId);
    Task<List<ChatMessageDto>> GetMessagesAsync(long conversationId);
    Task<ChatMessageDto> SaveMessageAsync(long conversationId, ChatSenderType senderType, long senderId, string text);
    Task MarkMessagesReadAsync(long conversationId, ChatSenderType readBy);
}
