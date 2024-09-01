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
    /// Логика взаимодействия для AddAddress.xaml
    /// </summary>
    public partial class AddAddress : Window
    {
        public AddressBd.Address adress;

        public AddAddress(AddressBd.Address ad)
        {
            InitializeComponent();
            adress = ad;
            DisplayAddressDetails();
        }
        private void DisplayAddressDetails()
        {
            if (adress != null)
            {
                StreetTextBox.Text = adress.Street;
                HouseNumberTextBox.Text = adress.HouseNumber;
            }
        }

        private void AddAddressButton_Click(object sender, RoutedEventArgs e)
        {
            string street = StreetTextBox.Text;
            string houseNumber = HouseNumberTextBox.Text;

            if (!string.IsNullOrEmpty(street) && !string.IsNullOrEmpty(houseNumber))
            {
                AddressBd.Address newAddress = new AddressBd.Address
                {
                    AddressID = adress.AddressID,
                    Street = street,
                    HouseNumber = houseNumber,
                };

                adress = newAddress;
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
