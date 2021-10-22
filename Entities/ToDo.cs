using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Entities
{
    public class ToDo : AuditedEntity
    {
        public string Title { get; set; }
        public int CreatorId { get; set; }
        
    }
}
