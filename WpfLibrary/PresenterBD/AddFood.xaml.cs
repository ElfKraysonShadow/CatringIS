using Microsoft.Win32;
using System;
using System.IO;
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
    /// Логика взаимодействия для AddFood.xaml
    /// </summary>
    public partial class AddFood : Window
    {
        public FoodBd.Food food;
        private FoodGroupBd foodGroupBd;

        public AddFood(FoodBd.Food fd, string bd)
        {
            foodGroupBd = new FoodGroupBd(bd);
            food = fd;
            InitializeComponent();
            FoodGroupNameComboBox.ItemsSource = foodGroupBd.FoodGroups;
            FoodGroupNameComboBox.DisplayMemberPath = "FoodGroupName";
            DisplayFoodDetails();
        }

        private void DisplayFoodDetails()
        {
            if (food != null)
            {
                FoodNameTextBox.Text = food.FoodName;
                CostTextBox.Text = food.Cost.ToString();
                YieldTextBox.Text = food.Yield.ToString();
                DescriptionTextBox.Text = food.Description;
                FoodGroupNameComboBox.SelectedItem = foodGroupBd.FoodGroups.FirstOrDefault(p => p.FoodGroupName == food.FoodGroupName);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string foodName = FoodNameTextBox.Text;
            double cost;
            int yield;
            string description = DescriptionTextBox.Text;
            string foodGroupName = (FoodGroupNameComboBox.SelectedItem as FoodGroupBd.FoodGroup)?.FoodGroupName;

            if (!string.IsNullOrEmpty(foodName) &&
                double.TryParse(CostTextBox.Text, out cost) &&
                int.TryParse(YieldTextBox.Text, out yield) &&
                !string.IsNullOrEmpty(description) &&
                !string.IsNullOrEmpty(foodGroupName))
            {
                FoodBd.Food newFood = new FoodBd.Food
                {
                    FoodID = food.FoodID,
                    FoodName = foodName,
                    FoodGroupName = foodGroupName,
                    Cost = cost,
                    Yield = yield,
                    Description = description,
                    Photo = food.Photo
                };

                food = newFood;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Заполните обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddPhoto_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;


                    byte[] imageBytes = File.ReadAllBytes(filePath);


                    food.Photo = imageBytes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
