using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Data.Entity;
using ЛР1_2_Латыпова_БАИ_24.Helper;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.View;

namespace ЛР1_2_Латыпова_БАИ_24.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        // поле для хранения текста ошибки
        public string Error { get; set; }

        // поле для хранения сообщения
        public string Message { get; set; }

        // ссылка на RoleViewModel
        public RoleViewModel RoleVM { get; set; }

        // конструктор класса
        public PersonViewModel(RoleViewModel roleVM)
        {
            RoleVM = roleVM;
            ListPerson = LoadPerson();
        }

        private Person selectedPerson;

        // выбранный сотрудник
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged("SelectedPerson");
                EditPerson.CanExecute(true);
            }
        }

        // список сотрудников
        public ObservableCollection<Person> ListPerson { get; set; }
            = new ObservableCollection<Person>();

        // получение максимального идентификатора
        public int MaxId()
        {
            int max = 0;

            foreach (var person in ListPerson)
            {
                if (max < person.Id)
                {
                    max = person.Id;
                }
            }

            return max;
        }

        #region AddPerson

        private RelayCommand addPerson;

        // добавление сотрудника
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                (addPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee(RoleVM)
                    {
                        Title = "Новый сотрудник"
                    };

                    int maxIdPerson = MaxId() + 1;

                    Person person = new Person
                    {
                        Id = maxIdPerson,
                        Birthday = DateTime.Now
                    };

                    wnPerson.DataContext = person;

                    if (wnPerson.ShowDialog() == true)
                    {
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            // проверка выбранной должности
                            var role = db.Roles.Find(person.RoleId);

                            if (role != null)
                            {
                                db.Persons.Add(person);
                                db.SaveChanges();

                                // добавление сотрудника в список
                                person.Role = RoleVM.ListRole
                                    .FirstOrDefault(r => r.Id == person.RoleId);

                                ListPerson.Add(person);
                                SelectedPerson = person;
                            }
                            else
                            {
                                Message = "Необходимо выбрать должность сотрудника.";
                                MessageBox.Show(Message);
                            }
                        }
                    }
                },
                (obj) => true));
            }
        }

        #endregion

        #region EditPerson

        private RelayCommand editPerson;

        // редактирование сотрудника
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                (editPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee(RoleVM)
                    {
                        Title = "Редактирование данных сотрудника"
                    };

                    Person person = SelectedPerson;

                    // создание копии сотрудника для редактирования
                    Person tempPerson = new Person
                    {
                        Id = person.Id,
                        RoleId = person.RoleId,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Birthday = person.Birthday
                    };

                    wnPerson.DataContext = tempPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            // поиск сотрудника в базе данных
                            Person personInDb = db.Persons.Find(person.Id);

                            if (personInDb != null)
                            {
                                // обновление данных сотрудника
                                personInDb.RoleId = tempPerson.RoleId;
                                personInDb.FirstName = tempPerson.FirstName;
                                personInDb.LastName = tempPerson.LastName;
                                personInDb.Birthday = tempPerson.Birthday;

                                db.SaveChanges();
                            }
                        }

                        // обновление данных в списке
                        person.RoleId = tempPerson.RoleId;
                        person.FirstName = tempPerson.FirstName;
                        person.LastName = tempPerson.LastName;
                        person.Birthday = tempPerson.Birthday;
                        person.Role = RoleVM.ListRole
                            .FirstOrDefault(r => r.Id == person.RoleId);

                        // обновление отображения списка
                        int index = ListPerson.IndexOf(person);
                        ListPerson.Remove(person);
                        ListPerson.Insert(index, person);
                        SelectedPerson = person;
                    }
                },
                (obj) => SelectedPerson != null && ListPerson.Count > 0));
            }
        }

        #endregion

        #region DeletePerson

        private RelayCommand deletePerson;

        // удаление сотрудника
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                    Person person = SelectedPerson;

                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по сотруднику: \n" +
                        person.LastName + " " + person.FirstName,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        using (CompanyEntities db = new CompanyEntities())
                        {
                            // поиск сотрудника в базе данных
                            Person personInDb = db.Persons.Find(person.Id);

                            if (personInDb != null)
                            {
                                db.Persons.Remove(personInDb);
                                db.SaveChanges();
                            }
                        }

                        ListPerson.Remove(person);
                        SelectedPerson = null;
                    }
                },
                (obj) => SelectedPerson != null && ListPerson.Count > 0));
            }
        }

        #endregion

        #region Methods

        // загрузка сотрудников из базы данных
        public ObservableCollection<Person> LoadPerson()
        {
            using (CompanyEntities db = new CompanyEntities())
            {
                return new ObservableCollection<Person>(
                    db.Persons.Include("Role").ToList());
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