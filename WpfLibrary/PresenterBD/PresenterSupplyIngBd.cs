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
    public class PresenterSupplyIngredientBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _supplyIngredientGrid;
        private SupplyBd _supplyBd;
        private SupplyIngredientBd _supplyIngredientBd;
        private IngredientBd _ingredientBd;
        private string BD;

        public PresenterSupplyIngredientBd(Window view, string bd)
        {
            _supplyBd = new SupplyBd(bd);
            _ingredientBd = new IngredientBd(bd);
            _supplyIngredientBd = new SupplyIngredientBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _supplyIngredientGrid = new DataGrid();
            _supplyIngredientGrid.Width = double.NaN;
            _supplyIngredientGrid.CanUserAddRows = false;
            _supplyIngredientGrid.AutoGenerateColumns = false;
            _supplyIngredientGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("SupplyID");

            DataGridTextColumn providerNameColumn = new DataGridTextColumn();
            providerNameColumn.Header = "Название отдела";
            providerNameColumn.Binding = new System.Windows.Data.Binding("ProviderName");

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

            DataGridTextColumn purchasePriceColumn = new DataGridTextColumn();
            purchasePriceColumn.Header = "Стоимость покупки";
            purchasePriceColumn.Binding = new System.Windows.Data.Binding("PurnashePrice");

            _supplyIngredientGrid.Columns.Add(idColumn);
            _supplyIngredientGrid.Columns.Add(providerNameColumn);
            _supplyIngredientGrid.Columns.Add(dateColumn);
            _supplyIngredientGrid.Columns.Add(ingredientNameColumn);
            _supplyIngredientGrid.Columns.Add(quantityColumn);
            _supplyIngredientGrid.Columns.Add(unitOfMeasureColumn);
            _supplyIngredientGrid.Columns.Add(purchasePriceColumn);


            SetItemSource();
            gridPanel.Children.Add(_supplyIngredientGrid);
        }
        public void AddObject()
        {
            AddSupply addWindow = new AddSupply(new SupplyBd.Supply(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _supplyBd.AddSupply(addWindow.supply);
                foreach (AddSupply.SelectedSupplyIngredient Selected in addWindow.selectedIngredients)
                {
                    SupplyIngredientBd.SupplyIngredient newSpI = new SupplyIngredientBd.SupplyIngredient
                    {
                        SupplyID = _supplyBd.SupplyList.Last().SupplyID,
                        IngredientID = Selected.Ingredient.IngredientID,
                        Quantity = Selected.Quantity.Value,
                        PurnashePrice = Selected.PurnashePrice.Value
                    };
                    _supplyIngredientBd.AddSupplyIngredient(newSpI);
                }
                MessageBox.Show("Поставка успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                SetItemSource();
                _supplyIngredientGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_supplyIngredientGrid.SelectedItem != null)
            {
                int index = _supplyIngredientGrid.SelectedIndex;
                SupplyIngredientBd.SupplyIngredient selectedSupply = _supplyIngredientBd.SupplyIngredientList[index];
                EditSupply editWindow = new EditSupply(selectedSupply);

                if (editWindow.ShowDialog() == true)
                {
                    _supplyIngredientBd.UpdateSupplyIngredient(editWindow.supplyIngredient);
                    MessageBox.Show("Поставка успешно изменена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    SetItemSource();
                    _supplyIngredientGrid.Items.Refresh();
                }
            }
        }
        public void DeleteObject()
        {
            if (_supplyIngredientGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить поставку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _supplyIngredientGrid.SelectedIndex;
                        int supplyIndex = _supplyBd.SupplyList.FindIndex(a => a.SupplyID == _supplyIngredientBd.SupplyIngredientList[index].SupplyID);
                        SupplyBd.Supply selectedSupply = _supplyBd.SupplyList[supplyIndex];

                        _supplyBd.DeleteSupply(selectedSupply.SupplyID);
                        _supplyIngredientBd.DeleteSupplyIngredient(selectedSupply.SupplyID);
                        SetItemSource();
                        _supplyIngredientGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении поставки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            var supplyIngredientList = from si in _supplyIngredientBd.SupplyIngredientList
                                       join s in _supplyBd.SupplyList on si.SupplyID equals s.SupplyID
                                       join i in _ingredientBd.IngredientList on si.IngredientID equals i.IngredientID
                                       select new
                                       {
                                           si.SupplyID,
                                           s.Date,
                                           s.ProviderName,
                                           i.IngredientName,
                                           si.Quantity,
                                           i.UnitOfMeasure,
                                           si.PurnashePrice
                                       };
            var filteredList = supplyIngredientList
                .Where(supply =>
                    supply.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(supply)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();
            var seenIds = new HashSet<int>();
            var seenProviders = new HashSet<string>();
            var seenDates = new HashSet<DateTime>();

            var processedList = supplyIngredientList.Select(item =>
            {

                if (!seenIds.Contains(item.SupplyID))
                {
                    seenProviders.Clear();
                    seenDates.Clear();
                }

                var newItem = new
                {
                    SupplyID = seenIds.Contains(item.SupplyID) ? (int?)null : item.SupplyID,
                    ProviderName = seenProviders.Contains(item.ProviderName) ? null : item.ProviderName,
                    IngredientName = item.IngredientName,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Date = seenDates.Contains(item.Date) ? (DateTime?)null : item.Date,
                    PurnashePrice = item.PurnashePrice
                };

                seenIds.Add(item.SupplyID);
                seenProviders.Add(item.ProviderName);
                seenDates.Add(item.Date);

                return newItem;
            }).ToList();

            _supplyIngredientGrid.ItemsSource = processedList;
            _supplyIngredientGrid.Items.Refresh();
        }

        private void SetItemSource()
        {
            var supplyIngredientList = from si in _supplyIngredientBd.SupplyIngredientList
                                     join s in _supplyBd.SupplyList on si.SupplyID equals s.SupplyID
                                     join i in _ingredientBd.IngredientList on si.IngredientID equals i.IngredientID
                                     select new
                                     {
                                         si.SupplyID,
                                         s.Date,
                                         s.ProviderName,
                                         i.IngredientName,
                                         si.Quantity,
                                         i.UnitOfMeasure,
                                         si.PurnashePrice
                                     };

            var seenIds = new HashSet<int>();
            var seenProviders = new HashSet<string>();
            var seenDates = new HashSet<DateTime>();

            var processedList = supplyIngredientList.Select(item =>
            {

                if (!seenIds.Contains(item.SupplyID))
                {
                    seenProviders.Clear();
                    seenDates.Clear();
                }

                var newItem = new
                {
                    SupplyID = seenIds.Contains(item.SupplyID) ? (int?)null : item.SupplyID,
                    ProviderName = seenProviders.Contains(item.ProviderName) ? null : item.ProviderName,
                    IngredientName = item.IngredientName,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Date = seenDates.Contains(item.Date) ? (DateTime?)null : item.Date,
                    PurnashePrice = item.PurnashePrice
                };

                seenIds.Add(item.SupplyID);
                seenProviders.Add(item.ProviderName);
                seenDates.Add(item.Date);

                return newItem;
            }).ToList();

            _supplyIngredientGrid.ItemsSource = processedList;
        }
    }
}