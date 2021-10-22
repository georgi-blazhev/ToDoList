using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Entities
{
    public abstract class AuditedEntity : Entity
    {
        public DateTime LastChange { get; set; }
        public int LastChangeByUserId { get; set; }
    }
}
