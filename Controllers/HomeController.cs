using BLL;
using ENTITY;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KitapSatis.Controllers
{
    public class HomeController : Controller
    {
        private PublishingHouseManager publishingHouseManager = new PublishingHouseManager();
        private AuthorManager authorManager = new AuthorManager();
        private CategoryManager categoryManager = new CategoryManager();
        private SubCategoryManager subCategoryManager = new SubCategoryManager();
        private BookManager bookManager = new BookManager();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Books(int? sayfaNo,int? id,string search)
        {
            ViewBag.Kategoriler = categoryManager.List();


            int _sayfaNo = sayfaNo ?? 1;
            if (id!=null)
            {
                return View(bookManager.List(x=>x.AltKategoriID==id).OrderBy(x=>x.KitapID).ToPagedList<Kitap>(_sayfaNo,9));
            }
            else if (!String.IsNullOrEmpty(search))
            {
                return View(bookManager.List(x => x.Yazar.YazarAdi.Contains(search) || x.YayinEvi.YayinEviAdi.Contains(search) || x.KitapAdi.Contains(search)).OrderBy(x=>x.KitapID).ToPagedList<Kitap>(_sayfaNo,9));
            }
            return View(bookManager.List(x=>x.Adet>0).OrderBy(x=>x.KitapID).ToPagedList<Kitap>(_sayfaNo,9));
        }

        public ActionResult Authors(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;

            var apiUrl = "http://localhost:52773/api/Member";

            //Connect API
            Uri url = new Uri(apiUrl);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string json = client.DownloadString(url);
            //END

            //JSON Parse START
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<Yazar> jsonList = ser.Deserialize<List<Yazar>>(json);
            //END

            return View(jsonList.OrderBy(x => x.YazarAdi).ToPagedList<Yazar>(_sayfaNo, 5));
        }

        public ActionResult PublishingHomes(int? sayfaNo)
        {
            int _sayfaNo = sayfaNo ?? 1;

            var apiUrl = "http://localhost:52773/api/Member/PublishingHomes";

            //Connect API
            Uri url = new Uri(apiUrl);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string json = client.DownloadString(url);
            //END

            //JSON Parse START
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<YayinEvi> jsonList = ser.Deserialize<List<YayinEvi>>(json);
            //END

            return View(jsonList.OrderBy(x => x.YayinEviAdi).ToPagedList<YayinEvi>(_sayfaNo, 5));
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult BookDetails(int id)
        {
            var model = bookManager.Find(x => x.KitapID == id);
            return View(model);
        }
    }
}