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
    /// Логика взаимодействия для AddSoldVolume.xaml
    /// </summary>
    public partial class AddSoldVolume : Window
    {
        public SoldVolumeBd.SoldVolume soldVolume;
        private FoodBd foodBd;

        public AddSoldVolume(SoldVolumeBd.SoldVolume sv, string bd)
        {
            foodBd = new FoodBd(bd);
            soldVolume = sv;
            InitializeComponent();
            FoodNameComboBox.ItemsSource = foodBd.FoodList;
            FoodNameComboBox.DisplayMemberPath = "FoodName";
            DisplaySoldVolumeDetails();
        }

        private void DisplaySoldVolumeDetails()
        {
            if (soldVolume != null)
            {
                FoodNameComboBox.SelectedItem = foodBd.FoodList.FirstOrDefault(f => f.FoodName == soldVolume.FoodName);
                QuantityTextBox.Text = soldVolume.Quantity.ToString();
            }
        }

        private void AddSoldVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = (FoodNameComboBox.SelectedItem as FoodBd.Food)?.FoodName;
            int quantity;

            if (!string.IsNullOrEmpty(foodName) &&
                int.TryParse(QuantityTextBox.Text, out quantity))
            {
                SoldVolumeBd.SoldVolume newSoldVolume = new SoldVolumeBd.SoldVolume
                {
                    SoldID = soldVolume.SoldID,
                    FoodName = foodName,
                    Quantity = quantity,
                };

                soldVolume = newSoldVolume;
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
