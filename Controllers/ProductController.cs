using MansorySupplyHub.Data;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products;
            foreach (var obj in objList)
            {
                obj.Category = _db.Categories.FirstOrDefault(u => u.id == obj.CategoryId);
            };
           
            return View(objList);
        }

        //Get-UPSERT(Creating a get for create and updates)
        public IActionResult Upsert(int? Id)
        {
            Product product = new Product();
            if (Id == null)
            {
                //this is create
                return View(product);
            }
            else
            {
                product = _db.Products.Find(Id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }

        ////POST-UPSERT
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert(Category obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Categories.Add(obj);
        //        _db.SaveChanges();
        //        return RedirectToAction("index");
        //    }
        //    return View(obj);
        //}

        ////GET-DELETE
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    var obj = _db.Categories.Find(id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(obj);
        //}

        ////POST-DELETE
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(int? id)
        //{
        //    var obj = _db.Categories.Find(id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    _db.Categories.Remove(obj);
        //    _db.SaveChanges();
        //    return RedirectToAction("index");
        //}
    }
}
