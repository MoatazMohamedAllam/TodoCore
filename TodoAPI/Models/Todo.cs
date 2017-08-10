using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string TodoItem { get; set; }
        public Boolean IsDone { get; set; }
        public string AddedBy { get; set; }
        public DateTime Date { get; set; }
    }
}
