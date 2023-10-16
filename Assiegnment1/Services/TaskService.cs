

using Library.DTO;
using ServerSide.Models;
using ServerSide.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSide.Services;

public interface ITaskService
{
    public Task CreateTask(TaskDTO taskDto);
    public Task CompleteTaskStatus(int taskId);
    public Task CancelTaskStatus(int taskId);
    public Task ChangeTaskStatus(UpdateStatusDTO taskdto);
    public Task<List<TaskDataDTO>> GetAllUserTasks(string username);
    public Task<Dictionary<string, List<TaskDataDTO>>> GetSubordinateTasksAsync(string managerUsername);
    public Task<List<TaskDataDTO>> GetSubordinateTasksbyPriorityAsync(string managerUsername, Library.DTO.Priority priority);
    public Task<List<TaskDataDTO>> GetSubordinateTasksbyStatusAsync(string managerUsername, Library.DTO.Status status);
    public Task<List<TaskDataDTO>> GetSubordinateTasksbyStartDateAsync(string managerUsername, DateTime startdate);
    public Task<List<TaskDataDTO>> GetSubordinateTasksbyEndDateAsync(string managerUsername, DateTime enddate);
    public Task<List<TaskDataDTO>> GetSubordinateTasksbyTitleAsync(string managerUsername, string title);


}
public class TaskService : ITaskService
{

    private readonly ITaskToDoRepository _taskRepository;
    private readonly IReportingLineRepository _reportinglineRepository;
    public TaskService(ITaskToDoRepository taskRepository, IReportingLineRepository reportinglineRepository)
    {
        _taskRepository = taskRepository;
        _reportinglineRepository = reportinglineRepository;
    }

    //[Manager]
    //[Employee]
    public async Task CreateTask(TaskDTO taskDto)
    {
        try
        {
            // Validation
            if (string.IsNullOrEmpty(taskDto.Title))
            {
                throw new ArgumentException("Title cannot be null or empty.");
            }

            if (!Enum.IsDefined(typeof(Models.Status), taskDto.Status))
            {
                throw new ArgumentException("Invalid status value.");
            }

            if (!Enum.IsDefined(typeof(Models.Priority), taskDto.Priority))
            {
                throw new ArgumentException("Invalid priority value.");
            }

            int taskId = await _taskRepository.GetMaxIdAsync();

            TaskToDo taskTodo = new TaskToDo
            {
                TaskID = taskId,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = (Models.Status)taskDto.Status,
                StartDate = DateTime.Now,
                Priority = (Models.Priority)taskDto.Priority,
                AssignedUserName = taskDto.AssignedUserName,
            };

            await _taskRepository.AddAsync(taskTodo);
            await _taskRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    //[Employee]
    public async Task CompleteTaskStatus(int taskId)
    {
        try {

             TaskToDo task = await _taskRepository.GetTaskById(taskId);
             task.Status = Models.Status.Completed;
             task.EndDate = DateTime.Now;
             await _taskRepository.SaveChangesAsync();
        }
        catch (Exception) { throw; }   
    }

    //[Employee]
    public async Task CancelTaskStatus(int taskId)
    {
        try {
            TaskToDo task = await _taskRepository.GetTaskById(taskId);
            task.Status = Models.Status.Canceled;
            await _taskRepository.SaveChangesAsync();
        }
        catch(Exception) {throw; } 
    }

   // [Manager]
    public async Task ChangeTaskStatus(UpdateStatusDTO taskdto)
    {
        try
        {
            TaskToDo task = await _taskRepository.GetTaskById(taskdto.TaskID);
            task.Status = (Models.Status)taskdto.Status;
            await _taskRepository.SaveChangesAsync();
        }
        catch (Exception) { throw; }
    }

    //[Emploee]
    public async Task<List<TaskDataDTO>> GetAllUserTasks(string username)
    {
        try
        {
            List<TaskToDo> tasks = await _taskRepository.GetAllAsync(username);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(tasks);
            return result;
        }
        catch { throw; }
    }


    //[Manager]
    public async Task<Dictionary<string, List<TaskDataDTO>>> GetSubordinateTasksAsync(string managerUsername)
    {
        try
        {
            List<ReportingLineHistory> subordinatesReportingLine = await _reportinglineRepository.GetSubordinateAsync(managerUsername);

            List<string> subordinates = MapReportingLineModelsToString(subordinatesReportingLine);

            Dictionary<string, List<TaskDataDTO>> subordinateTasks = new Dictionary<string, List<TaskDataDTO>>();

            foreach (string subordinate in subordinates)
            {
                List<TaskDataDTO> taskTodo = await GetAllUserTasks(subordinate);
                subordinateTasks.Add(subordinate, taskTodo);
                
            }

            return subordinateTasks;
        }
        catch{ throw; }
    }

    //[Manager]

    public async Task<List<TaskDataDTO>> GetSubordinateTasksbyPriorityAsync(string managerUsername, Library.DTO.Priority priority)
    {
        try
        {
            Dictionary<string, List<TaskDataDTO>> _taskTodo = await GetSubordinateTasksAsync(managerUsername);
            var list = _taskTodo.Values.SelectMany(t => t).ToList();
            Models.Priority modelPriority = (Models.Priority)priority;
            List<TaskToDo> taskToDo = await _taskRepository.GetAllTaskWithPriority(modelPriority);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(taskToDo);
            return result;
        }
        catch { throw; }
    }

    //[Manager]
    public async Task<List<TaskDataDTO>> GetSubordinateTasksbyStatusAsync(string managerUsername, Library.DTO.Status status)
    {
        try
        {
            Dictionary<string, List<TaskDataDTO>> _taskTodo = await GetSubordinateTasksAsync(managerUsername);
            var list = _taskTodo.Values.SelectMany(t => t).ToList();
            Models.Status modelStatus = (Models.Status)status;
            List<TaskToDo> taskToDo = await _taskRepository.GetAllTaskWithStatus(modelStatus);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(taskToDo);
            return result;
        }
        catch { throw; }
    }

    //[Manager]
    public async Task<List<TaskDataDTO>> GetSubordinateTasksbyStartDateAsync(string managerUsername, DateTime startdate)
    {
        try
        {
            Dictionary<string, List<TaskDataDTO>> _taskTodo = await GetSubordinateTasksAsync(managerUsername);
            var list = _taskTodo.Values.SelectMany(t => t).ToList();
            List<TaskToDo> taskToDo = await _taskRepository.GetAllTaskWithStartDate(startdate);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(taskToDo);
            return result;
        }
        catch { throw; }
    }

    //[Manager]
    public async Task<List<TaskDataDTO>> GetSubordinateTasksbyEndDateAsync(string managerUsername, DateTime enddate)
    {
        try
        {
            Dictionary<string, List<TaskDataDTO>> _taskTodo = await GetSubordinateTasksAsync(managerUsername);
            var list = _taskTodo.Values.SelectMany(t => t).ToList();
            List<TaskToDo> taskToDo = await _taskRepository.GetAllTaskWithEndDate(enddate);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(taskToDo);
            return result;
        }
        catch { throw; }
    }

    //[Manager]
    public async Task<List<TaskDataDTO>> GetSubordinateTasksbyTitleAsync(string managerUsername, string title)
    {
        try
        {
            Dictionary<string, List<TaskDataDTO>> _taskTodo = await GetSubordinateTasksAsync(managerUsername);
            var list = _taskTodo.Values.SelectMany(t => t).ToList();
            List<TaskToDo> taskToDo = await _taskRepository.GetAllTaskWithTitle(title);
            List<TaskDataDTO> result = MaptaskModelTotaskDto(taskToDo);
            return result;
        }
        catch { throw; }
    }

    private List<string> MapReportingLineModelsToString(List<ReportingLineHistory> reportingLinesHistory)
    {
        List<string> result = new List<string>();
        foreach(var reportingLine in reportingLinesHistory)
        {
            result.Add(reportingLine.Subordinate);
        }
        return result;
    }

    private List<TaskDataDTO> MaptaskModelTotaskDto(List<TaskToDo> tasks)
    {
        List<TaskDataDTO> result = new List<TaskDataDTO>();
        foreach (var task in tasks)
        {
            result.Add(new TaskDataDTO
            {
                Title = task.Title,
                Description = task.Description,
                Priority = (Library.DTO.Priority)task.Priority,
                Status = (Library.DTO.Status)task.Status,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
            });
        }
        return result;
    }
}

