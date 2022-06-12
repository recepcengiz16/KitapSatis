using BLL;
using ENTITY;
using ENTITY.ViewObjects;
using KitapSatis.Filters;
using KitapSatis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KitapSatis.Controllers
{
    public class MemberController : Controller
    {
        private UserManager manager = new UserManager();
        private BasketManager basketManager = new BasketManager();
        private OrderManager orderManager = new OrderManager();
        private OrderDetailsManager orderDetails = new OrderDetailsManager();
        private BookManager bookManager = new BookManager();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(VMLoginUser user)
        {
            if (ModelState.IsValid)
            {
                string sifre = manager.Md5(user.Sifre);
                var uye = manager.Find(x => x.UyeKullaniciAdi == user.KullaniciAdi && x.UyeSifre == sifre && x.RolID==2);
                if (uye != null)
                {
                    CurrentSession.Set<Uye>("login", uye);
                    return Redirect("/Home/Books");
                }
                else
                {
                    ViewBag.Mesaj = "Kullanıcı Adı veya Şifre Hatalı";
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Uye uye)
        {
            if (ModelState.IsValid)
            {
                string sifre = uye.UyeSifre;
                string kod = manager.Md5(sifre);
                uye.UyeSifre = kod;
                uye.UyeSifreTekrar = kod;

                manager.Insert(uye);
                return Redirect("/Home/Index");
            }
            return View();
        }

        [Auth]
        public ActionResult LogOut()
        {
            CurrentSession.Clear();
            return RedirectToAction("Login");
        }

        [Auth]
        public ActionResult Profil(int id)
        {
            var model=manager.Find(x => x.UyeID == id);
            string sifre = manager.Decrypt(model.UyeSifre);
            model.UyeSifre = sifre;
            model.UyeSifreTekrar = sifre;
            return View(model);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profil(int id,Uye uye)
        {
            if (ModelState.IsValid)
            {
                string sifre = manager.Md5(uye.UyeSifre);

                var model = manager.Find(x => x.UyeID == id);
                model.UyeAdi = uye.UyeAdi;
                model.UyeSoyadi = uye.UyeSoyadi;
                model.RolID = uye.RolID;
                model.UyeAdres = uye.UyeAdres;
                model.UyeEposta = uye.UyeEposta;
                model.UyeKullaniciAdi = uye.UyeKullaniciAdi;
                model.UyeSifre = sifre;
                model.UyeSifreTekrar = sifre;
                model.UyeTelefon = uye.UyeTelefon;

                manager.Update(model);
                ViewBag.Mesaj = "Başarılı bir şekilde güncellendi.";
            }
            else
            {
                ViewBag.Mesaj = "Güncelleme başarısız.";
            }

            return View(uye);
        }

        [Auth]
        public ActionResult Basket()
        {
            var sepet = basketManager.Find(x => x.UyeID == CurrentSession.User.UyeID).Kitaplar;
            return View(sepet);
        }

        [Auth]
        public ActionResult AddToBasket(int id)
        {
            var model = bookManager.Find(x => x.KitapID == id);

            var sepet = basketManager.Find(x => x.UyeID == CurrentSession.User.UyeID);

            if (sepet==null)
            {
                Sepet yeniSepet = new Sepet();
                yeniSepet.UyeID = CurrentSession.User.UyeID;

                VMSepetUrun sepetUrun = new VMSepetUrun();
                sepetUrun.Kitap = model;
                sepetUrun.Adet = 1;

                yeniSepet.Kitaplar.Add(sepetUrun);
                basketManager.Insert(yeniSepet);
                model.Adet -= 1;
                return RedirectToAction("Books","Home");
            }
            else
            {
                var kitap = sepet.Kitaplar.FirstOrDefault(x => x.Kitap.KitapID == model.KitapID);
                if (kitap!=null)
                {
                    kitap.Adet += 1;
                    basketManager.Update(sepet);
                }
                else
                {
                    VMSepetUrun sepetUrun = new VMSepetUrun();
                    sepetUrun.Kitap = model;
                    sepetUrun.Adet = 1;
                    sepet.Kitaplar.Add(sepetUrun);
                    basketManager.Update(sepet);
                    return RedirectToAction("Books", "Home");
                }
            }


            return RedirectToAction("Books", "Home");
        }

        [Auth]
        public ActionResult DeleteFromBasket(int id)
        {
            var sepet = basketManager.Find(x => x.UyeID == CurrentSession.User.UyeID);
            var kitap = sepet.Kitaplar.FirstOrDefault(x => x.Kitap.KitapID == id);
            sepet.Kitaplar.Remove(kitap);

            basketManager.Update(sepet);

            return RedirectToAction("Basket");
        }

        [Auth]
        public ActionResult UpdateBasket(int id,int yon)
        {
            var sepet = basketManager.Find(x => x.UyeID == CurrentSession.User.UyeID);
            if (yon==1)
            {
                sepet.Kitaplar.FirstOrDefault(x => x.Kitap.KitapID == id).Adet++;
            }else if (yon == 0)
            {
                if (sepet.Kitaplar.FirstOrDefault(x => x.Kitap.KitapID == id).Adet>1)
                {
                    sepet.Kitaplar.FirstOrDefault(x => x.Kitap.KitapID == id).Adet--;
                }
                else
                {
                    DeleteFromBasket(id);
                }
            }
            basketManager.Update(sepet);

            return RedirectToAction("Basket");
        }

        [Auth]
        public ActionResult GiveOrder()
        {
            var sepet = basketManager.Find(x => x.UyeID == CurrentSession.User.UyeID);

            Siparis siparis = new Siparis();
            siparis.UyeID = CurrentSession.User.UyeID;

            decimal fiyat = 0;
            foreach (var item in sepet.Kitaplar)
            {
                fiyat += item.Kitap.Fiyat * item.Adet;
            }
            siparis.GenelTutar = fiyat;
            orderManager.Insert(siparis);

            
            VMSiparisler vmSiparisler = new VMSiparisler();
            foreach (var item in sepet.Kitaplar)
            {
                SiparisDetay siparisDetay = new SiparisDetay();
                siparisDetay.BirimFiyat = item.Kitap.Fiyat;
                siparisDetay.KitapAdet = item.Adet;
                siparisDetay.SiparisID = siparis.SiparisID;
                siparisDetay.KitapID = item.Kitap.KitapID;
                orderDetails.Insert(siparisDetay);

                vmSiparisler.Siparis = siparis;
                vmSiparisler.SiparisDetay.Add(siparisDetay);
            }

            var uye=manager.Find(x => x.UyeID == CurrentSession.User.UyeID);
            uye.Siparisler.Add(siparis);

            sepet.Kitaplar.Clear();

            return View(vmSiparisler);
        }

        [Auth]
        public ActionResult Orders()
        {
            var uye = manager.Find(x => x.UyeID == CurrentSession.User.UyeID);
            return View(uye.Siparisler);
        }

        [Auth]
        public ActionResult OrderDetails(int id)
        {
            var details=orderDetails.List(x => x.SiparisID == id);
            return View(details);
        }
    }
}