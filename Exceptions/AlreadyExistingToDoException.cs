using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Exceptions
{
    public class AlreadyExistingToDoException : Exception
    {
        public AlreadyExistingToDoException(string message) : base (message)
        {
        }
    }
}
