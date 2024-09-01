using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using CAT_COMPANY;
using System.Windows.Data;

namespace WpfLibrary.PresenterBD
{
    public class PresenterFoodBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _foodGrid;
        private FoodBd _foodBd;
        private string BD;

        public PresenterFoodBd(Window view, string bd)
        {
            _foodBd = new FoodBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _foodGrid = new DataGrid();
            _foodGrid.Width = double.NaN;
            _foodGrid.CanUserAddRows = false;
            _foodGrid.AutoGenerateColumns = false;
            _foodGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("FoodID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название";
            nameColumn.Binding = new System.Windows.Data.Binding("FoodName");

            DataGridTextColumn groupColumn = new DataGridTextColumn();
            groupColumn.Header = "Группа";
            groupColumn.Binding = new System.Windows.Data.Binding("FoodGroupName");

            DataGridTextColumn costColumn = new DataGridTextColumn();
            costColumn.Header = "Стоимость";
            costColumn.Binding = new System.Windows.Data.Binding("Cost");

            DataGridTextColumn yieldColumn = new DataGridTextColumn();
            yieldColumn.Header = "Выход";
            yieldColumn.Binding = new System.Windows.Data.Binding("Yield");

            DataGridTextColumn descriptionColumn = new DataGridTextColumn();
            descriptionColumn.Header = "Описание";
            descriptionColumn.Binding = new System.Windows.Data.Binding("Description");

            DataGridTemplateColumn photoColumn = new DataGridTemplateColumn();
            photoColumn.Header = "Фото";
            FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetBinding(Image.SourceProperty, new Binding("Photo"));
            imageFactory.SetValue(FrameworkElement.WidthProperty, 50.0);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 50.0);
            DataTemplate cellTemplate = new DataTemplate { VisualTree = imageFactory };
            photoColumn.CellTemplate = cellTemplate;

            _foodGrid.Columns.Add(idColumn);
            _foodGrid.Columns.Add(nameColumn);
            _foodGrid.Columns.Add(groupColumn);
            _foodGrid.Columns.Add(costColumn);
            _foodGrid.Columns.Add(yieldColumn);
            _foodGrid.Columns.Add(descriptionColumn);
            _foodGrid.Columns.Add(photoColumn);

            _foodGrid.ItemsSource = _foodBd.FoodList;
            gridPanel.Children.Add(_foodGrid);
        }

        public void AddObject()
        {
            AddFood addWindow = new AddFood(new FoodBd.Food(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _foodBd.AddFood(addWindow.food);
                MessageBox.Show("Блюдо> успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _foodGrid.ItemsSource = _foodBd.FoodList;
                _foodGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_foodGrid.SelectedItem != null)
            {
                int index = _foodGrid.SelectedIndex;
                FoodBd.Food selectedFood = _foodBd.FoodList[index];
                AddFood editWindow = new AddFood(selectedFood, BD);

                if (editWindow.ShowDialog() == true)
                {
                    _foodBd.UpdateFood(editWindow.food);
                    MessageBox.Show("Блюдо успешно обновлёно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _foodGrid.ItemsSource = _foodBd.FoodList;
                    _foodGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_foodGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить блюдо?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _foodGrid.SelectedIndex;
                        FoodBd.Food selectedFood = _foodBd.FoodList[index];

                        _foodBd.DeleteFood(selectedFood.FoodID);
                        _foodGrid.ItemsSource = _foodBd.FoodList;
                        _foodGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении блюда: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<FoodBd.Food> filteredList = _foodBd.FoodList
                .Where(food =>
                    food.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(food)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _foodGrid.ItemsSource = filteredList;
            _foodGrid.Items.Refresh();
        }
    }
}
