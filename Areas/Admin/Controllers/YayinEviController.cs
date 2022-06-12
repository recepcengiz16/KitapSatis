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
    public class YayinEviController : Controller
    {
        private PublishingHouseManager manager = new PublishingHouseManager();
        // GET: Admin/YayinEvi
        public ActionResult Index(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;
            return View(manager.List().OrderBy(x=>x.YayinEviAdi).ToPagedList<YayinEvi>(_sayfaNo,5));
        }

      
        // GET: Admin/YayinEvi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/YayinEvi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(YayinEvi yayinEvi)
        {
            if (ModelState.IsValid)
            {
                manager.Insert(yayinEvi);
                return RedirectToAction("Index");
            }
            return View(yayinEvi);
        }

        // GET: Admin/YayinEvi/Edit/5
        public ActionResult Edit(int id)
        {
            var model = manager.Find(x => x.YayinEviID == id);
            return View(model);
        }

        // POST: Admin/YayinEvi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, YayinEvi yayinEvi)
        {
            if (ModelState.IsValid)
            {
                var model = manager.Find(x => x.YayinEviID == id);
                model.YayinEviAdi = yayinEvi.YayinEviAdi;
                manager.Update(model);
                return RedirectToAction("Index");
            }
            return View(yayinEvi);
        }

        // GET: Admin/YayinEvi/Delete/5
        public ActionResult Delete(int id)
        {
            var model = manager.Find(x => x.YayinEviID == id);
            manager.Delete(model);
            return RedirectToAction("Index");
        }

       
    }
}
