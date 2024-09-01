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
    public class PresenterBankBd : IPresenterCommon
    {
        private Window _view;
        private DataGrid bankGrid;
        private BankBd bankBd;

        public PresenterBankBd(Window view, string bd)
        {
            bankBd = new BankBd(bd);
            _view = view;

            StackPanel gridPanel = _view.FindName("GridPanel") as StackPanel;
            gridPanel.Children.Clear();
            bankGrid = new DataGrid();
            bankGrid.Width = double.NaN;
            bankGrid.CanUserAddRows = false;
            bankGrid.AutoGenerateColumns = false;
            bankGrid.IsReadOnly = true;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "ID";
            idColumn.Binding = new System.Windows.Data.Binding("BankID");

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            nameColumn.Header = "Название банка";
            nameColumn.Binding = new System.Windows.Data.Binding("BankName");

            bankGrid.Columns.Add(idColumn);
            bankGrid.Columns.Add(nameColumn);

            bankGrid.ItemsSource = bankBd.Banks;
            gridPanel.Children.Add(bankGrid);
        }

        public void AddObject()
        {
            string bankName = GetBankNameFromUser("Введите название банка:");

            if (!string.IsNullOrEmpty(bankName))
            {
                BankBd.Bank newBank = new BankBd.Bank
                {
                    BankName = bankName
                };

                bankBd.AddBank(newBank);
                bankGrid.ItemsSource = bankBd.Banks;
                bankGrid.Items.Refresh();
            }
        }

        public void EditObject()
        {
            if (bankGrid.SelectedItem != null)
            {
                int index = bankGrid.SelectedIndex;
                BankBd.Bank selectedBank = bankBd.Banks[index];

                string newBankName = GetBankNameFromUser("Введите новое название банка:", selectedBank.BankName);

                if (!string.IsNullOrEmpty(newBankName))
                {
                    selectedBank.BankName = newBankName;

                    bankBd.UpdateBank(selectedBank);
                    bankGrid.ItemsSource = bankBd.Banks;
                    bankGrid.Items.Refresh();
                }
            }
        }

        public void DeleteObject()
        {
            if (bankGrid.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Точно хотите удалить банк?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int index = bankGrid.SelectedIndex;
                        BankBd.Bank selectedBank = bankBd.Banks[index];
                        bankBd.DeleteBank(selectedBank.BankID);
                        bankGrid.ItemsSource = bankBd.Banks;
                        bankGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении банка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void Search(string searchTerm)
        {
            List<BankBd.Bank> filteredList = bankBd.Banks
                .Where(bank =>
                    bank.GetType().GetProperties()
                        .Any(prop =>
                            prop.GetValue(bank)?.ToString()?.ToLower().Contains(searchTerm.ToLower()) ?? false
                        )
                )
                .ToList();

            bankGrid.ItemsSource = filteredList;
            bankGrid.Items.Refresh();
        }

        private string GetBankNameFromUser(string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, "User Input", defaultValue);
        }
    }

}
