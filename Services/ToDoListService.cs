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
    public class ToDoListService
    {
        private const string StoreFileName = "ToDos.json";
        private readonly FileDatabase<ToDo> _storage;
        private readonly UserService userService;
        private readonly List<ToDo> toDos;

        public ToDoListService()
        {
            _storage = new FileDatabase<ToDo>();

            toDos = _storage.Read<List<ToDo>>(StoreFileName);

            if (toDos ==  null)
            {
                toDos = new List<ToDo>();
            }

            userService = new UserService();
        }

        public void CreateToDo(string title)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated and to check if this title already exists
                throw new Exceptions.UnauthorizedAccessException("Cannot create a list without logging in!");
            }

            ToDo list = new ToDo() {
                Id = toDos.Count + 1,
                Title = title, 
                CreatorId = userService.CurrentUser.Id, 
                CreatedAt = DateTime.Now, 
                LastChange = DateTime.Now, 
                LastChangeByUserId = 
                userService.CurrentUser.Id
           };
            toDos.Add(list);
            SaveToFile();
        }

        public void DeleteToDo(int id)
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
                    toDos.Remove(list);
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
            
        private void SaveToFile()
        {
            _storage.Write(StoreFileName, toDos);
        }

    }
}
