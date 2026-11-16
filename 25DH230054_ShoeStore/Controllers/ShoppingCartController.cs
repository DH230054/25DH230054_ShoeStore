using System;
using System.Linq;
using System.Web.Mvc;
using _25DH230054_ShoeStore.Models; // Đảm bảo đúng chữ DH viết hoa

namespace _25DH230054_ShoeStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // 1. Lấy giỏ hàng từ Session
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // 2. Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int productId, string selectedSize, string selectedColor, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product != null)
            {
                var cart = GetCart();
                cart.Add(product, selectedSize, selectedColor, quantity);
            }
            return RedirectToAction("Index");
        }

        // 3. Hiển thị trang Giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // 4. Cập nhật số lượng
        public ActionResult UpdateCart(int id, string selectedSize, string selectedColor, int quantity)
        {
            var cart = GetCart();
            cart.UpdateQuantity(id, selectedSize, selectedColor, quantity);
            return RedirectToAction("Index");
        }

        // 5. Xóa sản phẩm
        public ActionResult RemoveFromCart(int id, string selectedSize, string selectedColor)
        {
            var cart = GetCart();
            cart.Remove(id, selectedSize, selectedColor);
            return RedirectToAction("Index");
        }

        // 6. Thanh toán (GET)
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(cart);
        }

        // 7. Lưu đơn hàng vào CSDL (POST)
        [HttpPost]
        public ActionResult Checkout(string customerName, string customerPhone, string customerAddress)
        {
            var cart = GetCart();
            if (!cart.Items.Any()) return RedirectToAction("Index", "Home");

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Tạo Customer mới
                    var customer = new Customer
                    {
                        CustomerName = customerName,
                        CustomerPhone = customerPhone,
                        CustomerAddress = customerAddress
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    // Tạo Order mới
                    Order order = new Order
                    {
                        CustomerID = customer.CustomerID,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.TotalValue()
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Lưu chi tiết đơn hàng
                    foreach (var item in cart.Items)
                    {
                        OrderDetail detail = new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price
                        };
                        db.OrderDetails.Add(detail);
                    }
                    db.SaveChanges();

                    transaction.Commit();
                    cart.Clear();
                    return RedirectToAction("OrderSuccess");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Lỗi đặt hàng: " + ex.Message);
                }
            }
            return View(cart);
        }

        // 8. Trang thành công
        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}