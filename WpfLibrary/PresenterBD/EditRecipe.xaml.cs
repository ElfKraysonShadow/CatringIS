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
    /// Логика взаимодействия для AddRecipeIngredient.xaml
    /// </summary>
    public partial class EditRecipe : Window
    {
        public RecipeIngredientBd.RecipeIngredient recipeIngredient;

        public EditRecipe(RecipeIngredientBd.RecipeIngredient spi)
        {
            InitializeComponent();
            recipeIngredient = spi;
            DisplayRecipeIngredientDetails();
        }
        private void DisplayRecipeIngredientDetails()
        {
            if (recipeIngredient != null)
            {
                NetWeightTextBox.Text = Convert.ToString(recipeIngredient.NetWeight);
                GrossWeightTextBox.Text = Convert.ToString(recipeIngredient.GrossWeight);
            }
        }

        private void EditRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            double netWeight;
            double grossWeight;

            if (double.TryParse(NetWeightTextBox.Text, out netWeight) && double.TryParse(GrossWeightTextBox.Text, out grossWeight))
            {
                RecipeIngredientBd.RecipeIngredient newRecipeIngredient = new RecipeIngredientBd.RecipeIngredient
                {
                    RecipeIngID = recipeIngredient.RecipeIngID,
                    IngredientID = recipeIngredient.IngredientID,
                    RecipeID = recipeIngredient.RecipeID,
                    NetWeight = netWeight,
                    GrossWeight = grossWeight,
                };

                recipeIngredient = newRecipeIngredient;
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
