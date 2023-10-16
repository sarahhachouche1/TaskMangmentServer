using Library;
using Library.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServerSide.Configurations;
using ServerSide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSide.Repositories;

public interface IDepartmentRepository : IGenericRepository<Department>
{
    public Task<int> GetMaxDepartmentID();
    public Task<bool?> ContainsAsync(string departmentname);
    public Task<List<DepartmentDTO>> GetAllDepartmentsAsync();
    public Task<List<Department>> GetAllDepartmentsModelAsync();

}
public class DepartmentRepository : IDepartmentRepository
{
    private List<Department>? _departments;
    private readonly IFileHandler _filehandler;
    private static readonly object lockob = new object();
    private static DepartmentRepository? instance = null;
    private static SemaphoreSlim _semaphore3 = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim _semaphore4 = new SemaphoreSlim(1, 1);
    private readonly IConfiguration config;
    private readonly string _departmentDataPath;

    public static DepartmentRepository Instance
    {

        get
        {
            lock (lockob)
            {
                if (instance == null)
                {
                    instance = new DepartmentRepository(new FileHandler());
                }
                return instance;
            }
        }

    }

    private DepartmentRepository(IFileHandler filehandler)
    {
        _filehandler = filehandler;
        config = AppConfiguration.Configuration();
        _departmentDataPath = config["AppSettings:Departments"];
    }

    internal async Task<List<Department>> InitiateDepartmentsAsync()
    {

        try
        {
            await _semaphore3.WaitAsync();
            if (_departments == null)
            {
                string departmentDataJson = await _filehandler.ReadFileAsync(_departmentDataPath);
                _departments = JsonConvert.DeserializeObject<List<Department>>(departmentDataJson) ?? new List<Department>();
                    
                  
                
            }
            return _departments;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            _semaphore3.Release();
        }

    }

    public async Task AddAsync(Department department)
    {
        try
        {
            await _semaphore4.WaitAsync();
            _departments = await InitiateDepartmentsAsync();

            _departments.Add(department);


            if (!_departments.Contains(department))
            {
                throw new Exception("Could not create the department");
            }


        }
        finally
        {
            _semaphore4.Release();
        }
    }

    public async Task<int> GetMaxDepartmentID()
    {
        try
        {
            await _semaphore4.WaitAsync();
            _departments = await InitiateDepartmentsAsync();
            if (_departments != null && _departments.Any())
            {
                return _departments.Max(d => d.DeparmtentID) + 1;
            }
            else
            {
                return 1;
            }
        }
        finally
        {
            _semaphore4.Release();
        }

    }

    public async Task<bool?> ContainsAsync(string departmentname)
    {

        try
        {
            await _semaphore4.WaitAsync();

            _departments = await InitiateDepartmentsAsync();

            bool? con = _departments?.Any(department => department.DepartmentName.Equals(departmentname, StringComparison.OrdinalIgnoreCase));

            return con;
        }
        finally
        {
            _semaphore4.Release();
        }

    }

    public async Task SaveChangesAsync()
    {

        try
        {
            await _semaphore4.WaitAsync();
            string updatedDepartment = JsonConvert.SerializeObject(_departments.OrderBy(d => d.DeparmtentID));
            await _filehandler.WriteFileAsync(_departmentDataPath, updatedDepartment);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            _semaphore4.Release();
        }
    }

    public async Task<List<DepartmentDTO>> GetAllDepartmentsAsync()
    {
        try
        {
            await _semaphore4.WaitAsync();
            _departments = await InitiateDepartmentsAsync();

            List<DepartmentDTO> departmentDTO = new List<DepartmentDTO>();  
            foreach(Department department in _departments)
            {
                departmentDTO.Add(new DepartmentDTO
                {
                    DepartmentName = department.DepartmentName,
                });
            }

            return departmentDTO;
        }
        catch (Exception)
        {
            throw;
        }
        finally { _semaphore4.Release(); }
    }

    public async Task<List<Department>> GetAllDepartmentsModelAsync()
    {
        try
        {
            await _semaphore4.WaitAsync();
            _departments = await InitiateDepartmentsAsync();
            return _departments;
        }
        catch (Exception) { throw; }
    }
}
