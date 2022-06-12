using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using KitapSatis.Filters;

namespace KitapSatis.Areas.Admin.Controllers
{
    [AuthAdmin]
    public class KategoriController : Controller
    {
        // GET: Admin/Kategori
        private CategoryManager manager = new CategoryManager();
        private SubCategoryManager submanager = new SubCategoryManager();
        public ActionResult Index(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;
            return View(manager.List().OrderBy(x=>x.KategoriAdi).ToPagedList<Kategori>(_sayfaNo,5));
        }       

        // GET: Admin/Kategori/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Kategori/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                manager.Insert(kategori);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Admin/Kategori/Edit/5
        public ActionResult Edit(int id)
        {
            var kategori=manager.Find(x => x.KategoriID == id);
            
            return View(kategori);
        }

        // POST: Admin/Kategori/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                Kategori kat=manager.Find(x => x.KategoriID == id);
                kat.KategoriAdi = kategori.KategoriAdi;
                manager.Update(kat);
                return RedirectToAction("Index");
            }
            return View(kategori);
        }

        // GET: Admin/Kategori/Delete/5
        public ActionResult Delete(int id)
        {
            manager.Delete(manager.Find(x=>x.KategoriID==id));
            return RedirectToAction("Index");
        }

        public ActionResult SubCategories(int id)
        {
            ViewBag.KategoriID = id;
            var altKategoriler = submanager.List(x => x.KategoriID == id);
            manager.Find(x => x.KategoriID == id).AltKategoriler = altKategoriler;
            return View(altKategoriler);
        }

        public ActionResult SubCategoryCreate(int id)
        {
            ViewBag.KategoriID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubCategoryCreate(int id,AltKategori altKategori)
        {
            altKategori.KategoriID = id;
            submanager.Insert(altKategori);
            manager.Find(x => x.KategoriID == id).AltKategoriler.Add(altKategori);
            return RedirectToAction("Index");
        }

        public ActionResult SubCategoryEdit(int id)
        {
            var altkategori = submanager.Find(x => x.AltKategoriID == id);
            return View(altkategori);
        }

        [HttpPost]
        public ActionResult SubCategoryEdit(int id,AltKategori altKategori)
        {
            if (ModelState.IsValid)
            {
                var altKat=submanager.Find(x => x.AltKategoriID == id);
                altKat.AltKategoriAdi = altKategori.AltKategoriAdi;
                submanager.Update(altKat);
                return RedirectToAction("Index");
            }
            return View(altKategori);
        }

        public ActionResult SubCategoryDelete(int id)
        {
            var altkat = submanager.Find(x => x.AltKategoriID == id);
            submanager.Delete(altkat);
            return RedirectToAction("Index");
        }
    }
}
