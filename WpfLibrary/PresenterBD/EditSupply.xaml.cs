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
    /// Логика взаимодействия для AddSupplyIngredient.xaml
    /// </summary>
    public partial class EditSupply : Window
    {
        public SupplyIngredientBd.SupplyIngredient supplyIngredient;

        public EditSupply(SupplyIngredientBd.SupplyIngredient spi)
        {
            InitializeComponent();
            supplyIngredient = spi;
            DisplaySupplyIngredientDetails();
        }
        private void DisplaySupplyIngredientDetails()
        {
            if (supplyIngredient != null)
            {
                QuantityTextBox.Text = Convert.ToString(supplyIngredient.Quantity);
                PurnashePriceTextBox.Text = Convert.ToString(supplyIngredient.PurnashePrice);
            }
        }

        private void EditSupplyButton_Click(object sender, RoutedEventArgs e)
        {
            double quantity;
            double purnashePrice;

            if (double.TryParse(QuantityTextBox.Text, out quantity) && double.TryParse(PurnashePriceTextBox.Text, out purnashePrice))
            {
                SupplyIngredientBd.SupplyIngredient newSupplyIngredient = new SupplyIngredientBd.SupplyIngredient
                {
                    SupplyIngID = supplyIngredient.SupplyIngID,
                    IngredientID = supplyIngredient.IngredientID,
                    SupplyID = supplyIngredient.SupplyID,
                    Quantity = quantity,
                    PurnashePrice = purnashePrice,
                };

                supplyIngredient = newSupplyIngredient;
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
