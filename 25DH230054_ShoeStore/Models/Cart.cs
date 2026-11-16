using System.Collections.Generic;
using System.Linq;

namespace _25DH230054_ShoeStore.Models
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        // Thêm sản phẩm vào giỏ (gộp nếu trùng ID, Size, Màu)
        public void Add(Product product, string size, string color, int quantity = 1)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == product.ProductID && i.SelectedSize == size && i.SelectedColor == color);
            if (existingItem == null)
            {
                items.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage,
                    Price = product.ProductPrice,
                    Quantity = quantity,
                    SelectedSize = size,
                    SelectedColor = color
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        // Cập nhật số lượng
        public void UpdateQuantity(int productId, string size, string color, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId && i.SelectedSize == size && i.SelectedColor == color);
            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    items.Remove(item);
            }
        }

        // Xóa sản phẩm khỏi giỏ
        public void Remove(int productId, string size, string color)
        {
            items.RemoveAll(i => i.ProductID == productId && i.SelectedSize == size && i.SelectedColor == color);
        }

        // Tính tổng tiền giỏ hàng
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Xóa sạch giỏ hàng sau khi đặt hàng
        public void Clear()
        {
            items.Clear();
        }
    }
}