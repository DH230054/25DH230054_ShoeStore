using System.Linq;
using System.Web.Mvc;
using _25DH230054_ShoeStore.Models;
using _25DH230054_ShoeStore.Models.ViewModel;
using PagedList;

namespace _25DH230054_ShoeStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // 1. TRANG CHỦ: Hiển thị danh sách giày cho khách mua sắm + Tìm kiếm + Phân trang
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, int? page)
        {
            var model = new ProductSearchVM();
            model.SearchTerm = searchTerm;
            model.MinPrice = minPrice;
            model.MaxPrice = maxPrice;

            var products = db.Products.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDecription.Contains(searchTerm));
            }

            // Lọc giá
            if (minPrice.HasValue) products = products.Where(p => p.ProductPrice >= minPrice.Value);
            if (maxPrice.HasValue) products = products.Where(p => p.ProductPrice <= maxPrice.Value);

            int pageSize = 8; // Hiển thị 8 sản phẩm mỗi trang
            int pageNumber = (page ?? 1);
            model.Products = products.OrderBy(p => p.ProductID).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // 2. CHI TIẾT SẢN PHẨM: Cho khách chọn Size, Màu trước khi mua
        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }
    }
}