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
    public class PresenterIngredientBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _ingredientGrid;
        private IngredientBd _ingredientBd;
        private string BD;

        public PresenterIngredientBd(Window view, string bd)
        {
            _ingredientBd = new IngredientBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _ingredientGrid = new DataGrid();
            _ingredientGrid.Width = double.NaN;
            _ingredientGrid.CanUserAddRows = false;
            _ingredientGrid.AutoGenerateColumns = false;
            _ingredientGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("IngredientID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название";
            nameColumn.Binding = new System.Windows.Data.Binding("IngredientName");

            DataGridTextColumn unitColumn = new DataGridTextColumn();
            unitColumn.Header = "Единица измерения";
            unitColumn.Binding = new System.Windows.Data.Binding("UnitOfMeasure");

            DataGridTextColumn markupColumn = new DataGridTextColumn();
            markupColumn.Header = "Наценка";
            markupColumn.Binding = new System.Windows.Data.Binding("Markup");

            DataGridTextColumn stockColumn = new DataGridTextColumn();
            stockColumn.Header = "Остаток на складе";
            stockColumn.Binding = new System.Windows.Data.Binding("StockRemain");

            DataGridTextColumn providerNameColumn = new DataGridTextColumn();
            providerNameColumn.Header = "Поставщик";
            providerNameColumn.Binding = new System.Windows.Data.Binding("ProviderName");

            _ingredientGrid.Columns.Add(idColumn);
            _ingredientGrid.Columns.Add(nameColumn);
            _ingredientGrid.Columns.Add(unitColumn);
            _ingredientGrid.Columns.Add(markupColumn);
            _ingredientGrid.Columns.Add(stockColumn);
            _ingredientGrid.Columns.Add(providerNameColumn);

            _ingredientGrid.ItemsSource = _ingredientBd.IngredientList;
            gridPanel.Children.Add(_ingredientGrid);
        }

        public void AddObject()
        {
            AddIngredient addWindow = new AddIngredient(new IngredientBd.Ingredient(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _ingredientBd.AddIngredient(addWindow.ingredient);
                MessageBox.Show("Ингредиент успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _ingredientGrid.ItemsSource = _ingredientBd.IngredientList;
                _ingredientGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_ingredientGrid.SelectedItem != null)
            {
                int index = _ingredientGrid.SelectedIndex;
                IngredientBd.Ingredient selectedIngredient = _ingredientBd.IngredientList[index];
                AddIngredient editWindow = new AddIngredient(selectedIngredient, BD);

                if (editWindow.ShowDialog() == true)
                {
                    _ingredientBd.UpdateIngredient(editWindow.ingredient);
                    MessageBox.Show("Ингредиент успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _ingredientGrid.ItemsSource = _ingredientBd.IngredientList;
                    _ingredientGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_ingredientGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить ингредиент?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _ingredientGrid.SelectedIndex;
                        IngredientBd.Ingredient selectedIngredient = _ingredientBd.IngredientList[index];

                        _ingredientBd.DeleteIngredient(selectedIngredient.IngredientID);
                        _ingredientGrid.ItemsSource = _ingredientBd.IngredientList;
                        _ingredientGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении ингредиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<IngredientBd.Ingredient> filteredList = _ingredientBd.IngredientList
                .Where(ingredient =>
                    ingredient.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(ingredient)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _ingredientGrid.ItemsSource = filteredList;
            _ingredientGrid.Items.Refresh();
        }
    }
}
