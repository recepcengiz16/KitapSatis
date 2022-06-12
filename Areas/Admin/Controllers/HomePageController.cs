using BLL;
using ENTITY;
using ENTITY.ViewObjects;
using KitapSatis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace KitapSatis.Areas.Admin.Controllers
{
    public class HomePageController : Controller
    {
        private UserManager manager = new UserManager();
        // GET: Admin/Home
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
                var uye = manager.Find(x => x.UyeKullaniciAdi == user.KullaniciAdi && x.UyeSifre == sifre);
                if (uye != null)
                {
                    CurrentSession.Set<Uye>("login", uye);
                    return Redirect("/Admin/Kategori/Index");
                }
                else
                {
                    ViewBag.Mesaj = "Kullanıcı Adı veya Şifre Hatalı";
                }
            }
            return View();
        }

        public ActionResult LogOut()
        {
            CurrentSession.Clear();
            return RedirectToAction("Login");
        }
    }
}