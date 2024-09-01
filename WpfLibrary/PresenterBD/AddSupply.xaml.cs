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
using static CAT_COMPANY.BankBd;

namespace WpfLibrary.PresenterBD
{
    /// <summary>
    /// Логика взаимодействия для AddSupply.xaml
    /// </summary>
    public partial class AddSupply : Window
    {
        public class SelectedSupplyIngredient
        {
            public IngredientBd.Ingredient Ingredient { get; set; }
            public double? Quantity { get; set; }
            public double? PurnashePrice { get; set; }
        }

        public SupplyBd.Supply supply;
        private IngredientBd ingredientBd;
        private ProviderBd providerBd; 
        public List<SelectedSupplyIngredient> selectedIngredients;

        public AddSupply(SupplyBd.Supply sp, string bd)
        {
            ingredientBd = new IngredientBd(bd);
            providerBd = new ProviderBd(bd);
            supply = sp;
            selectedIngredients = new List<SelectedSupplyIngredient>();
            InitializeComponent();
            ProviderNameComboBox.ItemsSource = providerBd.ProviderList;
            ProviderNameComboBox.DisplayMemberPath = "ProviderName";
            IngredientNameComboBox.ItemsSource = ingredientBd.IngredientList.Select(i => new SelectedSupplyIngredient { Ingredient = i }).ToList();
        }
        private void AddSupplyButton_Click(object sender, RoutedEventArgs e)
        {
            string providerName = (ProviderNameComboBox.SelectedItem as ProviderBd.Provider)?.ProviderName; ;
            selectedIngredients = ((List<SelectedSupplyIngredient>)IngredientNameComboBox.ItemsSource).Where(i => i.Quantity.HasValue && i.PurnashePrice.HasValue).ToList();
            if (!string.IsNullOrEmpty(providerName) && DatePicker.SelectedDate.HasValue && selectedIngredients.Count != 0)
            {
                SupplyBd.Supply newSupply = new SupplyBd.Supply
                {
                    SupplyID = supply.SupplyID,
                    ProviderName = providerName,
                    Date = DatePicker.SelectedDate.Value
                };

                supply = newSupply;
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
