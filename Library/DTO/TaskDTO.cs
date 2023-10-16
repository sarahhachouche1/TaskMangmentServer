namespace Library.DTO;

public class TaskDTO
{ 
    public int TaskID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
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
