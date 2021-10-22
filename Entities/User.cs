using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Entities
{
    class User : Entity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role UserRole { get; set; }
        public int CreatorId { get; set; }
        public DateTime LastChange { get; set; }
        public int LastChangeByUserId { get; set; }


    }


    
}
