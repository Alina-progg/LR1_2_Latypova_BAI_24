using System;
using System.Windows;
using System.Windows.Controls;
using ЛР1_2_Латыпова_БАИ_24.Model;
using ЛР1_2_Латыпова_БАИ_24.ViewModel;

namespace ЛР1_2_Латыпова_БАИ_24.View
{
    /// <summary>
    /// Логика взаимодействия для WindowNewEmployee.xaml
    /// </summary>
    public partial class WindowNewEmployee : Window
    {
        // Конструктор принимает RoleViewModel, чтобы взять оттуда актуальный ListRole
        public WindowNewEmployee(RoleViewModel roleVM)
        {
            InitializeComponent();

            // Привязываем ComboBox к общему списку должностей
            CbRole.ItemsSource = roleVM.ListRole;

            Loaded += (s, e) =>
            {
                PersonDpo p = DataContext as PersonDpo;
                if (p == null) return;
                foreach (Role r in CbRole.Items)
                {
                    if (r.NameRole == p.RoleName)
                    {
                        CbRole.SelectedItem = r;
                        break;
                    }
                }
            };
        }

        // Обработчик для переключения видимости TextBox и DatePicker
        private void tbBirthday_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (tbBirthday.Visibility == Visibility.Hidden)
            {
                ClBirthday.Visibility = Visibility.Visible;
            }
            else
            {
                ClBirthday.Visibility = Visibility.Hidden;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}