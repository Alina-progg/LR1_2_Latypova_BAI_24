using System;
using System.Windows;
using System.Windows.Controls;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.ViewModel;

namespace ЛР1_2_Латыпова_БАИ_24.View
{
    public partial class WindowNewEmployee : Window
    {
        // конструктор получает список должностей
        public WindowNewEmployee(RoleViewModel roleVM)
        {
            InitializeComponent();

            // привязка списка должностей к ComboBox
            CbRole.ItemsSource = roleVM.ListRole;
        }

        // сохранение данных
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        // отмена изменений
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}