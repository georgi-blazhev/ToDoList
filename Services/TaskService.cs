using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Entities;
using ToDoList.Exceptions;

namespace ToDoList.Services
{
    class TaskService
    {
        private const string StoreFileName = "Tasks.json";
        private readonly FileDatabase<Entities.Task> _storage;
        private readonly List<Entities.Task> tasks;
        private readonly UserService userService;
        private readonly ToDoService toDoService;

        public TaskService()
        {
            _storage = new FileDatabase<Entities.Task>();

            tasks = _storage.Read<List<Entities.Task>>(StoreFileName);

            if (tasks == null)
            {
                tasks = new List<Entities.Task>();
            }
            userService = UserService.GetInstance();
            toDoService = new ToDoService();
        }

        public void CreateTask(int toDoId, string title, string description, bool isComplete)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated and to check if this title already exists
                throw new Exceptions.UnauthorizedAccessException("Cannot create a list without logging in!");
            }

            Entities.Task task = new Entities.Task()
            {
                Id = tasks.Count + 1,
                ToDoId = toDoId,
                Title = title,
                Description = description,
                IsComplete = isComplete,
                CreatedAt = DateTime.Now,
                LastChange = DateTime.Now,
                LastChangeByUserId = userService.CurrentUser.Id
            };

            tasks.Add(task);
            SaveToFile();
        }

        public void DeleteTask(int id)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated and to check if this title already exists
                throw new Exceptions.UnauthorizedAccessException("Cannot create a list without logging in!");
            }

            if (userService.CurrentUser.Id != toDoService.GetTodo(id).CreatorId)
            {
                throw new Exceptions.UnauthorizedAccessException("Cannot access this ToDo");
            }

            foreach (var task in tasks)
            {
                if (task.Id == id)
                {
                    tasks.Remove(task);
                    SaveToFile();
                }
            }

            // TODO: to create an exception for nonexistent tasks
            throw new NonExistentToDoException("This task doesn't exist!");
        }

        public void EditTask(int id, string title, string description, bool isComplete)
        {
            if (userService.CurrentUser == null)
            {
                //TODO: Exception for unauthenticated and to check if this title already exists
                throw new Exceptions.UnauthorizedAccessException("Cannot create a list without logging in!");
            }

            if (userService.CurrentUser.Id != toDoService.GetTodo(id).CreatorId)
            {
                throw new Exceptions.UnauthorizedAccessException("Cannot access this ToDo");
            }

            foreach (var task in tasks)
            {
                if (task.Id == id)
                {
                    task.Title = title;
                    task.Description = description;
                    task.IsComplete = isComplete;
                    task.LastChange = DateTime.Now;
                    task.LastChangeByUserId = userService.CurrentUser.Id;
                    SaveToFile();
                }
            }
            // TODO: to create an exception for nonexistent tasks
            throw new NonExistentToDoException("This task doesn't exist!");
        }


        private void SaveToFile()
        {
            _storage.Write(StoreFileName, tasks);
        }
    }
}
