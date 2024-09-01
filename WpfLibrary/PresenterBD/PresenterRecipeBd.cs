using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using CAT_COMPANY;

namespace WpfLibrary.PresenterBD
{
    public class PresenterRecipeIngredientBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _recipeIngredientGrid;
        private RecipeBd _recipeBd;
        private RecipeIngredientBd _recipeIngredientBd;
        private IngredientBd _ingredientBd;
        private string BD;

        public PresenterRecipeIngredientBd(Window view, string bd)
        {
            _recipeBd = new RecipeBd(bd);
            _ingredientBd = new IngredientBd(bd);
            _recipeIngredientBd = new RecipeIngredientBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _recipeIngredientGrid = new DataGrid();
            _recipeIngredientGrid.Width = double.NaN;
            _recipeIngredientGrid.CanUserAddRows = false;
            _recipeIngredientGrid.AutoGenerateColumns = false;
            _recipeIngredientGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("RecipeID");

            DataGridTextColumn foodNameColumn = new DataGridTextColumn();
            foodNameColumn.Header = "Блюдо";
            foodNameColumn.Binding = new System.Windows.Data.Binding("FoodName");

            DataGridTextColumn descriptionColumn = new DataGridTextColumn();
            descriptionColumn.Header = "Описание";
            descriptionColumn.Binding = new System.Windows.Data.Binding("Description");

            DataGridTextColumn ingredientNameColumn = new DataGridTextColumn();
            ingredientNameColumn.Header = "Название ингредиента";
            ingredientNameColumn.Binding = new System.Windows.Data.Binding("IngredientName");

            DataGridTextColumn grossWeightColumn = new DataGridTextColumn();
            grossWeightColumn.Header = "Вес брутто";
            grossWeightColumn.Binding = new System.Windows.Data.Binding("GrossWeight");

            DataGridTextColumn netWeightColumn = new DataGridTextColumn();
            netWeightColumn.Header = "Вес нетто";
            netWeightColumn.Binding = new System.Windows.Data.Binding("NetWeight");

            _recipeIngredientGrid.Columns.Add(idColumn);
            _recipeIngredientGrid.Columns.Add(foodNameColumn);
            _recipeIngredientGrid.Columns.Add(descriptionColumn);
            _recipeIngredientGrid.Columns.Add(ingredientNameColumn);
            _recipeIngredientGrid.Columns.Add(netWeightColumn);
            _recipeIngredientGrid.Columns.Add(grossWeightColumn);

            SetItemSource();
            gridPanel.Children.Add(_recipeIngredientGrid);
        }

        public void AddObject()
        {
            AddRecipe addWindow = new AddRecipe(new RecipeBd.Recipe(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _recipeBd.AddRecipe(addWindow.recipe);
                foreach (AddRecipe.SelectedRecipeIngredient Selected in addWindow.selectedIngredients)
                {
                    RecipeIngredientBd.RecipeIngredient newRpI = new RecipeIngredientBd.RecipeIngredient
                    {
                        RecipeID = _recipeBd.RecipeList.Last().RecipeID,
                        IngredientID = Selected.Ingredient.IngredientID,
                        NetWeight = Selected.NetWeight.Value,
                        GrossWeight = Selected.GrossWeight.Value
                    };
                    _recipeIngredientBd.AddRecipeIngredient(newRpI);
                }
                MessageBox.Show("Рецепт успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                SetItemSource();
                _recipeIngredientGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_recipeIngredientGrid.SelectedItem != null)
            {
                int index = _recipeIngredientGrid.SelectedIndex;
                RecipeIngredientBd.RecipeIngredient selectedRecipe = _recipeIngredientBd.RecipeIngredientList[index];
                EditRecipe editWindow = new EditRecipe(selectedRecipe);

                if (editWindow.ShowDialog() == true)
                {
                    _recipeIngredientBd.UpdateRecipeIngredient(editWindow.recipeIngredient);
                    MessageBox.Show("Рецепт успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetItemSource();
                    _recipeIngredientGrid.Items.Refresh();
                }
            }
        }
        public void DeleteObject()
        {
            if (_recipeIngredientGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить рецепт?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _recipeIngredientGrid.SelectedIndex;
                        int recipeIndex = _recipeBd.RecipeList.FindIndex(a => a.RecipeID == _recipeIngredientBd.RecipeIngredientList[index].RecipeID);
                        RecipeBd.Recipe selectedRecipe = _recipeBd.RecipeList[recipeIndex];

                        _recipeBd.DeleteRecipe(selectedRecipe.RecipeID);
                        _recipeIngredientBd.DeleteRecipeIngredient(selectedRecipe.RecipeID);
                        SetItemSource();
                        _recipeIngredientGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            var recipeIngredientList = from ri in _recipeIngredientBd.RecipeIngredientList
                                       join r in _recipeBd.RecipeList on ri.RecipeID equals r.RecipeID
                                       join i in _ingredientBd.IngredientList on ri.IngredientID equals i.IngredientID
                                       select new
                                       {
                                           ri.RecipeID,
                                           r.FoodName,
                                           r.Description,
                                           i.IngredientName,
                                           ri.GrossWeight,
                                           ri.NetWeight
                                       };
            var filteredList = recipeIngredientList
                .Where(recipe =>
                    recipe.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(recipe)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();
            var seenIds = new HashSet<int>();
            var seenFoods = new HashSet<string>();
            var seenDecsriptions = new HashSet<string>();

            var processedList = recipeIngredientList.Select(item =>
            {

                if (!seenIds.Contains(item.RecipeID))
                {
                    seenFoods.Clear();
                    seenDecsriptions.Clear();
                }

                var newItem = new
                {
                    RecipeID = seenIds.Contains(item.RecipeID) ? (int?)null : item.RecipeID,
                    FoodName = seenFoods.Contains(item.FoodName) ? null : item.FoodName,
                    IngredientName = item.IngredientName,
                    GrossWeight = item.GrossWeight,
                    Description = seenDecsriptions.Contains(item.Description) ? null : item.Description,
                    NetWeight = item.NetWeight
                };

                seenIds.Add(item.RecipeID);
                seenFoods.Add(item.FoodName);
                seenDecsriptions.Add(item.Description);

                return newItem;
            }).ToList();

            _recipeIngredientGrid.ItemsSource = processedList;
            _recipeIngredientGrid.Items.Refresh();
        }

        private void SetItemSource()
        {
            var recipeIngredientList = from ri in _recipeIngredientBd.RecipeIngredientList
                                       join r in _recipeBd.RecipeList on ri.RecipeID equals r.RecipeID
                                       join i in _ingredientBd.IngredientList on ri.IngredientID equals i.IngredientID
                                       select new
                                       {
                                           ri.RecipeID,
                                           r.FoodName,
                                           r.Description,
                                           i.IngredientName,
                                           ri.GrossWeight,
                                           ri.NetWeight
                                       };

            var seenIds = new HashSet<int>();
            var seenFoods = new HashSet<string>();
            var seenDecsriptions = new HashSet<string>();

            var processedList = recipeIngredientList.Select(item =>
            {

                if (!seenIds.Contains(item.RecipeID))
                {
                    seenFoods.Clear();
                    seenDecsriptions.Clear();
                }

                var newItem = new
                {
                    RecipeID = seenIds.Contains(item.RecipeID) ? (int?)null : item.RecipeID,
                    FoodName = seenFoods.Contains(item.FoodName) ? null : item.FoodName,
                    Description = seenDecsriptions.Contains(item.Description) ? null : item.Description,
                    IngredientName = item.IngredientName,
                    NetWeight = item.NetWeight,
                    GrossWeight = item.GrossWeight
                };

                seenIds.Add(item.RecipeID);
                seenFoods.Add(item.FoodName);
                seenDecsriptions.Add(item.Description);

                return newItem;
            }).ToList();

            _recipeIngredientGrid.ItemsSource = processedList;
        }
    }
}