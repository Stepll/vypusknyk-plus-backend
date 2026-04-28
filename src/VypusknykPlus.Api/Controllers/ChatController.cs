using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService) => _chatService = chatService;

    [HttpGet("my")]
    public async Task<IActionResult> GetMyConversation()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var conversation = await _chatService.GetOrCreateConversationAsync(userId);
        return Ok(new { conversation.Id });
    }

    [HttpGet("my/messages")]
    public async Task<IActionResult> GetMyMessages()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var conversation = await _chatService.GetOrCreateConversationAsync(userId);
        var messages = await _chatService.GetMessagesAsync(conversation.Id);
        return Ok(messages);
    }
}
