using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Entities;

namespace ToDoList.Services
{
    class TaskService
    {
        private const string StoreFileName = "Tasks.json";
        private readonly FileDatabase<Entities.Task> _storage;
        private readonly List<Entities.Task> tasks;
        private readonly UserService userService;

        public TaskService()
        {
            _storage = new FileDatabase<Entities.Task>();

            tasks = _storage.Read<List<Entities.Task>>(StoreFileName);

            if (tasks == null)
            {
                tasks = new List<Entities.Task>();
            }
            userService = UserService.GetInstance();
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


        private void SaveToFile()
        {
            _storage.Write(StoreFileName, tasks);
        }
    }
}
