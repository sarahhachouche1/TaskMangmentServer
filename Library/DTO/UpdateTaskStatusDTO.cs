using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DTO;

public class UpdateStatusDTO
{
    public int TaskID { get; set; }
    public Status Status { get; set; }
    public string CreatedBy { get; set; }
}
