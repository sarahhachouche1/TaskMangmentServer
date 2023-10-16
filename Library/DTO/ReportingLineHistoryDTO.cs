using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DTO;

public class ReportingLineHistoryDTO
{
    public string Managername { get; set; }
    public string Subordinate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
