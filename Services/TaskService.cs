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

        public void EditTask(int id, string title, string description)
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
                    task.IsComplete = false;
                    task.LastChange = DateTime.Now;
                    task.LastChangeByUserId = userService.CurrentUser.Id;
                    SaveToFile();
                }
            }
            // TODO: to create an exception for nonexistent tasks
            throw new NonExistentToDoException("This task doesn't exist!");
        }

        public void AssignTask(int userId, int toDoId, int taskId)
        {
            ToDo toDo = toDoService.GetTodo(toDoId);
            User user = userService.ReadSingleUser(userId);
            if (toDo.CreatorId == userId || user.SharedToDos.Contains(toDoId))
            {
                toDo.AssignedTasks.Add(taskId);
            }
            throw new Exceptions.UnauthorizedAccessException("User not creator or doesn't have the ToDo shared!");
        }

        public List<Entities.Task> GetTasks()
        {
            List<Entities.Task> allCreatedAndSharedTasks = new List<Entities.Task>(); 

            foreach (var task in tasks)
            {
                User current = userService.CurrentUser;
                ToDo toDo = toDoService.GetTodo(task.ToDoId);
                if (CheckIfCreatedByOrAssigned(current, toDo, task))
                {
                    allCreatedAndSharedTasks.Add(task);
                }
            }
            return allCreatedAndSharedTasks;
        }

        public bool CheckIfCreatedByOrAssigned(User current, ToDo toDo, Entities.Task task)
        {
            return ((current.Id == toDo.CreatorId) || toDo.AssignedTasks.Contains(task.Id));
        }

        private Entities.Task GetTask(int id)
        {
            foreach (var task in tasks)
            {
                if (task.Id == id)
                {
                    return task;
                }
            }
            //TODO: TODO Exception for tasks
            throw new NonExistentToDoException("Task doesn't exist!");
        }

        public void CompleteTask(int id)
        {
            User current = userService.CurrentUser;
            Entities.Task task = GetTask(id);
            ToDo toDo = toDoService.GetTodo(task.ToDoId);
            if (CheckIfCreatedByOrAssigned(current, toDo, task))
            {
                task.IsComplete = !task.IsComplete;
            }
        }

        private void SaveToFile()
        {
            _storage.Write(StoreFileName, tasks);
        }
    }
}
