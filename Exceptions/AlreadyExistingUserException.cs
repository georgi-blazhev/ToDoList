using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Exceptions
{
    class AlreadyExistingUserException : Exception
    {
        public AlreadyExistingUserException(string message) : base(message)
        {
        }
    }
}
