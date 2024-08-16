using MessangerBackend.Core.Exceptions;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Models;

namespace MessangerBackend.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> Login(string nickname, string password)
    {
        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
        {
            throw new UserAuthenticationException("Nickname and password cannot be empty or whitespace!");
        }
        
        var users = await _repository.GetQuery<User>(u => u.Nickname == nickname);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UserAuthenticationException("Invalid data for login!");

        return user;
    }

    public async Task<User> Register(string nickname, string password)
    {
        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
        {
            throw new UserAuthenticationException("Nickname and password cannot be empty or whitespace!");
        }

        var existingUsers = await _repository.GetQuery<User>(u => u.Nickname == nickname);
        var existingUser = existingUsers.FirstOrDefault();

        if (existingUser != null)
        {
            throw new UserAuthenticationException("There is already a user with this nickname!");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Nickname = nickname,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            LastSeenOnline = DateTime.UtcNow
        };

        await _repository.Add(user);
        return user;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _repository.GetById<User>(id);
    }

    public IEnumerable<User> GetUsers(int page, int size)
    {
        return _repository.GetAll<User>().Skip((page - 1) * size).Take(size).ToList();
    }

    public IEnumerable<User> SearchUsers(string nickname)
    {
        return _repository.GetAll<User>().AsEnumerable()
            .Where(u => u.Nickname.Contains(nickname, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}