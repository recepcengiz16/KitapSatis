using BLL;
using ENTITY;
using KitapSatis.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KitapSatis.Areas.Admin.Controllers
{
    [AuthAdmin]
    public class RolController : Controller
    {
        private RoleManager manager = new RoleManager();
        // GET: Admin/Rol
        public ActionResult Index()
        {
            return View(manager.List());
        }
     

        // GET: Admin/Rol/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Rol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rol rol)
        {
            if (ModelState.IsValid)
            {
                manager.Insert(rol);
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Admin/Rol/Edit/5
        public ActionResult Edit(int id)
        {
            var rol = manager.Find(x => x.RolID == id);
            return View(rol);
        }

        // POST: Admin/Rol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Rol rol)
        {
            if (ModelState.IsValid)
            {
                var model = manager.Find(x => x.RolID == id);
                model.RolAdi = rol.RolAdi;
                manager.Update(model);
                return RedirectToAction("Index");
            }
            return View(rol);
        }

        // GET: Admin/Rol/Delete/5
        public ActionResult Delete(int id)
        {
            var rol = manager.Find(x => x.RolID == id);
            manager.Delete(rol);
            return RedirectToAction("Index");
        }

        
    }
}
