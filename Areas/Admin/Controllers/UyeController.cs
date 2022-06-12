using BLL;
using ENTITY;
using KitapSatis.Filters;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KitapSatis.Areas.Admin.Controllers
{
    //[Authorize] //yetkisi olmayan giremez.
    [AuthAdmin]
    public class UyeController : Controller
    {
        private UserManager manager = new UserManager();
        private RoleManager rolemng = new RoleManager();
        // GET: Admin/Uye Listesi
        public ActionResult Index(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;
            return View(manager.List(x=>x.RolID==2).OrderBy(x => x.UyeAdi).ToPagedList<Uye>(_sayfaNo, 5));
        }

        public ActionResult AdminList()
        {
            return View(manager.List(x=>x.RolID==1));
        }

        public ActionResult Create()
        {
            ViewBag.RolID = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Uye uye)
        {
            ViewBag.RolID = 1;
            if (ModelState.IsValid)
            {
                string sifre = uye.UyeSifre;
                string kod = manager.Md5(sifre);
                uye.UyeSifre = kod;
                uye.UyeSifreTekrar = kod;

                manager.Insert(uye);
                return RedirectToAction("AdminList");
            }
            else
            {
                return View(uye);
            }
        }

        //GET: Admin/Uye/Edit/5
        public ActionResult Edit(int id)
        {
            var model = manager.Find(x => x.UyeID == id);
            string kod = model.UyeSifre;
            string sifre = manager.Decrypt(kod);
           // ViewBag.RolID = model.RolID;
            model.UyeSifre = sifre;
            model.UyeSifreTekrar = sifre;
            return View(model);
        }

       // POST: Admin/Uye/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Uye uye)
        {            
            if (ModelState.IsValid)
            {
                string sifre = manager.Md5(uye.UyeSifre);
                var model = manager.Find(x => x.UyeID == id);
                //ViewBag.RolID = model.RolID;
                model.UyeAdi = uye.UyeAdi;
                model.UyeSoyadi = uye.UyeSoyadi;
                model.UyeKullaniciAdi = uye.UyeKullaniciAdi;
                model.UyeSifre = sifre;
                model.UyeSifreTekrar = sifre;
                model.UyeEposta = uye.UyeEposta;
                model.UyeAdres = uye.UyeAdres;
                model.UyeTelefon = uye.UyeTelefon;
                model.RolID = uye.RolID;
                manager.Update(model);

                ViewBag.Mesaj="Başarılı bir şekilde güncellendi";
            }
            else
            {
                ViewBag.Mesaj = "Güncelleme başarısız";
            }
            return View(uye);
        }

        // GET: Admin/Uye/Delete/5
        public ActionResult Delete(int id)
        {
            var model = manager.Find(x => x.UyeID == id);
            manager.Delete(model);
            return RedirectToAction("Index");
        }       

    }
}
