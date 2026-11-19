using System;
using System.Data.Entity;
using System.Linq;
using ЛР1_2_Латыпова_БАИ_24.Model;

namespace ЛР1_2_Латыпова_БАИ_24
{
    public class CompanyEntities : DbContext
    {
        // конструктор контекста базы данных
        public CompanyEntities()
            : base("name=CompanyEntities")
        {
        }

        // набор данных о должностях
        public virtual DbSet<Role> Roles { get; set; }

        // набор данных о сотрудниках
        public virtual DbSet<Person> Persons { get; set; }
    }
}