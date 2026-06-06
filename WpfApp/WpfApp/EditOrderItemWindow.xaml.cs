using System.Windows;

namespace WpfApp
{
    public partial class EditOrderItemWindow : Window
    {
        public int Quantity { get; private set; }
        public double Price { get; private set; }

        public EditOrderItemWindow(int qty, double price)
        {
            InitializeComponent();

            QtyBox.Text = qty.ToString();
            PriceBox.Text = price.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Безопасный парсинг количества (MessageBoxImage вместо MessageBoxIcon)
            if (!int.TryParse(QtyBox.Text, out int parsedQty) || parsedQty <= 0)
            {
                MessageBox.Show("Укажите корректное положительное количество товара!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Безопасный парсинг цены (MessageBoxImage вместо MessageBoxIcon)
            if (!double.TryParse(PriceBox.Text, out double parsedPrice) || parsedPrice < 0)
            {
                MessageBox.Show("Укажите корректную стоимость товара!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Quantity = parsedQty;
            Price = parsedPrice;

            DialogResult = true;
            Close();
        }

    }
}
