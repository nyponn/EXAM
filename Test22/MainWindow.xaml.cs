using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EXAM.Scripts;

namespace EXAM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productName = ItemComboBox.SelectedItem.ToString(); // Название продукта
                DateTime transportDate = TransportDatePicker.SelectedDate ?? DateTime.Now; // Дата транспортировки

                // Проверка даты перевозки
                if (transportDate > DateTime.Now)
                {
                    MessageBox.Show("Дата перевозки не может быть больше сегодняшней.");
                    return;
                }

                int transportTime = int.Parse(TransportTimeTextBox.Text); // Время транспортировки
                int? minTemp = string.IsNullOrWhiteSpace(MinTempTextBox.Text) ? (int?)null : int.Parse(MinTempTextBox.Text);
                int? maxTemp = string.IsNullOrWhiteSpace(MaxTempTextBox.Text) ? (int?)null : int.Parse(MaxTempTextBox.Text);

                // Проверка минимальной и максимальной температуры
                if (minTemp.HasValue && maxTemp.HasValue && minTemp.Value > maxTemp.Value)
                {
                    MessageBox.Show("Минимальная температура не может быть больше максимальной.");
                    return;
                }

                var temperatures = TemperatureInputTextBox.Text.Split(',').Select(int.Parse).ToArray();
                int allowedViolationTime = int.Parse(AllowedViolationTimeTextBox.Text); // Допустимое время в неподходящей температуре

                // Проверка, что количество температур не превышает ожидаемое
                int expectedTemperatures = transportTime * 6; // Каждые 10 минут за час
                if (temperatures.Length > expectedTemperatures)
                {
                    MessageBox.Show($"Количество температур превышает ожидаемое ({expectedTemperatures}).");
                    return;
                }

                // Проверка, что поездка еще не завершена
                bool isTripCompleted = temperatures.Length == expectedTemperatures;

                var monitor = new TemperatureMonitor(productName, transportDate, transportTime, minTemp, maxTemp, temperatures, allowedViolationTime, isTripCompleted);
                var report = monitor.CheckConditions();

                ResultTextBlock.Text = report;
                SaveReportToFile(report);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var (startTime, temperatures) = DataLoader.LoadDataFromFile(openFileDialog.FileName);
                TransportDatePicker.SelectedDate = startTime;
                TemperatureInputTextBox.Text = string.Join(", ", temperatures);
            }
        }

        private void SaveReportToFile(string report)
        {
            string filePath = "report.txt";
            File.WriteAllText(filePath, report);
            MessageBox.Show($"Отчет сохранен в {filePath}");
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем ввод цифр и минуса
            Regex regex = new Regex("[^0-9-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TransportTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TransportTimeTextBox.Text, out int value))
            {
                if (value < 3 || value > 9)
                {
                    MessageBox.Show("Срок доставки должен быть от 3 до 9 часов.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void MinTempTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Дополнительные проверки для минимальной температуры, если необходимо
        }

        private void MaxTempTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Дополнительные проверки для максимальной температуры, если необходимо
        }

        private void AllowedViolationTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AllowedViolationTimeTextBox.Text, out int value))
            {
                if (value < 0)
                {
                    MessageBox.Show("Допустимое время в неподходящей температуре не может быть отрицательным.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}