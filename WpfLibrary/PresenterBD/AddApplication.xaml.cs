using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CAT_COMPANY;

namespace WpfLibrary.PresenterBD
{
    /// <summary>
    /// Логика взаимодействия для AddApplication.xaml
    /// </summary>
    public partial class AddApplication : Window
    {
        public class SelectedIngredient
        {
            public IngredientBd.Ingredient Ingredient { get; set; }
            public double? Quantity { get; set; }
        }

        public ApplicationBd.Application application;
        private IngredientBd ingredientBd;
        public List<SelectedIngredient> selectedIngredients;

        public AddApplication(ApplicationBd.Application ap, string bd)
        {
            ingredientBd = new IngredientBd(bd);
            application = ap;
            selectedIngredients = new List<SelectedIngredient>();
            InitializeComponent();
            IngredientNameComboBox.ItemsSource = ingredientBd.IngredientList.Select(i => new SelectedIngredient { Ingredient = i }).ToList();
        }

        private void AddApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            string departmentName = DepartmentNameTextBox.Text;
            selectedIngredients = ((List<SelectedIngredient>)IngredientNameComboBox.ItemsSource).Where(i => i.Quantity.HasValue).ToList();
            if (!string.IsNullOrEmpty(departmentName) && DatePicker.SelectedDate.HasValue)
            {
                ApplicationBd.Application newApplication = new ApplicationBd.Application
                {
                    ApplicationID = application.ApplicationID,
                    DepartmentName = departmentName,
                    Date = DatePicker.SelectedDate.Value
                };

                application = newApplication;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
