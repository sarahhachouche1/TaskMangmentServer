using Library;
using Library.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServerSide.Configurations;
using ServerSide.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace ServerSide.Repositories;

public interface IReportingLineRepository : IGenericRepository<ReportingLineHistory>
{
    public Task DeleteAsync(string managerName, string employeeName);
    public Task<List<ReportingLineHistory>> GetSubordinateAsync(string managerName);

}
public class ReportingLineRepository : IReportingLineRepository
{
    private ConcurrentDictionary<string, List<ReportingLineHistory>> _reportingLine;
    private readonly IFileHandler _filehandler;
    private static readonly object lockob = new object();
    private static ReportingLineRepository? instance = null;
    private static SemaphoreSlim _semaphore5 = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim _semaphore6 = new SemaphoreSlim(1, 1);
    private readonly IConfiguration config;
    private readonly string _reportingLineDataPath;

    public static ReportingLineRepository Instance
    {
        get
        {
            lock (lockob)
            {
                if (instance == null)
                {
                    instance = new ReportingLineRepository(new FileHandler());
                }
                return instance;
            }
        }

    }

    private ReportingLineRepository(IFileHandler filehandler)
    {
        _filehandler = filehandler;
        config = AppConfiguration.Configuration();
        _reportingLineDataPath = config["AppSettings:ReportingLines"];
    }

    internal async Task<ConcurrentDictionary<string, List<ReportingLineHistory>>> InitiateReportingLinesAsync()
    {
        try
        {
            await _semaphore5.WaitAsync();
            if (_reportingLine == null)
            {
                string reportingDataJson = await _filehandler.ReadFileAsync(_reportingLineDataPath);
                Dictionary<string, List<ReportingLineHistory>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<ReportingLineHistory>>>(reportingDataJson) ?? new Dictionary<string, List<ReportingLineHistory>>();
                _reportingLine = new ConcurrentDictionary<string, List<ReportingLineHistory>>(dictionary);
            }
            return _reportingLine;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _semaphore5.Release();
        }
    }

    public async Task AddAsync(ReportingLineHistory reportingLine)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _reportingLine = await InitiateReportingLinesAsync();

            if (_reportingLine.ContainsKey(reportingLine.Managername))
            {
                _reportingLine[reportingLine.Managername].Add(reportingLine);
            }
            else
            {
                _reportingLine.TryAdd(reportingLine.Managername, new List<ReportingLineHistory> { reportingLine });
            }
        }
        finally
        {
            _semaphore6.Release();
        }
    }

    public async Task DeleteAsync (string managerName , string employeeName)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _reportingLine = await InitiateReportingLinesAsync();

            if (_reportingLine.ContainsKey(managerName))
            {
                var reportLine = _reportingLine[managerName].FirstOrDefault(u => u.Subordinate == employeeName);

                if (reportLine != null)
                {
                    reportLine.EndDate = DateTime.Now;
                }
                else
                {
                    throw new Exception($"User {employeeName} not found in {managerName}'s reporting line.");
                }
            }
            else
            {
                throw new Exception($"Manager {managerName} not found.");
            }
        }
        finally
        {
            _semaphore6.Release();
        }
    }

    public async Task<List<ReportingLineHistory>> GetSubordinateAsync(string managerName)
    {
        await _semaphore6.WaitAsync();

        _reportingLine = await InitiateReportingLinesAsync();

        if (_reportingLine.ContainsKey(managerName))
        {
            return _reportingLine[managerName];
           
        }
        throw new Exception($"{managerName} is not manager ");
    }

    public async Task SaveChangesAsync()
    {

        try
        {
            await _semaphore6.WaitAsync();
            string updatedDepartment = JsonConvert.SerializeObject(_reportingLine);
            await _filehandler.WriteFileAsync(_reportingLineDataPath, updatedDepartment);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _semaphore6.Release();
        }
    }
}



