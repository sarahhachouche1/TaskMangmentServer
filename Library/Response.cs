using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Response
    {
         public string RequestId { get; set; }
         public bool IsSuccess { get; set; }  
         public int StatusCode { get; set; }  
         public string ErrorMessage { get; set; }

         public object? Content {  get; set; } 
    }
}
