namespace VypusknykPlus.Application.Entities;

public enum ChatSenderType { User, Admin }

public class ChatMessage : BaseEntity
{
    public long ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;
    public ChatSenderType SenderType { get; set; }
    public long SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}
