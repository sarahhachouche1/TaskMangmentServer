namespace Library.DTO;

public class UserRegesrationDTO
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public Role UserRole { get; set; }
    public int DepartmentID { get; set; }
}
public enum Role
{

    Admin,
    Manager,
    TeamMember
}
