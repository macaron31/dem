using System.Windows;

namespace WpfApp
{
    public partial class EditOrderWindow : Window
    {
        public string Status { get; private set; }

        public EditOrderWindow(string currentStatus)
        {
            InitializeComponent();
            StatusBox.Text = currentStatus;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, чтобы поле статуса не было пустым
            if (string.IsNullOrWhiteSpace(StatusBox.Text))
            {
                MessageBox.Show("Поле статуса не может быть пустым!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Status = StatusBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
