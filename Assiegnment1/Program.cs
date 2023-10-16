using Library;
using Library.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServerSide.Configurations;
using ServerSide.Models;
using ServerSide.Repositories;
using ServerSide.Services;

public class Program
{
    private static IConfiguration config;
    private static string _RequestsFolderPath;

    static void Main(string[] args)
    {
        config = AppConfiguration.Configuration();
        _RequestsFolderPath = config["AppSettings:Requests"];
        RequestBuilder start = new RequestBuilder();

        start.SetUpFileWatcher(_RequestsFolderPath);
        Console.ReadLine();
    }
}

/*
TaskToDoRepository taskrepo = TaskToDoRepository.Instance;
ReportingLineRepository repo = ReportingLineRepository.Instance;
TaskService batata = new TaskService(taskrepo, repo);
try
{
    var homos = await batata.GetSubordinateTasksAsync("Afef");
    foreach (var hom in homos)
        Console.WriteLine(hom);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

/*
static async Task Main(string[] args)
{
try
{
var watcher = new FileWatcher("C:\\Root\\Requests");
watcher.OnFileChanged += Batata;
}catch(Exception ex)
{
Console.WriteLine(ex.ToString());
}
}

/*static async Task Batata(string path, string request)
{
Console.WriteLine($"Path: {path}");
Console.WriteLine($"Request: {request}");
// Your logic here...
}*//*
UserAuthenticationDTO dto = new UserAuthenticationDTO()
{
    UserName = "sara",
    Password ="1234"
    
};
Request r =  new Request()
{
    Uri = "AuthenticationService/Login",
    Content = new Dictionary<string, object> { { "userDto", dto } }

};
string request = JsonConvert.SerializeObject(r);
RequestBuilder requestbuilder = new RequestBuilder();
requestbuilder.HandleRequest(request);
var watcher = RequestBuilder.SetUpFileWatcher("C:\\Root\\Requests", requestbuilder.HandleRequest);
Console.ReadLine(); */
/*
UserRegesrationDTO user = new UserRegesrationDTO
{
    UserName = "jad",
    Password = "batata",
    UserRole = Library.DTO.Role.TeamMember,
    DepartmentID = 1
};
UserRepository repo = UserRepository.Instance;
UserService userService = new UserService(repo);
await userService.CreateUser(user);

/*
TaskDTO task = new TaskDTO
{
    Title = "Assigenment",
    Description = "Finish serverside",
    Status = Library.DTO.Status.Pending,
    Priority = Library.DTO.Priority.High,
    AssignedUserName = "Rami"

};

TaskToDoRepository taskrepository = TaskToDoRepository.Instance;
TaskService taskservice = new TaskService(taskrepository);
await taskservice.CreateTask(task);*/

/*User user1 = new User
{
    UserID = 1,
    UserName = "fatima",
    Password = "shu fe",
    UserRole = ServerSide.Models.Role.TeamMember

};*/

/*
DepartmentDTO department = new DepartmentDTO
{
    DepartmentName = "IT"
};
ReportingLineRepository report = ReportingLineRepository.Instance;
UserRepository userRepository = UserRepository.Instance;
ReportingLineService batata = new ReportingLineService(report, userRepository);
List<ReportingLineHistoryDTO> dd = (await batata.GetManagerReportingLineHistory("Admmin")).ToList();
foreach(var x in dd)
{
    Console.WriteLine(x.Subordinate );
}

/*DepartmentRepository departmentRepo = DepartmentRepository.Instance;
DepartmentService batata = new DepartmentService(departmentRepo);*/
//await batata.CreateDepartment(department);
//UserRepository userRepo = UserRepository.Instance;
//UserService batata = new UserService(userRepo);
//await batata.CreateUser(user);
/*uthenticationService batata = new AuthenticationService(userRepo);
string dd = await batata.ChangePasswordAsync("Afef" , "marhabe");
Console.WriteLine(dd);  
/*await batata.CreateUser(user);*//*
var nateje = await batata.ListUsers();
Console.WriteLine(nateje);
foreach(var x in nateje)
{
    Console.WriteLine(x);
    Console.WriteLine(x.ManagerName + " " + x.Username + " " + x.Role + " " + x.DepartmentName);
}*/
/*
/*List<User> users= new List<User>();
UserRepository userRepo = UserRepository.Instance;
userRepo.AddAsync(user);
userRepo.AddAsync(user1);
userRepo.SaveChangesAsync();
Console.WriteLine(await userRepo.GetMaxUserID());
Console.WriteLine(await userRepo.ContainsAsync(user.UserName));
*/