using MessangerBackend.Core.Models;
using MessangerBackend.DTOs;
using MessangerBackend.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessangerBackend.Controllers;

[ApiController]
[Route("message")]
public class MessageController : Controller
{
    private readonly MessangerContext _context;

    public MessageController(MessangerContext context)
    {
        _context = context;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<PrivateChatDTO>> CreatePrivateChat(PrivateChatDTO privateChatDto)
    {
        var users = _context.Users.Where(x => privateChatDto.UsersIds.Contains(x.Id)).ToList();
        var privateChat = new PrivateChat
        {
            Users = users,
            CreatedAt = DateTime.UtcNow
        };
        _context.Add(privateChat);
        await _context.SaveChangesAsync();

        return Ok(privateChatDto);
    }

    [HttpPost]
    public async Task<ActionResult<bool>> SendMessage(MessageDTO messageDto)
    {
        var sender = _context.Users.Single(x => x.Id == messageDto.SenderId);
        var chat = _context.PrivateChats.Single(x => x.Id == messageDto.ChatId);
        var message = new Message
        {
            Content = messageDto.Text,
            Sender = sender,
            Chat = chat,
            SentAt = DateTime.UtcNow
        };

        _context.Add(message);
        await _context.SaveChangesAsync();
        return true;
    }

    [HttpGet("user/{userId}/messages")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesByUserId(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return NotFound($"User with id {userId} not found.");

        var messages = await _context.Messages
            .Where(m => m.Sender.Id == userId)
            .Select(m => new MessageDTO
            {
                SenderId = m.Sender.Id,
                ChatId = m.Chat.Id,
                Text = m.Content
            })
            .ToListAsync();

        return Ok(messages);
    }
}