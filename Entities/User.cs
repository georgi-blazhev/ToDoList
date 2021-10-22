using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Exceptions;

namespace ToDoList.Entities
{
    public class User : AuditedEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role UserRole { get; set; }
        public int CreatorId { get; set; }
        public List<int> SharedToDos { get; }

        public void AddToShared(int id)
        {
            if (SharedToDos.Contains(id))
            {
                throw new AlreadyExistingToDoException("This ToDo is already shared!");
            }
            SharedToDos.Add(id);
        }

        public void RemoveFromShared(int id)
        {
            if (SharedToDos.Contains(id))
            {
                SharedToDos.Remove(id);
            }
            throw new NonExistentToDoException("ToDo not found!");
        }


    }


    
}
