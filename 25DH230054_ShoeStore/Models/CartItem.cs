namespace _25DH230054_ShoeStore.Models
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; } // Đảm bảo có thuộc tính Price
        public int Quantity { get; set; }
        public string SelectedSize { get; set; }
        public string SelectedColor { get; set; }

        public decimal TotalPrice
        {
            get { return Price * Quantity; }
        }
    }
}