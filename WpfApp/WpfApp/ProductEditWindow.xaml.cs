using System;
using System.Windows;

namespace WpfApp
{
    public partial class ProductEditWindow : Window
    {
        // Надежное и безопасное хранение извлеченных данных
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }

        public ProductEditWindow(string name, double price, string desc)
        {
            InitializeComponent();

            NameBox.Text = name;
            PriceBox.Text = price.ToString();
            DescBox.Text = desc;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, чтобы название товара не было пустым
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Наименование позиции должно быть обязательно заполнено!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Безопасно переводим текст цены в число
            if (!double.TryParse(PriceBox.Text, out double parsedPrice) || parsedPrice < 0)
            {
                MessageBox.Show("Укажите корректную положительную стоимость товара!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохраняем проверенные данные в свойства
            Name = NameBox.Text;
            Price = parsedPrice;
            Description = DescBox.Text;

            DialogResult = true;
            Close();
        }
    }
}
