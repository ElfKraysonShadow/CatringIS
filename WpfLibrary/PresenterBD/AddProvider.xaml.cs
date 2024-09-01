using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CAT_COMPANY;

namespace WpfLibrary.PresenterBD
{
    /// <summary>
    /// Логика взаимодействия для AddProvider.xaml
    /// </summary>
    public partial class AddProvider : Window
    {
        public ProviderBd.Provider provider;
        private BankBd bankBd;
        private AddressBd adressBd;

        public AddProvider(ProviderBd.Provider pr, string bd)
        {
            bankBd = new BankBd(bd);
            adressBd = new AddressBd(bd);
            provider = pr;
            InitializeComponent();
            BankNameComboBox.ItemsSource = bankBd.Banks;
            BankNameComboBox.DisplayMemberPath = "BankName";
            FullAddressComboBox.ItemsSource = adressBd.AddressList;
            DisplayProviderDetails();
        }

        private void DisplayProviderDetails()
        {
            if (provider != null)
            {
                ProviderNameTextBox.Text = provider.ProviderName;
                ManagerNameTextBox.Text = provider.ManagerName;
                ManagerNumberTextBox.Text = provider.ManagerNumber;
                ProviderINNTextBox.Text = provider.ProviderINN;
                BankNameComboBox.SelectedItem = bankBd.Banks.FirstOrDefault(b => b.BankName == provider.BankName);
                BankAccountTextBox.Text = provider.BankAccount;
                FullAddressComboBox.SelectedItem = adressBd.AddressList.FirstOrDefault(a => (a.Street + ", " + a.HouseNumber) == provider.FullAddress);
            }
        }

        private void AddProviderButton_Click(object sender, RoutedEventArgs e)
        {
            string providerName = ProviderNameTextBox.Text;
            string managerName = ManagerNameTextBox.Text;
            string managerNumber = ManagerNumberTextBox.Text;
            string providerINN = ProviderINNTextBox.Text;
            string bankName = (BankNameComboBox.SelectedItem as BankBd.Bank)?.BankName;
            var selectedAddress = FullAddressComboBox.SelectedItem as AddressBd.Address;
            string fullAddress = selectedAddress != null ? $"{selectedAddress.Street}, {selectedAddress.HouseNumber}" : null;
            string bankAccount = BankAccountTextBox.Text;

            if (!string.IsNullOrEmpty(providerName) &&
                !string.IsNullOrEmpty(managerName) &&
                !string.IsNullOrEmpty(managerNumber) &&
                !string.IsNullOrEmpty(providerINN) &&
                !string.IsNullOrEmpty(bankName) &&
                !string.IsNullOrEmpty(fullAddress) &&
                !string.IsNullOrEmpty(bankAccount))
            {
                ProviderBd.Provider newProvider = new ProviderBd.Provider
                {
                    ProviderID = provider.ProviderID,
                    ProviderName = providerName,
                    ManagerName = managerName,
                    ManagerNumber = managerNumber,
                    ProviderINN = providerINN,
                    BankName = bankName,
                    FullAddress = fullAddress,
                    BankAccount = bankAccount,
                };

                provider = newProvider;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
