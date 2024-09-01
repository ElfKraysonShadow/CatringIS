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
    public class PresenterSoldVolumeBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _soldVolumeGrid;
        private SoldVolumeBd _soldVolumeBd;
        string BD;

        public PresenterSoldVolumeBd(Window view, string bd)
        {
            _soldVolumeBd = new SoldVolumeBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _soldVolumeGrid = new DataGrid();
            _soldVolumeGrid.Width = double.NaN;
            _soldVolumeGrid.CanUserAddRows = false;
            _soldVolumeGrid.AutoGenerateColumns = false;
            _soldVolumeGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("SoldID");

            DataGridTextColumn foodNameColumn = new DataGridTextColumn();
            foodNameColumn.Header = "Название продукта";
            foodNameColumn.Binding = new System.Windows.Data.Binding("FoodName");

            DataGridTextColumn quantityColumn = new DataGridTextColumn();
            quantityColumn.Header = "Количество";
            quantityColumn.Binding = new System.Windows.Data.Binding("Quantity");

            _soldVolumeGrid.Columns.Add(idColumn);
            _soldVolumeGrid.Columns.Add(foodNameColumn);
            _soldVolumeGrid.Columns.Add(quantityColumn);

            _soldVolumeGrid.ItemsSource = _soldVolumeBd.SoldVolumeList;
            gridPanel.Children.Add(_soldVolumeGrid);
        }

        public void AddObject()
        {
            AddSoldVolume addWindow = new AddSoldVolume(new SoldVolumeBd.SoldVolume(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _soldVolumeBd.AddSoldVolume(addWindow.soldVolume);
                _soldVolumeGrid.ItemsSource = _soldVolumeBd.SoldVolumeList;
                _soldVolumeGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_soldVolumeGrid.SelectedItem != null)
            {
                int index = _soldVolumeGrid.SelectedIndex;
                SoldVolumeBd.SoldVolume selectedSoldVolume = _soldVolumeBd.SoldVolumeList[index];
                AddSoldVolume editWindow = new AddSoldVolume(selectedSoldVolume, BD);

                if (editWindow.ShowDialog() == true)
                {
                    _soldVolumeBd.UpdateSoldVolume(editWindow.soldVolume);
                    MessageBox.Show("Проданный объем успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _soldVolumeGrid.ItemsSource = _soldVolumeBd.SoldVolumeList;
                    _soldVolumeGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (_soldVolumeGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить проданный объем?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _soldVolumeGrid.SelectedIndex;
                        SoldVolumeBd.SoldVolume selectedSoldVolume = _soldVolumeBd.SoldVolumeList[index];

                        _soldVolumeBd.DeleteSoldVolume(selectedSoldVolume.SoldID);
                        _soldVolumeGrid.ItemsSource = _soldVolumeBd.SoldVolumeList;
                        _soldVolumeGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении проданного объема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<SoldVolumeBd.SoldVolume> filteredList = _soldVolumeBd.SoldVolumeList
                .Where(soldVolume =>
                    soldVolume.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(soldVolume)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _soldVolumeGrid.ItemsSource = filteredList;
            _soldVolumeGrid.Items.Refresh();
        }
    }
}
