using MessangerBackend.Core.Exceptions;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.Core.Models;
using MessangerBackend.Core.Services;
using MessangerBackend.Storage;
using Microsoft.EntityFrameworkCore;

namespace MessangerBackend.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task UserService_Login_CorrectInput()
    {
        // AAA Assign, Act, Assert
        var userService = CreateUserService();
        
        var password = "123456789";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var expectedUser = new User()
        {
            Nickname = "TestUser",
            Password = hashedPassword,
        };
       
        var user = await userService.Login("TestUser", password);
            
        Assert.Equal(expectedUser.Nickname, user.Nickname);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, user.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task UserService_Login_ThrowsExceptionWhenEmptyField(string data = null)
    {
        // Assign
        var service = CreateUserService();
        
        // Act
        var exceptionNicknameHandler = async () =>
        {
            await service.Login(data, "1234");
        };
        var exceptionPasswordHandler = async () =>
        {
            await service.Login("nick", data);
        };
        
        // Assert
        await Assert.ThrowsAsync<UserAuthenticationException>(exceptionNicknameHandler);
        await Assert.ThrowsAsync<UserAuthenticationException>(exceptionPasswordHandler);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("nickname", "")]
    [InlineData("", "password")]
    [InlineData(null, "password")]
    [InlineData("nickname", null)]
    public async Task UserService_Register_ThrowsExceptionWhenIncorrectData(string nickname, string password)
    {
        var service = CreateUserService();
        var exceptionHandler = async () => await service.Register(nickname, password);
        await Assert.ThrowsAsync<UserAuthenticationException>(exceptionHandler);
    }
    
    
    [Fact]
    public async Task SearchUsers_ReturnsUser_WhenNicknameIsCorrect()
    {
        var userService = CreateUserService();
        await userService.Register("TestUser2", "password");
        var result = userService.SearchUsers("TestUser2").ToList();
        
        Assert.Single(result);
        Assert.Equal("TestUser2", result[0].Nickname);
    }

    
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public async Task SearchUsers_ReturnsEmpty_WhenNicknameIsEmpty(string nickname)
    {
        
        var userService = CreateUserService();
        try
        {
            await userService.Register("", "password");
            await userService.Register("  ", "password");
            await userService.Register(null, "password");
        }
        catch (UserAuthenticationException)
        {
     
        }
      
        var result = userService.SearchUsers(nickname).ToList();
        Assert.Empty(result);
    }
    
    
    [Fact]
    public async Task SearchUsers_ReturnsEmpty_WhenNicknameNotFound()
    {
        var userService = CreateUserService();
        await userService.Register("TestUser3", "password");
        var result = userService.SearchUsers("UserNotFound").ToList();
        
        Assert.Empty(result);
    }
    
   
    [Fact]
    public async Task SearchUsers_ReturnsUser_WhenNicknameIsPartOfAnother()
    {
        var userService = CreateUserService();
        await userService.Register("TestDevUser", "password");
        var result = userService.SearchUsers("Dev").ToList();
    
        Assert.Single(result);
        Assert.Equal("TestDevUser", result[0].Nickname);
    }
    
  
    [Fact]
    public async Task SearchUsers_IsCaseInsensitive()
    {
        var userService = CreateUserService();
        await userService.Register("KAGUR", "password");
        var result = userService.SearchUsers("kagur").ToList();
        
        Assert.Single(result);
        Assert.Equal("KAGUR", result[0].Nickname);
    }
    
    //HW:
    [Fact]
    public async Task UserService_Register_CorrectData()
    {
        var userService = CreateUserService();
        const string expectedNickname = "NewUserInTestForRegister";
        var user = await userService.Register(expectedNickname, "passwordInTestForRegister");

        Assert.NotNull(user);
        Assert.Equal(expectedNickname, user.Nickname);
        Assert.True(BCrypt.Net.BCrypt.Verify("passwordInTestForRegister", user.Password));
        
    }
    
    [Fact]
    public void UserService_GetUsers_ReturnNotEmptyWhenUsersExist()
    {
        var userService = CreateUserService();
        var users = userService.GetUsers(1,10).ToList();
        Assert.NotEmpty(users);
    }

    [Theory]
    [InlineData("ユーザー", "パスワード")]
    [InlineData("ТестовыйЮзер", "Пароль1234")]
    [InlineData("TestUser%^$", "P@ssw0rd")]
    public async Task UserService_Register_WithUnicodeNicknames(string nickname, string password)
    {
        var userService = CreateUserService();
        var user = await userService.Register(nickname, password);
        var searchedUser = userService.SearchUsers(nickname).ToList();
        
        Assert.NotNull(user);
        Assert.Equal(nickname, user.Nickname);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, user.Password));
        Assert.Single(searchedUser);
        Assert.Equal(nickname, searchedUser[0].Nickname);
    }


    private IUserService CreateUserService()
    {
        var options = new DbContextOptionsBuilder<MessangerContext>()
            .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MessangerDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
            .Options;
        var context = new MessangerContext(options);
        var repository = new Repository(context);
        return new UserService(repository);
    }

}
