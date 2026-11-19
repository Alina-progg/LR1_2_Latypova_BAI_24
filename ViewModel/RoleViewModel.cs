
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ЛР1_2_Латыпова_БАИ_24.Helper;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.View;

namespace ЛР1_2_Латыпова_БАИ_24.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        // поле для хранения текста ошибки
        public string Error { get; set; }

        private Role selectedRole;

        // выбранная должность
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

        // список должностей
        public ObservableCollection<Role> ListRole { get; set; }
            = new ObservableCollection<Role>();

        // загрузка должностей из базы данных
        public RoleViewModel()
        {
            ListRole = LoadRole();
        }

        // получение максимального идентификатора
        public int MaxId()
        {
            int max = 0;

            foreach (var r in ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
            }

            return max;
        }

        private RelayCommand addRole;

        // добавление должности
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
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            db.Roles.Add(role);
                            db.SaveChanges();
                        }

                        ListRole.Add(role);
                        SelectedRole = role;
                    }
                }));
            }
        }

        private RelayCommand editRole;

        // редактирование должности
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
                    Role tempRole = role.ShallowCopy();

                    wnRole.DataContext = tempRole;

                    if (wnRole.ShowDialog() == true)
                    {
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            Role roleInDb = db.Roles.Find(role.Id);

                            if (roleInDb != null)
                            {
                                roleInDb.NameRole = tempRole.NameRole;
                                db.SaveChanges();
                            }
                        }

                        role.NameRole = tempRole.NameRole;
                    }
                },
                (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        private RelayCommand deleteRole;

        // удаление должности
        public RelayCommand DeleteRole
        {
            get
            {
                return deleteRole ??
                (deleteRole = new RelayCommand(obj =>
                {
                    Role role = SelectedRole;

                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по должности: " + role.NameRole,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            Role roleInDb = db.Roles.Find(role.Id);

                            if (roleInDb != null)
                            {
                                db.Roles.Remove(roleInDb);
                                db.SaveChanges();
                            }
                        }

                        ListRole.Remove(role);
                    }
                },
                (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        #region Methods

        // загрузка должностей из базы данных
        public ObservableCollection<Role> LoadRole()
        {
            using (CompanyEntities db = new CompanyEntities())
            {
                return new ObservableCollection<Role>(
                    db.Roles.ToList());
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
