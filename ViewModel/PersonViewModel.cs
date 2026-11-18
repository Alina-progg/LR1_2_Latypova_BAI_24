using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO; // добавлено для работы с файлами
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Newtonsoft.Json; // добавлено для работы с json
using ЛР1_2_Латыпова_БАИ_24.Helper;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.View;

namespace ЛР1_2_Латыпова_БАИ_24.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        // добавлено: путь к json файлу
        readonly string path = @"C:\Users\User\source\repos\ЛР1_2_Латыпова_БАИ_24\DataModels\PersonData.json";

        // добавлено: поля для работы с json и ошибками
        string _jsonPersons = String.Empty;
        public string Error { get; set; }
        public string Message { get; set; } // добавлено для вывода сообщений

        // Ссылка на RoleViewModel, чтобы передавать её в окно
        public RoleViewModel RoleVM { get; set; }

        // Конструктор принимает RoleViewModel (чтобы работал MainWindowViewModel)
        public PersonViewModel(RoleViewModel roleVM)
        {
            RoleVM = roleVM;

            // изменено: убрано ручное добавление данных, теперь загрузка из файла
            ListPerson = LoadPerson();
            ListPersonDpo = GetListPersonDpo();
        }

        private PersonDpo selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
                EditPerson.CanExecute(true);
            }
        }

        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; } = new ObservableCollection<PersonDpo>();
        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();

        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            if (ListPerson != null)
            {
                foreach (var person in ListPerson)
                {
                    PersonDpo p = new PersonDpo();
                    p = p.CopyFromPerson(person);
                    ListPersonDpo.Add(p);
                }
            }
            return ListPersonDpo;
        }

        public int MaxId()
        {
            int max = 0;
            if (ListPerson != null)
            {
                foreach (var r in this.ListPerson)
                {
                    if (max < r.Id) max = r.Id;
                }
            }
            return max;
        }

        #region AddPerson
        private RelayCommand addPerson;
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

                    // изменено: birthday теперь строка
                    PersonDpo per = new PersonDpo { Id = maxIdPerson, Birthday = DateTime.Now.ToString("dd.MM.yyyy") };

                    wnPerson.DataContext = per;

                    if (wnPerson.ShowDialog() == true)
                    {
                        // изменено: добавлена проверка на null для должности
                        var r = (Role)wnPerson.CbRole.SelectedItem;
                        if (r != null)
                        {
                            per.RoleName = r.NameRole;
                            // добавлено: форматирование даты в строку
                            per.Birthday = DateTime.Parse(per.Birthday).ToString("dd.MM.yyyy");

                            ListPersonDpo.Add(per);

                            Person p = new Person();
                            p = p.CopyFromPersonDPO(per);
                            ListPerson.Add(p);

                            // добавлено: сохранение изменений в файл
                            SaveChanges(ListPerson);
                        }
                    }
                },
                (obj) => true));
            }
        }
        #endregion

        #region EditPerson
        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                (editPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee(RoleVM)
                    {
                        Title = "Редактирование данных сотрудника",
                    };
                    PersonDpo personDpo = SelectedPersonDpo;
                    PersonDpo tempPerson = new PersonDpo();
                    tempPerson = personDpo.ShallowCopy();
                    wnPerson.DataContext = tempPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        // изменено: добавлена проверка на null
                        var r = (Role)wnPerson.CbRole.SelectedItem;
                        if (r != null)
                        {
                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            // изменено: форматирование даты
                            personDpo.Birthday = DateTime.Parse(tempPerson.Birthday).ToString("dd.MM.yyyy");

                            // изменено: поиск сотрудника и обновление через метод
                            var p = ListPerson.FirstOrDefault(x => x.Id == personDpo.Id);
                            if (p != null)
                            {
                                p = p.CopyFromPersonDPO(personDpo);
                            }

                            // добавлено: сохранение изменений в файл
                            SaveChanges(ListPerson);
                        }
                        else
                        {
                            // добавлено: сообщение если должность не выбрана
                            Message = "Необходимо выбрать должность сотрудника.";
                        }
                    }
                },
                (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }
        #endregion

        #region DeletePerson
        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                    PersonDpo person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
                        "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListPersonDpo.Remove(person);

                        // изменено: поиск удаляемого сотрудника через firstordefault
                        var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);
                        if (per != null)
                        {
                            ListPerson.Remove(per);
                            // добавлено: сохранение изменений в файл
                            SaveChanges(ListPerson);
                        }
                    }
                },
                (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }
        #endregion

        #region Methods
        // добавлено: метод загрузки из файла
        public ObservableCollection<Person> LoadPerson()
        {
            try
            {
                if (File.Exists(path))
                {
                    _jsonPersons = File.ReadAllText(path);
                    if (_jsonPersons != null)
                    {
                        return JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
                    }
                }
            }
            catch (Exception e)
            {
                Error = "Ошибка чтения json файла\n" + e.Message;
            }
            return new ObservableCollection<Person>();
        }

        // добавлено: метод сохранения в файл
        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            // добавлено: formatting.indented для красивого вывода
            var jsonPerson = JsonConvert.SerializeObject(listPersons, Newtonsoft.Json.Formatting.Indented);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonPerson);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла\n" + e.Message;
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