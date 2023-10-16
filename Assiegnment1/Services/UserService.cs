using Library.DTO;
using ServerSide.Models;
using ServerSide.Repositories;

namespace ServerSide.Services;

public interface IUserService
{
    public Task CreateUser(UserRegesrationDTO userDto);
    public Task<List<UserDataDTO>> ListUsers();
    public Task BlockUserAsync(string username);

}

class UserService : IUserService
{
    private readonly IUserRepository _userRepository;


    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    // [Admin]
    public async Task CreateUser(UserRegesrationDTO userDto)
    {
        try
        {
            //Generate token

            if (await _userRepository.ContainsAsync(userDto.UserName))
            {
                throw new Exception($"user {userDto.UserName} already exists");
            }
            int userId = await _userRepository.GetMaxUserID();

            string hashedpassword = HashingService.HashingPassword(userDto.Password);

            User user = new User
            {
                UserID = userId,
                UserName = userDto.UserName,
                Password = hashedpassword,
                UserRole = (Models.Role)userDto.UserRole,
                DepartmentID = userDto.DepartmentID
            };

            await _userRepository.AddAsync(user);

            await _userRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    //[Admin]
    public async Task<List<UserDataDTO>> ListUsers()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    //[Admin]
    public async Task BlockUserAsync(string username)
    {
        try
        {
            User user = await _userRepository.GetUserByNameAsync(username);

            _userRepository.BlockUnBlock(user);

            await _userRepository.SaveChangesAsync();
        }
        catch
        {
            throw new Exception($"Error in blocking {username}");
        }
    }

   
}
