namespace VypusknykPlus.Application.DTOs.Chat;

public class ChatMessageDto
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public long SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}

public class ChatConversationListItem
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public bool IsClosedByAdmin { get; set; }
}
