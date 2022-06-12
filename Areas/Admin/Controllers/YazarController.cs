using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using KitapSatis.Filters;

namespace KitapSatis.Areas.Admin.Controllers
{
    [AuthAdmin]
    public class YazarController : Controller
    {
        private AuthorManager manager = new AuthorManager();
        // GET: Admin/Yazar
        public ActionResult Index(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;
            return View(manager.List().OrderBy(x=>x.YazarAdi).ToPagedList<Yazar>(_sayfaNo,5));
        }


        // GET: Admin/Yazar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Yazar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Yazar yazar)
        {
            if (ModelState.IsValid)
            {
                manager.Insert(yazar);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Admin/Yazar/Edit/5
        public ActionResult Edit(int id)
        {
            var yazar = manager.Find(x => x.YazarID == id);
            return View(yazar);
        }

        // POST: Admin/Yazar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Yazar yazar)
        {
            if (ModelState.IsValid)
            {
                var model = manager.Find(x => x.YazarID == id);
                model.YazarAdi = yazar.YazarAdi;
                manager.Update(model);
                return RedirectToAction("Index");
            }
            return View(yazar);
        }

        // GET: Admin/Yazar/Delete/5
        public ActionResult Delete(int id)
        {
            var model = manager.Find(x => x.YazarID == id);
            manager.Delete(model);
            return RedirectToAction("Index");
        }

        public ActionResult BooksofAuthor()
        {
            return View();
        }
       
    }
}
