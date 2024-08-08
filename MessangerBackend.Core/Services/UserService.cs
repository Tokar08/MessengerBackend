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

    public Task<User> Login(string nickname, string password)
    {
        throw new NotImplementedException();
    }

    public Task<User> Register(string nickname, string password)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetUsers(int page, int size)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> SearchUsers(string nickname)
    {
        throw new NotImplementedException();
    }
}