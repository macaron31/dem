using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private readonly CRUD crud = new CRUD();
        private int selectedOrderId = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadAll();
        }

        private void LoadAll()
        {
            PartnersGrid.ItemsSource = crud.GetSuppliers().DefaultView;
            ProductsGrid.ItemsSource = crud.GetStockItems().DefaultView;
            OrdersGrid.ItemsSource = crud.GetProcurements().DefaultView;
        }

        private void LoadOrderItems(int procurementId)
        {
            OrderItemsGrid.ItemsSource = crud.GetSupplyLines(procurementId).DefaultView;
        }

        // =====================================================
        // СЕКЦИЯ 1: КОНТРАГЕНТЫ (ПАРТНЕРЫ / ПОСТАВЩИКИ)
        // =====================================================
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            crud.AddSupplier("Новая Организация", "test@mail.com", "88005553535", true);
            LoadAll();
        }

        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)PartnersGrid.SelectedItem;
            int id = Convert.ToInt32(row["ID"]);

            // Автоматически берем имена колонок из Config!
            var win = new PartnerEditWindow(
                row[$"{Config.Table1Attribute1}"].ToString(),
                row[$"{Config.Table1Attribute2}"].ToString(),
                row[$"{Config.Table1Attribute3}"].ToString()
            );

            if (win.ShowDialog() == true)
            {
                crud.UpdateSupplier(
                    id,
                    win.Company,
                    win.Email,
                    win.Phone,
                    Convert.ToInt32(row[$"{Config.Table1Attribute4}"]) == 1
                );
                LoadAll();
            }
        }

        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)PartnersGrid.SelectedItem;
            crud.DeleteSupplier(Convert.ToInt32(row["ID"]));
            LoadAll();
        }

        private void PartnersGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditPartner_Click(sender, e);
        }

        // =====================================================
        // СЕКЦИЯ 2: НОМЕНКЛАТУРА (ТОВАРЫ / МЕДИКАМЕНТЫ)
        // =====================================================
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            crud.AddStockItem("Новая позиция", 100.0, "Базовое описание");
            LoadAll();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)ProductsGrid.SelectedItem;
            int id = Convert.ToInt32(row["ID"]);

            // Автоматически берем имена колонок из Config!
            var win = new ProductEditWindow(
                row[$"{Config.Table2Attribute1}"].ToString(),
                Convert.ToDouble(row[$"{Config.Table2Attribute2}"]),
                row[$"{Config.Table2Attribute3}"].ToString()
            );

            if (win.ShowDialog() == true)
            {
                crud.UpdateStockItem(id, win.Name, win.Price, win.Description);
                LoadAll();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)ProductsGrid.SelectedItem;
            crud.DeleteStockItem(Convert.ToInt32(row["ID"]));
            LoadAll();
        }

        private void ProductsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditProduct_Click(sender, e);
        }

        // =====================================================
        // СЕКЦИЯ 3: ЖУРНАЛ ОПЕРАЦИЙ (ЗАКАЗЫ / ЗАКУПКИ)
        // =====================================================
        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            crud.AddProcurement(1, "Новый");
            LoadAll();
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)OrdersGrid.SelectedItem;
            crud.DeleteProcurement(Convert.ToInt32(row["ID"]));
            LoadAll();
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)OrdersGrid.SelectedItem;
            selectedOrderId = Convert.ToInt32(row["ID"]);
            LoadOrderItems(selectedOrderId);
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)OrdersGrid.SelectedItem;
            int id = Convert.ToInt32(row["ID"]);

            // Автоматически берем имя колонки статуса из Config!
            string status = row[$"{Config.Table3Attribute3}"].ToString();

            var win = new EditOrderWindow(status);
            if (win.ShowDialog() == true)
            {
                crud.UpdateProcurement(id, win.Status);
                LoadAll();
            }
        }

        private void AddOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrderId == -1) return;
            if (ProductsGrid.SelectedItem == null) return;

            DataRowView row = (DataRowView)ProductsGrid.SelectedItem;

            crud.AddSupplyLine(
                selectedOrderId,
                Convert.ToInt32(row["ID"]),
                1,
                Convert.ToDouble(row[$"{Config.Table2Attribute2}"])
            );
            LoadOrderItems(selectedOrderId);
        }

        private void DeleteOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemsGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)OrderItemsGrid.SelectedItem;
            crud.DeleteSupplyLine(Convert.ToInt32(row["ID"]));
            LoadOrderItems(selectedOrderId);
        }

        private void EditOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemsGrid.SelectedItem == null) return;
            DataRowView row = (DataRowView)OrderItemsGrid.SelectedItem;

            int id = Convert.ToInt32(row["ID"]);

            // Автоматически берем имена колонок количества и цены из Config!
            int qty = Convert.ToInt32(row[$"{Config.Table4Attribute3}"]);
            double price = Convert.ToDouble(row[$"{Config.Table4Attribute4}"]);

            var win = new EditOrderItemWindow(qty, price);
            if (win.ShowDialog() == true)
            {
                crud.UpdateSupplyLine(id, win.Quantity, win.Price);
                LoadOrderItems(selectedOrderId);
            }
        }
    }
}
