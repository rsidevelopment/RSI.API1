using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDevelopment.Services.Models._ViewModels
{
    public class ListViewModel<t>
    {
        public List<t> Items { get; set; } = new List<t>();
        public string Message { get; set; } = "Not Implemented";
        public bool IsSuccess { get; set; } = false;
    }
}
