using Library.DTO;
using ServerSide.Models;
using ServerSide.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Services;

public interface IReportingLineService
{
    public Task<string> assignUserToManager(string employeename, string managername);
    public Task<string> DeleteReportingLineAsync(string employeename, string manaername);
    public Task<string> EditReportingLineAsync(string employeeName, string newManager);
    public Task<List<ReportingLineHistoryDTO>> GetManagerReportingLineHistory(string managerName);
}
public class ReportingLineService : IReportingLineService
{

    private readonly IReportingLineRepository _reportingLineRepository;
    private readonly IUserRepository _userRepository;

    public ReportingLineService(IReportingLineRepository reportingRepository, IUserRepository userRepository)
    {
        _reportingLineRepository = reportingRepository;
        _userRepository = userRepository;
    }

    //[Admin]
    public async Task<string> assignUserToManager(string employeename, string managername)
    {
        try
        {
            User user = await _userRepository.GetUserByNameAsync(employeename);
            User manager = await _userRepository.GetUserByNameAsync(managername);

            if (manager.DepartmentID != user.DepartmentID) throw new Exception($"{managername} and {employeename} are not in same department");
            if (user.ManagerID != 0) throw new Exception($"{employeename} already has a manager");

            user.ManagerID = manager.UserID;
            await _userRepository.SaveChangesAsync();

            ReportingLineHistory log = new ReportingLineHistory
            {
                Managername = managername,
                Subordinate = employeename,
                StartDate = DateTime.Now,
            };

            await _reportingLineRepository.AddAsync(log);
            await _reportingLineRepository.SaveChangesAsync();

            return "Assigned successfully";
        }
        catch (Exception)
        {
            throw;
        }
    }

    //[Admin]
    public async Task<string> DeleteReportingLineAsync(string employeename, string manaername)
    {
        try
        {
            User user = await _userRepository.GetUserByNameAsync(employeename);
            user.ManagerID = 0;
            await _userRepository.SaveChangesAsync();

            await _reportingLineRepository.DeleteAsync(manaername, employeename);
            await _reportingLineRepository.SaveChangesAsync();
            return "reporting line deleted";
        }
        catch (Exception) { throw; }
    }

    //[Admin]
    public async Task<string> EditReportingLineAsync(string employeeName, string newManager)
    {
        try
        {
            User employee = await _userRepository.GetUserByNameAsync(employeeName);
            User manager = await _userRepository.GetUserByNameAsync(newManager);
            User oldManager = await _userRepository.GetUserByIDAsync(employee.ManagerID);

            if (employee.DepartmentID != manager.DepartmentID) throw new Exception($"{newManager} and {employeeName} are not in same department")
            {

            };
            await DeleteReportingLineAsync(employeeName, oldManager.UserName);
            await assignUserToManager(employeeName, newManager);

            await _reportingLineRepository.SaveChangesAsync();
            return "manager successfully updated";

        }
        catch (Exception) { throw; }
    }

    //[Admin]
    public async Task<List<ReportingLineHistoryDTO>> GetManagerReportingLineHistory(string managerName)
    {
        List<ReportingLineHistory> reportingLines = await _reportingLineRepository.GetSubordinateAsync(managerName);
        
        return MapReportinagLineModelToDTO(reportingLines);
    }
    
    
    private List<ReportingLineHistoryDTO> MapReportinagLineModelToDTO(List<ReportingLineHistory> reportingLines)
    {
        List<ReportingLineHistoryDTO> reportingLinesDto = new List<ReportingLineHistoryDTO>();
        foreach (var reportingLine in reportingLines)
        {

            reportingLinesDto.Add(new ReportingLineHistoryDTO
            {
                Managername = reportingLine.Managername,
                Subordinate = reportingLine.Subordinate,
                StartDate = reportingLine.StartDate,
                EndDate = reportingLine.EndDate,
            });
        }
        return reportingLinesDto;
    }

}


