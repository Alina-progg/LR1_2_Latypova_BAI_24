using Newtonsoft.Json; // добавлено для работы с json
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO; // добавлено для работы с файлами
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml;
using ЛР1_2_Латыпова_БАИ_24.Helper;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.View;

namespace ЛР1_2_Латыпова_БАИ_24.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        // путь к файлу данных
        readonly string path = @"C:\Users\User\source\repos\ЛР1_2_Латыпова_БАИ_24\DataModels\RoleData.json";

        // поле для хранения json строки
        string _jsonRoles = String.Empty;
        // поле для хранения текста ошибки
        public string Error { get; set; }

        private Role selectedRole;

        public Role SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged("SelectedRole");
                EditRole.CanExecute(true);
            }
        }

        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();

        public RoleViewModel()
        {
            ListRole = LoadRole(); // загрузка данных из файла вместо ручного заполнения
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
            }
            return max;
        }

        private RelayCommand addRole;

        public RelayCommand AddRole
        {
            get
            {
                return addRole ??
                (addRole = new RelayCommand(obj =>
                {
                    WindowNewRole wnRole = new WindowNewRole
                    {
                        Title = "Новая должность",
                    };
                    int maxIdRole = MaxId() + 1;
                    Role role = new Role { Id = maxIdRole };
                    wnRole.DataContext = role;
                    if (wnRole.ShowDialog() == true)
                    {
                        ListRole.Add(role);
                        SaveChanges(ListRole); // сохранение изменений в файл
                    }
                    SelectedRole = role;
                }));
            }
        }

        private RelayCommand editRole;

        public RelayCommand EditRole
        {
            get
            {
                return editRole ??
                (editRole = new RelayCommand(obj =>
                {
                    WindowNewRole wnRole = new WindowNewRole
                    {
                        Title = "Редактирование должности",
                    };
                    Role role = SelectedRole;
                    Role tempRole = new Role();
                    tempRole = role.ShallowCopy();
                    wnRole.DataContext = tempRole;
                    if (wnRole.ShowDialog() == true)
                    {
                        role.NameRole = tempRole.NameRole;
                        SaveChanges(ListRole); // сохранение изменений в файл
                    }
                },
                (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        private RelayCommand deleteRole;

        public RelayCommand DeleteRole
        {
            get
            {
                return deleteRole ??
                (deleteRole = new RelayCommand(obj =>
                {
                    Role role = SelectedRole;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по должности: " +
                        role.NameRole, "Предупреждение", MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListRole.Remove(role);
                        SaveChanges(ListRole); // сохранение изменений в файл
                    }
                },
                (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        #region Methods
        // метод загрузки данных из json файла
        public ObservableCollection<Role> LoadRole()
        {
            _jsonRoles = File.ReadAllText(path);
            if (_jsonRoles != null)
            {
                ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                return ListRole;
            }
            else
            {
                return null;
            }
        }

        // метод сохранения данных в json файл
        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole, Newtonsoft.Json.Formatting.Indented);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}