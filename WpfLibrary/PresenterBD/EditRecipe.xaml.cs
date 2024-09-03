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
        private IngredientBd ingredientBd;
        private RecipeBd recipeBd;
        public string newDescription;

        public EditRecipe(RecipeIngredientBd.RecipeIngredient spi, string bd)
        {
            ingredientBd = new IngredientBd(bd);
            recipeBd = new RecipeBd(bd);
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
                int recipeIndex = recipeBd.RecipeList.FindIndex(r => r.RecipeID == recipeIngredient.RecipeID);
                DescriptionTextBox.Text = recipeBd.RecipeList[recipeIndex].Description;
                int ingredientIndex = ingredientBd.IngredientList.FindIndex(i => i.IngredientID == recipeIngredient.IngredientID);
                IngredientLabel.Content = "Ингредиент " + ingredientBd.IngredientList[ingredientIndex].IngredientName;
            }
        }

        private void EditRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            double netWeight;
            double grossWeight;
            newDescription = DescriptionTextBox.Text;
            if (double.TryParse(NetWeightTextBox.Text, out netWeight) && double.TryParse(GrossWeightTextBox.Text, out grossWeight) && !string.IsNullOrEmpty(newDescription))
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
