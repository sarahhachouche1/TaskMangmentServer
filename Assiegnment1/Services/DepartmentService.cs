using Library.DTO;
using ServerSide.Models;
using ServerSide.Repositories;

namespace ServerSide.Services;

public interface IDepartmentService
{
    public Task CreateDepartment(DepartmentDTO departmentDto);

    public Task<List<DepartmentDTO>> GetAllDepartments();

    public Task ChangeUserDepartment(string username, int departmentid);
}
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;

    public DepartmentService(IDepartmentRepository departmentRepository, IUserRepository userRepository)
    {
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
    }

    //[Admin]
    public async Task CreateDepartment(DepartmentDTO departmentDto)
    {
        try
        {
            bool? containDepartment = await _departmentRepository.ContainsAsync(departmentDto.DepartmentName);
            if (containDepartment == true)
            {
                throw new Exception($"department {departmentDto.DepartmentName} already exists");
            }
            int departmentId = await _departmentRepository.GetMaxDepartmentID();

            Department department = new Department
            {
                DeparmtentID = departmentId,
                DepartmentName = departmentDto.DepartmentName,
            };
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    //[Admin]
    public async Task<List<DepartmentDTO>> GetAllDepartments()
    {
        try
        {
            List<DepartmentDTO> departments = await _departmentRepository.GetAllDepartmentsAsync();
            return departments;
        }
        catch (Exception) { throw; }
    }

    //[Admin]
    public async Task ChangeUserDepartment(string username, int departmentid)
    {
        User user = await _userRepository.GetUserByNameAsync(username);
        if (user.ManagerID != 0)
        {
            throw new Exception("Manager and Subordinate most be in same department");
        }
        user.DepartmentID = departmentid;

        await _userRepository.SaveChangesAsync();
    }

}