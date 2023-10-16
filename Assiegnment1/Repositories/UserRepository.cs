using ServerSide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading;
using Library.DTO;
using ServerSide.Configurations;

namespace ServerSide.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<bool> ContainsAsync(string username);
        public Task<int> GetMaxUserID();
        public Task<User> GetUserByNameAsync(string username);
        public Task<User> GetUserByIDAsync(int userid);
        public Task UpdateAsync(User userToUpdate);
        public void BlockUnBlock(User user);
        public Task<List<UserDataDTO>> GetAllUsersAsync();
    }
    public sealed class UserRepository : IUserRepository
    {
        private List<User>? _users;
        private List<Department> _departments;
        private readonly IFileHandler _filehandler;
        private static readonly object lockob = new object();
        private static UserRepository instance = null;
        private static SemaphoreSlim _semaphore1 = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim _semaphore2 = new SemaphoreSlim(1, 1);
        private static DepartmentRepository _departmentRepository = DepartmentRepository.Instance;
        private readonly IConfiguration config;
        private readonly string _userDataPath;
        public static UserRepository Instance
        {

            get
            {
                lock (lockob)
                {
                    if (instance == null)
                    {
                        instance = new UserRepository(new FileHandler());
                    }
                    return instance;
                }
            }

        }

        private UserRepository(IFileHandler filehandler)
        {
            _filehandler = filehandler;
            config = AppConfiguration.Configuration();
            _userDataPath = config["AppSettings:Users"];
        }

        private async Task<List<User>> InitiateUsersAsync()
        {

            try
            {
                await _semaphore1.WaitAsync();
                if (_users == null)
                {
                    string userDataJson = await _filehandler.ReadFileAsync(_userDataPath);
                    _users = JsonConvert.DeserializeObject<List<User>>(userDataJson);

                }
                return _users;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _semaphore1.Release();
            }

        }

        public async Task<bool> ContainsAsync(string username)
        {

            try
            {
                await _semaphore2.WaitAsync();

                _users = await InitiateUsersAsync();

                bool con = _users.Any(user => user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

                return con;
            }
            finally
            {
                _semaphore2.Release();
            }

        }

        public async Task<int> GetMaxUserID()
        {

            try
            {

                _users = await InitiateUsersAsync();
                await _semaphore2.WaitAsync();
                return _users.Count > 0 ? _users.Max(u => u.UserID) + 1 : 1;
            }
            finally
            {
                _semaphore2.Release();
            }

        }

        public async Task AddAsync(User user)
        {

            try
            {
                await _semaphore2.WaitAsync();
                _users = await InitiateUsersAsync();


                _users.Add(user);


                if (!_users.Contains(user))
                {
                    throw new Exception("Could not create the user");
                }


            }
            finally
            {
                _semaphore2.Release();
            }
        }

        public async Task SaveChangesAsync()
        {

            try
            {
                await _semaphore2.WaitAsync();
                string updatedUsers = JsonConvert.SerializeObject(_users.OrderBy(u => u.UserID));
                await _filehandler.WriteFileAsync(_userDataPath, updatedUsers);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _semaphore2.Release();
            }
        }

        public async Task<User> GetUserByNameAsync(string username)
        {
            _users = await InitiateUsersAsync();
            User? user = _users.FirstOrDefault(u => username == u.UserName);
            if (user == null)
            {
                throw new Exception("Could not found the user");
            }
            return user;
        }

        public async Task<User> GetUserByIDAsync(int userid)
        {
            _users = await InitiateUsersAsync();
            User? user = _users.FirstOrDefault(u => userid == u.UserID);
            if (user == null)
            {
                throw new Exception("Could not found the user");
            }
            return user;
        }

        public async Task UpdateAsync(User userToUpdate)
        {
            try
            {
                await _semaphore2.WaitAsync();
                _users = await InitiateUsersAsync();
                var existingUser = _users.SingleOrDefault(u => u.UserID == userToUpdate.UserID);
                if (existingUser == null)
                {
                    throw new Exception("User not found");
                }
                if (!string.IsNullOrEmpty(userToUpdate.UserName))
                {
                    if (await ContainsAsync(userToUpdate.UserName))
                    {
                        throw new Exception($"Username {userToUpdate.UserName} already exists");
                    }
                    existingUser.UserName = userToUpdate.UserName;
                }
                if (existingUser.ManagerID != 0)
                {
                    User manger = await GetUserByIDAsync(userToUpdate.UserID);
                    if (manger.DepartmentID != userToUpdate.DepartmentID)
                    {
                        throw new Exception($"manager {userToUpdate} is not in the same department ");
                    }
                    existingUser.ManagerID = userToUpdate.ManagerID;
                }


            }
            finally
            {
                _semaphore2.Release();
            }
        }

        public void BlockUnBlock(User user)
        {
            user.IsBlocked = !user.IsBlocked;
        }

        public async Task<List<UserDataDTO>> GetAllUsersAsync()
        {
            try
            {
                await _semaphore2.WaitAsync();
                _users = await InitiateUsersAsync();
                _departments = await _departmentRepository.GetAllDepartmentsModelAsync();

                var users = (from u in _users
                             join m in _users on u.ManagerID equals m.UserID into um
                             from m in um.DefaultIfEmpty()
                             join d in _departments on u.DepartmentID equals d.DeparmtentID
                             where u.IsDeleted == 0
                             select new UserDataDTO
                             {
                                 Username = u.UserName,
                                 ManagerName = m != null ? m.UserName : "No Manager",
                                 Role = (Library.DTO.Role)u.UserRole,
                                 DepartmentName = d.DepartmentName
                             }).ToList();
                List<UserDataDTO> result = new List<UserDataDTO>();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphore2.Release();
            }

        }

        
    }

}




