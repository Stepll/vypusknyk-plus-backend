namespace VypusknykPlus.Application.Entities;

public class ChatConversation : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime LastMessageAt { get; set; }
    public bool IsClosedByAdmin { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = [];
}
