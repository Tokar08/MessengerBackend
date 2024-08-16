using AutoMapper;
using MessangerBackend.Core.Exceptions;
using MessangerBackend.Core.Services;
using MessangerBackend.DTOs;
using MessangerBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = MessangerBackend.Requests.LoginRequest;

namespace MessangerBackend.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : Controller
{
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserController(IMapper mapper, UserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }


    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(CreateUserRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var user = await _userService.Register(request.Nickname, request.Password);
            return Created("user", _mapper.Map<UserDTO>(user));
        }
        catch (UserAuthenticationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var user = await _userService.Login(loginRequest.Nickname, loginRequest.Password);
            return Ok(_mapper.Map<UserDTO>(user));
        }
        catch (UserAuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 8)
    {
        try
        {
            var users = _userService.GetUsers(page, size);
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            return user != null
                ? Ok(_mapper.Map<UserDTO>(user))
                : BadRequest("User not found!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUser([FromQuery] string nickname)
    {
        try
        {
            var users = _userService.SearchUsers(nickname);
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("search-stats")]
    public IActionResult GetSearchStatistics()
    {
        var searchStatisticsService = HttpContext.RequestServices.GetRequiredService<SearchStatisticsService>();
        var stats = searchStatisticsService.GetStatistics();
        return Ok(stats);
    }
}