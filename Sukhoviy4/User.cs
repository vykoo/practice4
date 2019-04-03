using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sukhoviy4.Exceptions;

namespace Sukhoviy4
{ 
    [Serializable]
    public class User
    {
        private const string DataFilepath = "persondata";
        private const string UserFileTemplate = "person{0}.bin";
        private static readonly string[] ChinaZodiacs = { "Мавпа", "Півень", "Собака", "Свиня", "Миша", "Бик", "Тигр", "Кролик", "Дракон", "Змія", "Кінь", "Коза" };
        private static readonly string[] WesternZodiacs = { "Водолій", "Риби", "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва", "Терези", "Скорпіон", "Стрілець", "Козеріг" };

        public string Name { get; private set; }

        public string Surname { get; private set; }

        public string Email { get; private set; }

        public DateTime Birthday { get; private set; }

        public User(string name, string surname, string email, DateTime birthday)
        {
            if (name.Length < 2)
            {
                throw new WrongNameException($"Невірне ім'я: {name}");
            }

            if (surname.Length < 3)
            {
                throw new WrongSurnameException($"Невірне прізвище: {surname}!");
            }

            ValidateEmail(email);

            if (DateTime.Today < birthday || CountAge(birthday) > 135)
            {
                throw new WrongDateException(birthday);
            }

            Name = name;
            Surname = surname;
            Email = email;
            Birthday = birthday;
        }

        private int CountAge(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            if (birthday > today.AddYears(-age)) age--;
            return age;
        }

        private void ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (!match.Success)
                throw new WrongEmailException(email);
        }
        public bool IsAdult => CountAge(Birthday) >= 18;
        public bool IsBirthday => DateTime.Today.DayOfYear == Birthday.DayOfYear;
        public User(string name, string surname, string email) : this(name, surname, email, DateTime.Today) { }
        public User(string name, string surname, DateTime birthday) : this(name, surname, "not specified", birthday) { }
        public string ChineseSign => ChinaZodiacs[Birthday.Year % 12];

        public string SunSign
        {
            get
            {
                var day = Birthday.Day;
                var month = Birthday.Month;
                switch (month)
                {
                    case 1:
                    case 4:
                    {
                        return day >= 20 ? WesternZodiacs[month - 1] : (month == 1 ?
                            WesternZodiacs[WesternZodiacs.Length - 1] : WesternZodiacs[month - 2]);
                    }
                    case 2:
                        return day >= 19 ? WesternZodiacs[month - 1] : WesternZodiacs[month - 2];
                    case 3:
                    case 5:
                    case 6:
                    {
                        return day >= 21 ? WesternZodiacs[month - 1] : WesternZodiacs[month - 2];
                    }
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    {
                        return day >= 23 ? WesternZodiacs[month - 1] : WesternZodiacs[month - 2];
                    }
                    default:
                        return day >= 22 ? WesternZodiacs[month - 1] : WesternZodiacs[month - 2];
                }

            }
        }
        public void CopyUserFrom(User user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Email = user.Email;
            Birthday = user.Birthday;
        }

        private void SaveUsersTo(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException());
                Stream stream = new FileStream(path: filename,
                    mode: FileMode.Create,
                    access: FileAccess.Write,
                    share: FileShare.None);
                formatter.Serialize(serializationStream: stream, graph: this);
                stream.Close();
            }
            catch (IOException)
            {
            }
        }

        private static User LoadFrom(string filename)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
                var user = (User)formatter.Deserialize(stream);
                stream.Close();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async void LoadAllInto(List<User> users, Action action)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(DataFilepath))
                {
                    Directory.CreateDirectory(DataFilepath);
                    users.AddRange(UsersCreator.CreateUsers(50));
                    SaveData(users);
                }
                else
                {
                    users.AddRange(Directory.EnumerateFiles(DataFilepath).Select(LoadFrom));
                }
                action();
            });
        }

        public static void SaveData(List<User> users)
        {
            var i = 0;
            users.ForEach(delegate (User p)
            {
                p.SaveUsersTo(Path.Combine(DataFilepath, string.Format(UserFileTemplate, i++)));
            });
            string extraFile;
            while (File.Exists(extraFile = Path.Combine(DataFilepath, string.Format(UserFileTemplate, i++))))
                File.Delete(extraFile);
        }

        private static class UsersCreator
        {
            public static List<User> CreateUsers(int count)
            {
                var users = new List<User>();
                for (var i = 0; i < count; ++i)
                {
                    users.Add(new User("Username"+i, "Usersurname"+i, $"useremail{i}@ok.com", new DateTime(1998,10,10).AddMonths(i)));
                }

                return users;
            }

        }
    }
}

