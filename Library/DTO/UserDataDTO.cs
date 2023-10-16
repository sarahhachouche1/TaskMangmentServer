using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DTO
{
    public class UserDataDTO
    {
        public string Username { get; set; }
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
        public Role Role { get; set; }


    }
}
