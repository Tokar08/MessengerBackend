using MessangerBackend.Core.Models;
using MessangerBackend.DTOs;
using MessangerBackend.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessangerBackend.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : Controller
{
    private readonly MessangerContext _context;

    public ChatController(MessangerContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
    {
        var chats = _context.PrivateChats.Include(x => x.Users).Cast<Chat>().ToList();
        chats.AddRange(_context.GroupChats);

        return chats;
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser(UserDTO user)
    {
        var userDb = new User()
        {
            Nickname = user.Nickname,
            Password = user.Password,
            CreatedAt = DateTime.Now,
            LastSeenOnline = DateTime.Now
        };
        _context.Users.Add(userDb);
        await _context.SaveChangesAsync();
        return Created("user", user);
    }
    
    [HttpPost("createPrivateChat")]
    public async Task<ActionResult<PrivateChat>> AddPrivateChat(PrivateChatDTO chat)
    {
        var privateChat = new PrivateChat()
        {
            Users = _context.Users.Where(x => chat.UsersIds.Contains(x.Id)).ToList(),
            CreatedAt = DateTime.Now
        };
        _context.PrivateChats.Add(privateChat);
        await _context.SaveChangesAsync();
        return Created("chat", chat);
    }
}