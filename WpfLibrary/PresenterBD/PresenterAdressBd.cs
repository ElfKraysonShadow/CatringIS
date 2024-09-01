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
    public class PresenterAddressBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _adressGrid;
        private AddressBd _adressBd;

        public PresenterAddressBd(Window view, string bd)
        {
            _adressBd = new AddressBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _adressGrid = new DataGrid();
            _adressGrid.Width = double.NaN;
            _adressGrid.CanUserAddRows = false;
            _adressGrid.AutoGenerateColumns = false;
            _adressGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("AddressID");

            DataGridTextColumn streetColumn = new DataGridTextColumn();
            streetColumn.Header = "Название улицы";
            streetColumn.Binding = new System.Windows.Data.Binding("Street");

            DataGridTextColumn houseNumberColumn = new DataGridTextColumn();
            houseNumberColumn.Header = "Номер дома";
            houseNumberColumn.Binding = new System.Windows.Data.Binding("HouseNumber");

            _adressGrid.Columns.Add(idColumn);
            _adressGrid.Columns.Add(streetColumn);
            _adressGrid.Columns.Add(houseNumberColumn);

            _adressGrid.ItemsSource = _adressBd.AddressList;
            gridPanel.Children.Add(_adressGrid);
        }

        public void AddObject()
        {
            AddAddress addWindow = new AddAddress(new AddressBd.Address());
            if (addWindow.ShowDialog() == true)
            {
                _adressBd.AddAddress(addWindow.adress);
                _adressGrid.ItemsSource = _adressBd.AddressList;
                _adressGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_adressGrid.SelectedItem != null)
            {
                int index = _adressGrid.SelectedIndex;
                AddressBd.Address selectedAddress = _adressBd.AddressList[index];
                AddAddress editWindow = new AddAddress(selectedAddress);

                if (editWindow.ShowDialog() == true)
                {
                    _adressBd.UpdateAddress(editWindow.adress);
                    MessageBox.Show("Адрес успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _adressGrid.ItemsSource = _adressBd.AddressList;
                    _adressGrid.Items.Refresh();
                }
            }
        }
        public void DeleteObject()
        {
            if (_adressGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить адрес?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _adressGrid.SelectedIndex;
                        AddressBd.Address selectedAddress = _adressBd.AddressList[index];

                        _adressBd.DeleteAddress(selectedAddress.AddressID);
                        _adressGrid.ItemsSource = _adressBd.AddressList;
                        _adressGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении адреса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<AddressBd.Address> filteredList = _adressBd.AddressList
                .Where(adress =>
                    adress.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(adress)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _adressGrid.ItemsSource = filteredList;
            _adressGrid.Items.Refresh();
        }
    }

}
