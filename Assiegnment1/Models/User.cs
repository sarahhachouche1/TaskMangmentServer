namespace ServerSide.Models;

public class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int ManagerID { get; set; } = 0;
    public bool IsBlocked { get; set; } = false;
    public Role UserRole { get; set; }
    public bool IsActive { get; set; } = false;
    public int DepartmentID { get; set; } = 0; 
    public int IsDeleted { get; set; } = 0;

}
public enum Role
{
    Admin,
    Manager,
    TeamMember
}
