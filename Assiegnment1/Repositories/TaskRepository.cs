using Library;
using Library.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServerSide.Configurations;
using ServerSide.Models;
using ServerSide.Specifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSide.Repositories;

public interface ITaskToDoRepository : IGenericRepository<TaskToDo>
{
    public Task<int> GetMaxIdAsync();
    public Task<TaskToDo> GetTaskById(int taskId);
    public Task<List<TaskToDo>> GetAllAsync(string username);
    public Task<List<TaskToDo>> GetAllTaskWithPriority(Models.Priority priority);
    public Task<List<TaskToDo>> GetAllTaskWithStatus(Models.Status status);
    public Task<List<TaskToDo>> GetAllTaskWithTitle(string title);
    public Task<List<TaskToDo>> GetAllTaskWithStartDate(DateTime startdate);
    public Task<List<TaskToDo>> GetAllTaskWithEndDate(DateTime enddate);
}
public class TaskToDoRepository : ITaskToDoRepository
{
    private ConcurrentDictionary<string, List<TaskToDo>> _taskTodo;
    private readonly IFileHandler _filehandler;
    private static readonly object lockob = new object();
    private static TaskToDoRepository? instance = null;
    private static SemaphoreSlim _semaphore5 = new SemaphoreSlim(1, 1);
    private static SemaphoreSlim _semaphore6 = new SemaphoreSlim(1, 1);
    private readonly IConfiguration config;
    private readonly string _tasksDataPath;

    public static TaskToDoRepository Instance
    {
        get
        {
            lock (lockob)
            {
                if (instance == null)
                {
                    instance = new TaskToDoRepository(new FileHandler());
                }
                return instance;
            }
        }

    }

    private TaskToDoRepository(IFileHandler filehandler)
    {
        _filehandler = filehandler;
        config = AppConfiguration.Configuration();
        _tasksDataPath = config["AppSettings:Tasks"];
    }

    internal async Task<ConcurrentDictionary<string, List<TaskToDo>>> InitiateTasksToDoAsync()
    {
        try
        {
            await _semaphore5.WaitAsync();
            if (_taskTodo == null)
            {
                string taskDataJson = await _filehandler.ReadFileAsync(_tasksDataPath);
                Dictionary<string, List<TaskToDo>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<TaskToDo>>>(taskDataJson) ?? new Dictionary<string, List<TaskToDo>>();
                _taskTodo = new ConcurrentDictionary<string, List<TaskToDo>>(dictionary);
            }
            return _taskTodo;
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

    public async Task AddAsync(TaskToDo taskTodo)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            if (_taskTodo.ContainsKey(taskTodo.AssignedUserName))
            {
                _taskTodo[taskTodo.AssignedUserName].Add(taskTodo);
            }
            else
            {
                _taskTodo.TryAdd(taskTodo.AssignedUserName, new List<TaskToDo> { taskTodo });
            }
        }
        finally
        {
            _semaphore6.Release();
        }
    }

    public async Task<int> GetMaxIdAsync()
    {
        int maxId = 0;

        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            if (_taskTodo.Any())
            {
                maxId = _taskTodo.SelectMany(pair => pair.Value).Max(task => task.TaskID) + 1;
            }
        }
        finally
        {
            _semaphore6.Release();
        }

        return maxId;
    }

    public async Task<TaskToDo> GetTaskById(int taskId)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            foreach (var pair in _taskTodo)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (pair.Value[i].TaskID == taskId)
                    {
                        return pair.Value[i];
                    }
                }
            }
        }
        finally
        {
            _semaphore6.Release();
        }

        throw new KeyNotFoundException($"No task with ID {taskId} was found.");
    }

    public async Task<List<TaskToDo>> GetAllAsync(string username)
    {
        try
        {
            await _semaphore6.WaitAsync();
            _taskTodo = await InitiateTasksToDoAsync();

            if (_taskTodo.ContainsKey(username))
            {
                return _taskTodo[username];
            }
        }
        finally
        {
            _semaphore6.Release();
        }

        throw new KeyNotFoundException($"No tasks for user {username} were found.");
    }

    public async Task SaveChangesAsync()
    {

        try
        {
            await _semaphore6.WaitAsync();
            string updatedTasks = JsonConvert.SerializeObject(_taskTodo);
            await _filehandler.WriteFileAsync(_tasksDataPath, updatedTasks);
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

    public async Task<List<TaskToDo>> GetAllTaskWithPriority(Models.Priority priority)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            var specification = new TaskPrioritySpecification();
            var list = _taskTodo.Values.SelectMany(t => t).ToList();

            return list.Where(t => specification.IsSatisfiedBy(priority, t)).ToList();

        }
        catch
        {
            throw;
        }

    }

    public async Task<List<TaskToDo>> GetAllTaskWithStatus(Models.Status status)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            var specification = new TaskStatusSpecification();
            var list = _taskTodo.Values.SelectMany(t => t).ToList();

            return list.Where(t => specification.IsSatisfiedBy(status, t)).ToList();

        }
        catch { throw; }    

    }

    public async Task<List<TaskToDo>> GetAllTaskWithTitle(string title)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            var specification = new TaskTitleSpecification();
            var list = _taskTodo.Values.SelectMany(t => t).ToList();

            return list.Where(t => specification.IsSatisfiedBy(title, t)).ToList();

        }
        catch { throw; }

    }

    public async Task<List<TaskToDo>> GetAllTaskWithStartDate(DateTime startdate)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            var specification = new TaskStartDateSpecification();
            var list = _taskTodo.Values.SelectMany(t => t).ToList();

            return list.Where(t => specification.IsSatisfiedBy(startdate, t)).ToList();

        }
        catch { throw; }

    }

    public async Task<List<TaskToDo>> GetAllTaskWithEndDate(DateTime enddate)
    {
        try
        {
            await _semaphore6.WaitAsync();

            _taskTodo = await InitiateTasksToDoAsync();

            var specification = new TaskEndDateSpecification();
            var list = _taskTodo.Values.SelectMany(t => t).ToList();

            return list.Where(t => specification.IsSatisfiedBy(enddate, t)).ToList();

        }
        catch { throw; }

    }
}



