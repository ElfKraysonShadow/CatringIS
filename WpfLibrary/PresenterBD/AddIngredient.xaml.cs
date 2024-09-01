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
    /// Логика взаимодействия для AddIngredient.xaml
    /// </summary>
    public partial class AddIngredient : Window
    {
        public IngredientBd.Ingredient ingredient;
        private ProviderBd providerBd;

        public AddIngredient(IngredientBd.Ingredient ing, string bd)
        {
            providerBd = new ProviderBd(bd);
            ingredient = ing;
            InitializeComponent();
            ProviderNameComboBox.ItemsSource = providerBd.ProviderList;
            ProviderNameComboBox.DisplayMemberPath = "ProviderName";
            DisplayIngredientDetails();
        }

        private void DisplayIngredientDetails()
        {
            if (ingredient != null)
            {
                IngredientNameTextBox.Text = ingredient.IngredientName;
                UnitOfMeasureTextBox.Text = ingredient.UnitOfMeasure;
                MarkupTextBox.Text = ingredient.Markup.ToString();
                StockRemainTextBox.Text = ingredient.StockRemain.ToString();
                ProviderNameComboBox.SelectedItem = providerBd.ProviderList.FirstOrDefault(p => p.ProviderName == ingredient.ProviderName);
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            string ingredientName = IngredientNameTextBox.Text;
            string unitOfMeasure = UnitOfMeasureTextBox.Text;
            double markup;
            double stockRemain;
            string providerName = (ProviderNameComboBox.SelectedItem as ProviderBd.Provider)?.ProviderName;

            if (!string.IsNullOrEmpty(ingredientName) &&
                !string.IsNullOrEmpty(unitOfMeasure) &&
                double.TryParse(MarkupTextBox.Text, out markup) &&
                double.TryParse(StockRemainTextBox.Text, out stockRemain) &&
                !string.IsNullOrEmpty(providerName))
            {
                IngredientBd.Ingredient newIngredient = new IngredientBd.Ingredient
                {
                    IngredientID = ingredient.IngredientID,
                    IngredientName = ingredientName,
                    UnitOfMeasure = unitOfMeasure,
                    Markup = markup,
                    StockRemain = stockRemain,
                    ProviderName = providerName,
                };

                ingredient = newIngredient;
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
