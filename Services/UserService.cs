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
    public class UserService
    {
        private static readonly UserService _userService = new UserService();

        private const string StoreFileName = "Users.json";
        private readonly FileDatabase<User> _storage;

        private readonly List<User> _applicationUsers = new List<User>();

        private UserService()
        {
            _storage = new FileDatabase<User>();
            List<User> usersFromFile = _storage.Read<List<User>>(StoreFileName);
            _applicationUsers = usersFromFile;
        }

        public User CurrentUser { get; private set; }

        private void SaveToFile()
        {
            _storage.Write(StoreFileName, _applicationUsers);
        }

        public void Register(string Username, string Password, string FirstName, string LastName)
        {
            if (CurrentUser.UserRole == Role.RegularUser)
            {
                throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");
            }
            foreach (var user  in _applicationUsers)
            {
                if (user.Username == Username)
                {
                    throw new AlreadyExistingUserException("User already exists!");
                }
            }

            int newUserId = _applicationUsers.Count + 1;
            DateTime now = DateTime.Now;

            _applicationUsers.Add(new User()
            {
                Id = newUserId,
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                CreatedAt = now,
                UserRole = Role.RegularUser,
                LastChange = now,
                LastChangeByUserId = CurrentUser.Id
            });

            SaveToFile();
        }

        public void LogIn(string Username, string Password)
        {
            if (CurrentUser.UserRole == Role.RegularUser)
            {
                throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");
            }
            foreach (var user in _applicationUsers)
            {
                if (user.Username == Username && user.Password == Password)
                {
                    CurrentUser = user;
                    Console.WriteLine("Sucessfully logged in!");
                }    
            }
            throw new NonExistentUserException("Invalid username or password.");
        }

        public void DeleteUser(int UserId)
        {
            if (CurrentUser.UserRole == Role.RegularUser)
            {
                throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");
            }
            foreach (var user in _applicationUsers)
            {
                if (user.Id == UserId && CurrentUser.Id != UserId)
                {
                    _applicationUsers.Remove(user);
                    SaveToFile();
                }
            }
            throw new NonExistentUserException($"User with Id: {UserId} not found");
        }

        public void EditUserInfo(int UserId, string Username, string Password, string FirstName, string LastName)
        {
            if (CurrentUser.UserRole == Role.RegularUser)
            {
                throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");

            }
            foreach (var user in _applicationUsers)
            {
                if (user.Id == UserId)
                {
                    user.Username = Username;
                    user.Password = Password;
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.LastChange = DateTime.Now;
                    user.LastChangeByUserId = CurrentUser.Id;
                }
            }
            throw new NonExistentUserException($"User with Id: {UserId} not found");
        }

        public List<User> ReadUsers()
        {
            if (CurrentUser.UserRole == Role.Admin)
            {
                return _applicationUsers;
            }
            throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");
        }

        public User ReadSingleUser(int userId)
        {
            if (CurrentUser.UserRole == Role.Admin)
            {
                foreach (var user in _applicationUsers)
                {
                    if (user.Id == userId)
                    {
                        return user;
                    }
                }
            }
            throw new Exceptions.UnauthorizedAccessException($"Restricted access for {Role.RegularUser}");
        }

        public void LogOut()
        {
            CurrentUser = null;
        }

        public static UserService GetInstance()
        {
            return _userService;
        }
    }
}
