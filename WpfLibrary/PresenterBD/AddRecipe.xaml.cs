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
    /// Логика взаимодействия для AddRecipe.xaml
    /// </summary>
    public partial class AddRecipe : Window
    {
        public class SelectedRecipeIngredient
        {
            public IngredientBd.Ingredient Ingredient { get; set; }
            public double? GrossWeight { get; set; }
            public double? NetWeight { get; set; }
        }

        public RecipeBd.Recipe recipe;
        private IngredientBd ingredientBd;
        private FoodBd foodBd;
        public List<SelectedRecipeIngredient> selectedIngredients;

        public AddRecipe(RecipeBd.Recipe sp, string bd)
        {
            ingredientBd = new IngredientBd(bd);
            foodBd = new FoodBd(bd);
            recipe = sp;
            selectedIngredients = new List<SelectedRecipeIngredient>();
            InitializeComponent();
            FoodNameComboBox.ItemsSource = foodBd.FoodList;
            FoodNameComboBox.DisplayMemberPath = "FoodName";
            IngredientNameComboBox.ItemsSource = ingredientBd.IngredientList.Select(i => new SelectedRecipeIngredient { Ingredient = i }).ToList();
        }
        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = (FoodNameComboBox.SelectedItem as FoodBd.Food)?.FoodName;
            string description = DescriptionTextBox.Text;
            selectedIngredients = ((List<SelectedRecipeIngredient>)IngredientNameComboBox.ItemsSource).Where(i => i.GrossWeight.HasValue && i.NetWeight.HasValue).ToList();
            if (!string.IsNullOrEmpty(foodName) && !string.IsNullOrEmpty(description) && selectedIngredients.Count != 0)
            {
                RecipeBd.Recipe newRecipe = new RecipeBd.Recipe
                {
                    RecipeID = recipe.RecipeID,
                    FoodName = foodName,
                    Description = description
                };

                recipe = newRecipe;
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
