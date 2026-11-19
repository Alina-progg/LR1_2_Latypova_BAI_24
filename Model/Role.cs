using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ЛР1_2_Латыпова_БАИ_24.Model
{
    /// <summary>
    /// класс должность сотрудника
    /// </summary>
    public class Role : INotifyPropertyChanged
    {
        /// <summary>
        /// код должности
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// наименование должности
        /// </summary>
        private string nameRole;

        public string NameRole
        {
            get { return nameRole; }
            set
            {
                nameRole = value;
                OnPropertyChanged("NameRole");
            }
        }

        public Role()
        {
            // создаем коллекцию сотрудников
            Persons = new HashSet<Person>();
        }

        public Role(int id, string nameRole)
        {
            Id = id;
            NameRole = nameRole;
            Persons = new HashSet<Person>();
        }

        /// <summary>
        /// коллекция сотрудников с этой должностью
        /// </summary>
        public virtual ICollection<Person> Persons { get; set; }

        /// <summary>
        /// поверхностное копирование объекта
        /// </summary>
        public Role ShallowCopy()
        {
            return (Role)this.MemberwiseClone();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}