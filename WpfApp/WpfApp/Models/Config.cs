namespace WpfApp.Models
{
    internal class Config
    {
        //Лютый говнокод ;)

        public static string DataBaseName = "DataBase.sqlite"; //DataBase Name


        public static string Table0Name = "Users"; //Table name

        public static string Table0Attribute1 = "Name"; //text
        public static string Table0Attribute2 = "Login"; //text
        public static string Table0Attribute3 = "Password"; //real


        public static string Table1Name = "Partners"; //Table name

        public static string Table1Attribute1 = "company_name"; //text
        public static string Table1Attribute2 = "contact_email"; //text
        public static string Table1Attribute3 = "phone_number"; //text
        public static string Table1Attribute4 = "is_active"; //bool


        public static string Table2Name = "Products"; //Table name

        public static string Table2Attribute1 = "product_name"; //text
        public static string Table2Attribute2 = "price"; //real
        public static string Table2Attribute3 = "description"; //text


        public static string Table3Name = "Orders"; //Table name

        public static string Table3Attribute1 = "partner_id"; //int
        public static string Table3Attribute2 = "order_date"; //date
        public static string Table3Attribute3 = "status"; //text
        public static string Table3Attribute4 = "updated_at"; //date


        public static string Table4Name = "OrderItems"; //Table name

        public static string Table4Attribute1 = "order_id"; //int
        public static string Table4Attribute2 = "product_id"; //int
        public static string Table4Attribute3 = "quantity"; //int
        public static string Table4Attribute4 = "price_at_order"; //real

    }
}
