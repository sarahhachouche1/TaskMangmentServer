
namespace ServerSide.Models;

public class TaskToDo
{
    public int TaskID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AssignedUserName { get; set; }
}
public enum Status
{
    Pending,
    Incomplete,
    Completed,
    Canceled,
    Rejected,
    Accepted,
}
public enum Priority
{
    Low,
    Medium,
    High,
    Urgent
}


