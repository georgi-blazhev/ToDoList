using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Exceptions
{
    class NonExistentToDoException : Exception
    {
        public NonExistentToDoException(string message) : base (message)
        {
        }
    }
}
