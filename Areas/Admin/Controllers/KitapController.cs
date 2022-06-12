using BLL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Drawing;
using System.IO;
using KitapSatis.Filters;

namespace KitapSatis.Areas.Admin.Controllers
{
    [AuthAdmin]
    public class KitapController : Controller
    {
        private BookManager bookmng = new BookManager();
        private AuthorManager authormng = new AuthorManager();
        private CategoryManager catmanager = new CategoryManager();
        private SubCategoryManager subcatmanager = new SubCategoryManager();
        private PublishingHouseManager publishingmng = new PublishingHouseManager();
        // GET: Admin/Kitap
        public ActionResult Index(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;
            return View(bookmng.List().OrderBy(x=>x.KitapID).ToPagedList<Kitap>(_sayfaNo, 5));
        }

        // GET: Admin/Kitap/Create
        public ActionResult Create()
        {
            ViewBag.Yazarlar = new SelectList(authormng.List(),"YazarID","YazarAdi");
            ViewBag.Kategoriler = new SelectList(catmanager.List(),"KategoriID","KategoriAdi");
            ViewBag.YayinEvleri = new SelectList(publishingmng.List(),"YayinEviID","YayinEviAdi");
            return View();
        }

        
        public JsonResult SubCategories(int id)
        {           
            var liste = subcatmanager.List(x=>x.KategoriID==id).Select(x=>new { AltKategoriID=x.AltKategoriID,AltKategoriAdi=x.AltKategoriAdi });
            return Json(liste,JsonRequestBehavior.AllowGet);
        }

        // POST: Admin/Kitap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Kitap kitap,HttpPostedFileBase file)
        {
            ViewBag.Yazarlar = new SelectList(authormng.List(), "YazarID", "YazarAdi");
            ViewBag.Kategoriler = new SelectList(catmanager.List(), "KategoriID", "KategoriAdi");
            ViewBag.YayinEvleri = new SelectList(publishingmng.List(), "YayinEviID", "YayinEviAdi");

            if (ModelState.IsValid)
            {
                var resim = kitap.Resim;
                string guid = Guid.NewGuid().ToString();
                resim = guid +"_"+ resim;
                kitap.Resim = resim;

                string yol = Path.Combine(Server.MapPath("/KitapResimleri"), resim);
                file.SaveAs(yol);

                Image image = Image.FromFile(yol);

                Image yeni = image.GetThumbnailImage(555, 600, () => false, IntPtr.Zero);
                string thumbYol = Path.Combine(Server.MapPath("/KitapResimleri"),"Thumb_"+ resim);
                yeni.Save(thumbYol);

                bookmng.Insert(kitap);
                return RedirectToAction("Index");

            }

            return View(kitap);
        }

        // GET: Admin/Kitap/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Yazarlar = new SelectList(authormng.List(), "YazarID", "YazarAdi");
            ViewBag.Kategoriler = new SelectList(catmanager.List(), "KategoriID", "KategoriAdi");
            ViewBag.YayinEvleri = new SelectList(publishingmng.List(), "YayinEviID", "YayinEviAdi");
            var kitap = bookmng.Find(x => x.KitapID == id);
            ViewBag.AltKategoriler = new SelectList(subcatmanager.List(x => x.KategoriID == kitap.KategoriID), "AltKategoriID", "AltKategoriAdi");
            
            DateTime date = kitap.YayinTarihi;
            return View(kitap);
        }

        // POST: Admin/Kitap/Edit/5
        [HttpPost]
        public ActionResult Edit(int id,Kitap kitap,HttpPostedFileBase file)
        {
            ViewBag.Yazarlar = new SelectList(authormng.List(), "YazarID", "YazarAdi");
            ViewBag.Kategoriler = new SelectList(catmanager.List(), "KategoriID", "KategoriAdi");
            ViewBag.YayinEvleri = new SelectList(publishingmng.List(), "YayinEviID", "YayinEviAdi");
            ViewBag.AltKategoriler = new SelectList(subcatmanager.List(x => x.KategoriID == kitap.KategoriID), "AltKategoriID", "AltKategoriAdi");

            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    var model = bookmng.Find(x => x.KitapID == id);
                    model.KitapAdi = kitap.KitapAdi;
                    model.Adet = kitap.Adet;
                    model.Agirlik = kitap.Agirlik;
                    model.AltKategoriID = kitap.AltKategoriID;
                    model.Dil = kitap.Dil;
                    model.Fiyat = kitap.Fiyat;
                    model.KategoriID = kitap.KategoriID;
                    model.OzetBilgi = kitap.OzetBilgi;
                    model.Resim = kitap.Resim;
                    model.SayfaSayisi = kitap.SayfaSayisi;
                    model.YayinEviID = kitap.YayinEviID;
                    model.YayinTarihi = kitap.YayinTarihi;
                    model.YazarID = kitap.YazarID;

                    bookmng.Update(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    var resim = kitap.Resim;
                    string guid = Guid.NewGuid().ToString();
                    resim = guid + "_" + resim;
                    kitap.Resim = resim;

                    string yol = Path.Combine(Server.MapPath("/KitapResimleri"), resim);
                    file.SaveAs(yol);

                    Image image = Image.FromFile(yol);

                    Image yeni = image.GetThumbnailImage(555, 600, () => false, IntPtr.Zero);
                    string thumbYol = Path.Combine(Server.MapPath("/KitapResimleri"), "Thumb_" + resim);
                    yeni.Save(thumbYol);

                    var model = bookmng.Find(x => x.KitapID == id);
                    model.KitapAdi = kitap.KitapAdi;
                    model.Adet = kitap.Adet;
                    model.Agirlik = kitap.Agirlik;
                    model.AltKategoriID = kitap.AltKategoriID;
                    model.Dil = kitap.Dil;
                    model.Fiyat = kitap.Fiyat;
                    model.KategoriID = kitap.KategoriID;
                    model.OzetBilgi = kitap.OzetBilgi;
                    model.Resim = kitap.Resim;
                    model.SayfaSayisi = kitap.SayfaSayisi;
                    model.YayinEviID = kitap.YayinEviID;
                    model.YayinTarihi = kitap.YayinTarihi;
                    model.YazarID = kitap.YazarID;

                    bookmng.Update(model);
                    return RedirectToAction("Index");
                }
            }

            return View(kitap);
        }

        // GET: Admin/Kitap/Details/5
        public ActionResult Details(int id)
        {
            var model = bookmng.Find(x => x.KitapID == id);
            return View(model);
        }

        // GET: Admin/Kitap/Delete/5
        public ActionResult Delete(int id)
        {
            var model = bookmng.Find(x => x.KitapID == id);
            bookmng.Delete(model);
            return RedirectToAction("Index");
        }

        
      
    }
}
