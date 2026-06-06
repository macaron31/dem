using System.Windows;

namespace WpfApp
{
    public partial class PartnerEditWindow : Window
    {
        // Безопасное хранение данных после закрытия окна
        public string Company { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public PartnerEditWindow(string company, string email, string phone)
        {
            InitializeComponent();

            CompanyBox.Text = company;
            EmailBox.Text = email;
            PhoneBox.Text = phone;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем заполнение ключевого поля
            if (string.IsNullOrWhiteSpace(CompanyBox.Text))
            {
                MessageBox.Show("Наименование организации должно быть заполнено!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Фиксируем значения перед закрытием окна
            Company = CompanyBox.Text;
            Email = EmailBox.Text;
            Phone = PhoneBox.Text;

            DialogResult = true;
            Close();
        }
    }
}
