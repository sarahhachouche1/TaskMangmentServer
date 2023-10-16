using Library.DTO;
using ServerSide.Models;
using ServerSide.Repositories;

namespace ServerSide.Services;

public interface IAuthenticationService
{
    public Task Login(UserAuthenticationDTO useDto);

    public Task<string> Logout(UserAuthenticationDTO useDto);

    public Task<string> ChangePasswordAsync(string username, string newPassword);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Login(UserAuthenticationDTO userDto)
    {
        //generate token

        try
        {
            User user = await _userRepository.GetUserByNameAsync(userDto.UserName);
            if (user.IsBlocked == true) { throw new Exception($"{user.UserName}is Blocked, Cant Login "); }
            if (!HashingService.ValidatePassword(userDto.Password, user.Password))
                throw new Exception("Username or password is incorrect");

            user.IsActive = true;
            Console.WriteLine(user.IsActive);
            await _userRepository.SaveChangesAsync();

        }
        catch (Exception ex) { Console.WriteLine(ex); throw; }
    }

    public async Task<string> Logout(UserAuthenticationDTO useDto)
    {
        return "Logout";
    }

    public async Task<string> ChangePasswordAsync(string username, string newPassword)
    {
        try
        {
            User user = await _userRepository.GetUserByNameAsync(username);
            if (user.Password == newPassword)
            {
                throw new Exception("new password same as the old password");
            }
            string newhashedpassword = HashingService.HashingPassword(newPassword);
            user.Password = newhashedpassword;
            await _userRepository.SaveChangesAsync();
            return "Password Succesfully changed";
        }
        catch (Exception)
        {
            throw;
        }
    }
}



