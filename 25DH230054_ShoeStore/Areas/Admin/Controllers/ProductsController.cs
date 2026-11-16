using _25DH230054_ShoeStore.Models;
using _25DH230054_ShoeStore.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _25Dh230054_ShoeStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // 1. INDEX: Danh sách + Tìm kiếm + Lọc giá + Sắp xếp + Phân trang
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            model.SearchTerm = searchTerm;
            model.MinPrice = minPrice;
            model.MaxPrice = maxPrice;
            model.SortOrder = sortOrder;

            var products = db.Products.Include(p => p.Category).AsQueryable();

            // Trong ProductsController.cs, dòng tìm kiếm sửa lại chữ ProductDecription cho khớp:
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDecription.Contains(searchTerm));
                                           
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue) products = products.Where(p => p.ProductPrice >= minPrice.Value);
            if (maxPrice.HasValue) products = products.Where(p => p.ProductPrice <= maxPrice.Value);

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_asc": products = products.OrderBy(p => p.ProductName); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductID); break;
            }

            int pageSize = 5; // Số sản phẩm trên 1 trang
            int pageNumber = (page ?? 1);
            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // 2. DETAILS: Xem chi tiết sản phẩm
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // 3. CREATE (GET): Hiển thị form thêm mới
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // CREATE (POST): Xử lý lưu sản phẩm kèm Upload ảnh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,Sizes,Colors,Brand")] Product product, HttpPostedFileBase UploadImg)
        {
            if (ModelState.IsValid)
            {
                if (UploadImg != null && UploadImg.ContentLength > 0)
                {
                    string filename = Path.GetFileName(UploadImg.FileName);
                    string savePath = "~/Content/images/";
                    string physicalPath = Server.MapPath(savePath);

                    if (!Directory.Exists(physicalPath))
                    {
                        Directory.CreateDirectory(physicalPath);
                    }

                    UploadImg.SaveAs(Path.Combine(physicalPath, filename));
                    product.ProductImage = savePath + filename;
                }
                else
                {
                    product.ProductImage = "~/Content/images/default_img.png";
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // 4. EDIT (GET): Hiển thị form chỉnh sửa
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // EDIT (POST): Xử lý cập nhật thông tin và ảnh mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,Sizes,Colors,Brand")] Product product, HttpPostedFileBase UploadImg)
        {
            if (ModelState.IsValid)
            {
                if (UploadImg != null && UploadImg.ContentLength > 0)
                {
                    string filename = Path.GetFileName(UploadImg.FileName);
                    string savePath = "~/Content/images/";
                    string physicalPath = Server.MapPath(savePath);

                    UploadImg.SaveAs(Path.Combine(physicalPath, filename));
                    product.ProductImage = savePath + filename;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // 5. DELETE (GET): Xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // DELETE (POST): Thực hiện xóa khỏi Database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}