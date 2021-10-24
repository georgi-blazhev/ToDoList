using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Entities;
using ToDoList.Exceptions;
using ToDoList.Services;

namespace ToDoList.Services
{
    public class ToDoService
    {
        private const string StoreFileName = "ToDos.json";
        private readonly FileDatabase<ToDo> _storage;
        private readonly UserService userService;
        private readonly List<ToDo> toDos;

        public ToDoService()
        {
            _storage = new FileDatabase<ToDo>();

            toDos = _storage.Read<List<ToDo>>(StoreFileName);

            if (toDos ==  null)
            {
                toDos = new List<ToDo>();
            }

            userService = UserService.GetInstance();
        }

        public void CreateToDo(string title)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated and to check if this title already exists
                throw new Exceptions.UnauthorizedAccessException("Cannot create a list without logging in!");
            }

            ToDo toDo = new ToDo() {
                Id = toDos.Count + 1,
                Title = title, 
                CreatorId = userService.CurrentUser.Id, 
                CreatedAt = DateTime.Now, 
                LastChange = DateTime.Now, 
                LastChangeByUserId = 
                userService.CurrentUser.Id
           };
            toDos.Add(toDo);
            SaveToFile();
        }

        public void DeleteToDo(int id)
        {

            List<User> allUsers = userService.ReadUsers();

            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated
                throw new Exceptions.UnauthorizedAccessException("Cannot delete a list without logging in!");
            }

            foreach (var list in toDos)
            {
                if (list.Id == id)
                {
                    toDos.Remove(list);
                    foreach (var user in allUsers)
                    {
                        if (user.SharedToDos.Contains(id))
                        {
                            user.SharedToDos.Remove(id);
                        }
                    }
                    SaveToFile();
                }
            }
            throw new AlreadyExistingToDoException("This ToDo list already exists!");
        }

        public void EditToDoInfo(int id, string title)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated
                throw new Exceptions.UnauthorizedAccessException("Cannot delete a list without logging in!");
            }

            foreach (var list in toDos)
            {
                if (list.Id == id)
                {
                    list.Title = title;
                    list.LastChange = DateTime.Now;
                    list.LastChangeByUserId = userService.CurrentUser.Id;
                    SaveToFile();
                }
            }
            throw new NonExistentToDoException("This ToDo list does not exist!");
        }

        public void ShareToDo(int Id, int UserId)
        {
            User user = userService.ReadSingleUser(UserId);

            if (user != null)
            {
                try
                {
                    user.AddToShared(Id);
                }
                catch (AlreadyExistingToDoException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            throw new NonExistentUserException($"User with Id:{Id} not found!");
        }

        public ToDo GetTodo(int id)
        {
            foreach (var toDo in toDos)
            {
                if (toDo.Id == id)
                {
                    return toDo;
                }
            }
            throw new NonExistentToDoException($"ToDo with {id} doesn't exist");
        }

        public ToDo GetTodo(string title)
        {
            foreach (var toDo in toDos)
            {
                if (toDo.Title == title)
                {
                    return toDo;
                }
            }
            throw new NonExistentToDoException($"ToDo with {title} doesn't exist");
        }
        
        private void SaveToFile()
        {
            _storage.Write(StoreFileName, toDos);
        }

        public List<ToDo> GetAll()
        {
            List<ToDo> allAndSharedToDos = new List<ToDo>();

            User current = userService.CurrentUser;

            if (current.UserRole == Role.Admin)
            {
                return toDos;
            }

            foreach (var toDo in toDos)
            {
                if (current.Id == toDo.CreatorId || current.SharedToDos.Contains(toDo.Id))
                {
                    allAndSharedToDos.Add(toDo);
                }
            }
            return allAndSharedToDos;
        }
    }
}
