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
    public class PresenterFoodGroupBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid foodGroupGrid;
        private FoodGroupBd foodGroupBd;

        public PresenterFoodGroupBd(Window view, string bd)
        {
            foodGroupBd = new FoodGroupBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            foodGroupGrid = new DataGrid();
            foodGroupGrid.Width = double.NaN;
            foodGroupGrid.CanUserAddRows = false;
            foodGroupGrid.AutoGenerateColumns = false;
            foodGroupGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("FoodGroupID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название группы";
            nameColumn.Binding = new System.Windows.Data.Binding("FoodGroupName");

            foodGroupGrid.Columns.Add(idColumn);
            foodGroupGrid.Columns.Add(nameColumn);

            foodGroupGrid.ItemsSource = foodGroupBd.FoodGroups;
            gridPanel.Children.Add(foodGroupGrid);
        }

        public void AddObject()
        {
            string foodGroupName = GetFoodGroupNameFromUser("Введите название группы:");

            if (!string.IsNullOrEmpty(foodGroupName))
            {
                FoodGroupBd.FoodGroup newFoodGroup = new FoodGroupBd.FoodGroup
                {
                    FoodGroupName = foodGroupName
                };

                foodGroupBd.AddFoodGroup(newFoodGroup);
                foodGroupGrid.ItemsSource = foodGroupBd.FoodGroups;
                foodGroupGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (foodGroupGrid.SelectedItem != null)
            {
                int index = foodGroupGrid.SelectedIndex;
                FoodGroupBd.FoodGroup selectedFoodGroup = foodGroupBd.FoodGroups[index];

                string newFoodGroupName = GetFoodGroupNameFromUser("Введите новое название группы:", selectedFoodGroup.FoodGroupName);

                if (!string.IsNullOrEmpty(newFoodGroupName))
                {
                    selectedFoodGroup.FoodGroupName = newFoodGroupName;

                    foodGroupBd.UpdateFoodGroup(selectedFoodGroup);
                    foodGroupGrid.ItemsSource = foodGroupBd.FoodGroups;
                    foodGroupGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (foodGroupGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = foodGroupGrid.SelectedIndex;
                        FoodGroupBd.FoodGroup selectedFoodGroup = foodGroupBd.FoodGroups[index];
                        foodGroupBd.DeleteFoodGroup(selectedFoodGroup.FoodGroupID);
                        foodGroupGrid.ItemsSource = foodGroupBd.FoodGroups;
                        foodGroupGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<FoodGroupBd.FoodGroup> filteredList = foodGroupBd.FoodGroups
                .Where(foodGroup =>
                    foodGroup.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(foodGroup)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            foodGroupGrid.ItemsSource = filteredList;
            foodGroupGrid.Items.Refresh();
        }

        private string GetFoodGroupNameFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
