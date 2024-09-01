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
    public class PresenterProviderBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid _providerGrid;
        private ProviderBd _providerBd;
        private string BD;

        public PresenterProviderBd(Window view, string bd)
        {
            _providerBd = new ProviderBd(bd);
            _view = view;
            BD = bd;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();

            _providerGrid = new DataGrid();
            _providerGrid.Width = double.NaN;
            _providerGrid.CanUserAddRows = false;
            _providerGrid.AutoGenerateColumns = false;
            _providerGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("ProviderID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название";
            nameColumn.Binding = new System.Windows.Data.Binding("ProviderName");

            DataGridTextColumn managerNameColumn = new DataGridTextColumn();
            managerNameColumn.Header = "Имя менеджера";
            managerNameColumn.Binding = new System.Windows.Data.Binding("ManagerName");

            DataGridTextColumn managerNumberColumn = new DataGridTextColumn();
            managerNumberColumn.Header = "Номер менеджера";
            managerNumberColumn.Binding = new System.Windows.Data.Binding("ManagerNumber");

            DataGridTextColumn providerINNColumn = new DataGridTextColumn();
            providerINNColumn.Header = "ИНН поставщика";
            providerINNColumn.Binding = new System.Windows.Data.Binding("ProviderINN");

            DataGridTextColumn bankNameColumn = new DataGridTextColumn();
            bankNameColumn.Header = "Банк";
            bankNameColumn.Binding = new System.Windows.Data.Binding("BankName");

            DataGridTextColumn bankAccountColumn = new DataGridTextColumn();
            bankAccountColumn.Header = "Банковский счет";
            bankAccountColumn.Binding = new System.Windows.Data.Binding("BankAccount");

            DataGridTextColumn fullAddressColumn = new DataGridTextColumn();
            fullAddressColumn.Header = "Адрес";
            fullAddressColumn.Binding = new System.Windows.Data.Binding("FullAddress");

            _providerGrid.Columns.Add(idColumn);
            _providerGrid.Columns.Add(nameColumn);
            _providerGrid.Columns.Add(managerNameColumn);
            _providerGrid.Columns.Add(managerNumberColumn);
            _providerGrid.Columns.Add(providerINNColumn);
            _providerGrid.Columns.Add(bankNameColumn);
            _providerGrid.Columns.Add(bankAccountColumn);
            _providerGrid.Columns.Add(fullAddressColumn);

            _providerGrid.ItemsSource = _providerBd.ProviderList;
            gridPanel.Children.Add(_providerGrid);
        }


        public void AddObject()
        {
            AddProvider addWindow = new AddProvider(new ProviderBd.Provider(), BD);
            if (addWindow.ShowDialog() == true)
            {
                _providerBd.AddProvider(addWindow.provider);
                MessageBox.Show($"Поставщик успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _providerGrid.ItemsSource = _providerBd.ProviderList;
                _providerGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (_providerGrid.SelectedItem != null)
            {
                int index = _providerGrid.SelectedIndex;
                ProviderBd.Provider selectedProvider = _providerBd.ProviderList[index];
                AddProvider editWindow = new AddProvider(selectedProvider, BD);

                if (editWindow.ShowDialog() == true)
                {
                    _providerBd.UpdateProvider(editWindow.provider);
                    MessageBox.Show($"Поставщик успешно обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _providerGrid.ItemsSource = _providerBd.ProviderList;
                    _providerGrid.Items.Refresh();
                }
            }
        }
        public void DeleteObject()
        {
            if (_providerGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить поставщика?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = _providerGrid.SelectedIndex;
                        ProviderBd.Provider selectedProvider = _providerBd.ProviderList[index];

                        _providerBd.DeleteProvider(selectedProvider.ProviderID);
                        _providerGrid.ItemsSource = _providerBd.ProviderList;
                        _providerGrid.Items.Refresh();
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
            List<ProviderBd.Provider> filteredList = _providerBd.ProviderList
                .Where(provider =>
                    provider.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(provider)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            _providerGrid.ItemsSource = filteredList;
            _providerGrid.Items.Refresh();
        }
    }

}
