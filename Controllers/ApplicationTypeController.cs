using MansorySupplyHub.Data;
using MansorySupplyHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace MansorySupplyHub.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _db.ApplicationTypes;
            return View(objList);
        }

        //Get-CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            _db.ApplicationTypes.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("index");
        }

         //Get-EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.ApplicationTypes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _db.ApplicationTypes.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(obj);
        }


        //GET-DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.ApplicationTypes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST-DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var obj = _db.ApplicationTypes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.ApplicationTypes.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
