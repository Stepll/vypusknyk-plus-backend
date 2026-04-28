using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/chats")]
public class AdminChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public AdminChatController(IChatService chatService) => _chatService = chatService;

    [HttpGet]
    public async Task<IActionResult> GetConversations()
    {
        var result = await _chatService.GetConversationsAsync();
        return Ok(result);
    }

    [HttpGet("{conversationId:long}/messages")]
    public async Task<IActionResult> GetMessages(long conversationId)
    {
        var result = await _chatService.GetMessagesAsync(conversationId);
        return Ok(result);
    }
}
