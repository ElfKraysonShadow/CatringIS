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
    public class PresenterApplicationIngBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _applicationIngGrid;
        private ApplicationBd _applicationBd;
        private ApplicationIngBd _applicationIngBd;
        private IngredientBd _ingredientBd;
        private string BD;

        public PresenterApplicationIngBd(Window view, string bd)
        {
            _applicationBd = new ApplicationBd(bd);
            _ingredientBd = new IngredientBd(bd);
            _applicationIngBd = new ApplicationIngBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _applicationIngGrid = new DataGrid();
            _applicationIngGrid.Width = double.NaN;
            _applicationIngGrid.CanUserAddRows = false;
            _applicationIngGrid.AutoGenerateColumns = false;
            _applicationIngGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ApplicationID");

            DataGridTextColumn departmentNameColumn = new DataGridTextColumn();
            departmentNameColumn.Header = "Название отдела";
            departmentNameColumn.Binding = new System.Windows.Data.Binding("DepartmentName");

            DataGridTextColumn dateColumn = new DataGridTextColumn();
            dateColumn.Header = "Дата заявки";
            dateColumn.Binding = new System.Windows.Data.Binding("Date");

            DataGridTextColumn ingredientNameColumn = new DataGridTextColumn();
            ingredientNameColumn.Header = "Название ингредиента";
            ingredientNameColumn.Binding = new System.Windows.Data.Binding("IngredientName");

            DataGridTextColumn unitOfMeasureColumn = new DataGridTextColumn();
            unitOfMeasureColumn.Header = "Мера измерения";
            unitOfMeasureColumn.Binding = new System.Windows.Data.Binding("UnitOfMeasure");

            DataGridTextColumn quantityColumn = new DataGridTextColumn();
            quantityColumn.Header = "Количество";
            quantityColumn.Binding = new System.Windows.Data.Binding("Quantity");

            _applicationIngGrid.Columns.Add(idColumn);
            _applicationIngGrid.Columns.Add(departmentNameColumn);
            _applicationIngGrid.Columns.Add(dateColumn);
            _applicationIngGrid.Columns.Add(ingredientNameColumn);
            _applicationIngGrid.Columns.Add(quantityColumn);
            _applicationIngGrid.Columns.Add(unitOfMeasureColumn);


            SetItemSource();
            gridPanel.Children.Add(_applicationIngGrid);
        }
        public void AddObject()
        {
            AddApplication addWindow = new AddApplication(new ApplicationBd.Application(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _applicationBd.AddApplication(addWindow.application);
                foreach (AddApplication.SelectedIngredient Selected in addWindow.selectedIngredients)
                {
                    ApplicationIngBd.ApplicationIng newApI = new ApplicationIngBd.ApplicationIng
                    {
                        ApplicationID = _applicationBd.ApplicationList.Last().ApplicationID,
                        IngredientID = Selected.Ingredient.IngredientID,
                        Quantity = Selected.Quantity.Value
                    };
                    _applicationIngBd.AddApplicationIng(newApI);
                }
                MessageBox.Show("Заявка успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                SetItemSource();
                _applicationIngGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_applicationIngGrid.SelectedItem != null)
            {
                int index = _applicationIngGrid.SelectedIndex;
                ApplicationIngBd.ApplicationIng selectedApplication = _applicationIngBd.ApplicationIngList[index];
                double? newQuantity = Convert.ToDouble(GetQuantityFromUser("Введите новое количество:", Convert.ToString(selectedApplication.Quantity)));
                if (newQuantity.HasValue)
                {
                    selectedApplication.Quantity = newQuantity.Value;
                    _applicationIngBd.UpdateApplicationIng(selectedApplication);
                    SetItemSource();
                    _applicationIngGrid.Items.Refresh();
                }
            }
        }
        public void DeleteObject()
        {
            if (_applicationIngGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить заявку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _applicationIngGrid.SelectedIndex;
                        int applicationIndex = _applicationBd.ApplicationList.FindIndex(a => a.ApplicationID == _applicationIngBd.ApplicationIngList[index].ApplicationID);
                        ApplicationBd.Application selectedApplication = _applicationBd.ApplicationList[applicationIndex];

                        _applicationBd.DeleteApplication(selectedApplication.ApplicationID);
                        _applicationIngBd.DeleteApplicationIng(selectedApplication.ApplicationID);
                        SetItemSource();
                        _applicationIngGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            var applicationIngList = from ai in _applicationIngBd.ApplicationIngList
                                     join a in _applicationBd.ApplicationList on ai.ApplicationID equals a.ApplicationID
                                     join i in _ingredientBd.IngredientList on ai.IngredientID equals i.IngredientID
                                     select new
                                     {
                                         ai.ApplicationID,
                                         a.DepartmentName,
                                         a.Date,
                                         i.IngredientName,
                                         ai.Quantity,
                                         i.UnitOfMeasure
                                     };
            var filteredList = applicationIngList
                .Where(application =>
                    application.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(application)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();
            var seenIds = new HashSet<int>();
            var seenDepartments = new HashSet<string>();
            var seenDates = new HashSet<DateTime>();

            var processedList = filteredList.Select(item =>
            {

                if (!seenIds.Contains(item.ApplicationID))
                {
                    seenDepartments.Clear();
                    seenDates.Clear();
                }

                var newItem = new
                {
                    ApplicationID = seenIds.Contains(item.ApplicationID) ? (int?)null : item.ApplicationID,
                    DepartmentName = seenDepartments.Contains(item.DepartmentName) ? null : item.DepartmentName,
                    IngredientName = item.IngredientName,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Date = seenDates.Contains(item.Date) ? (DateTime?)null : item.Date
                };

                seenIds.Add(item.ApplicationID);
                seenDepartments.Add(item.DepartmentName);
                seenDates.Add(item.Date);

                return newItem;
            }).ToList();
            _applicationIngGrid.ItemsSource = processedList;
            _applicationIngGrid.Items.Refresh();
        }

        private void SetItemSource()
        {
            var applicationIngList = from ai in _applicationIngBd.ApplicationIngList
                                     join a in _applicationBd.ApplicationList on ai.ApplicationID equals a.ApplicationID
                                     join i in _ingredientBd.IngredientList on ai.IngredientID equals i.IngredientID
                                     select new
                                     {
                                         ai.ApplicationID,
                                         a.DepartmentName,
                                         a.Date,
                                         i.IngredientName,
                                         ai.Quantity,
                                         i.UnitOfMeasure
                                     };

            var seenIds = new HashSet<int>();
            var seenDepartments = new HashSet<string>();
            var seenDates = new HashSet<DateTime>();

            var processedList = applicationIngList.Select(item =>
            {

                if (!seenIds.Contains(item.ApplicationID))
                {
                    seenDepartments.Clear();
                    seenDates.Clear();
                }

                var newItem = new
                {
                    ApplicationID = seenIds.Contains(item.ApplicationID) ? (int?)null : item.ApplicationID,
                    DepartmentName = seenDepartments.Contains(item.DepartmentName) ? null : item.DepartmentName,
                    IngredientName = item.IngredientName,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Date = seenDates.Contains(item.Date) ? (DateTime?)null : item.Date
                };

                seenIds.Add(item.ApplicationID);
                seenDepartments.Add(item.DepartmentName);
                seenDates.Add(item.Date);

                return newItem;
            }).ToList();

            _applicationIngGrid.ItemsSource = processedList;
        }

        private string GetQuantityFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }
}