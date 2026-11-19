using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ЛР1_2_Латыпова_БАИ_24.ViewModel;

namespace ЛР1_2_Латыпова_БАИ_24.Model
{
    /// <summary>
    /// класс сотрудник
    /// </summary>
    public class Person
    {
        /// <summary>
        /// код сотрудника
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// код должности сотрудника
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// имя сотрудника
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// фамилия сотрудника
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// дата рождения сотрудника
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// должность сотрудника
        /// </summary>
        public virtual Role Role { get; set; }
    }
}