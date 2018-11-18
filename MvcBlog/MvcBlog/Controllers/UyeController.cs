using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcBlog.Models;
namespace MvcBlog.Controllers
{
    
    public class UyeController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        
        public ActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Login(Uye uye)
        {
            
            var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            if (login.KullaniciAdi == uye.KullaniciAdi && login.email == uye.email && login.sifre == uye.sifre)
            {
                Session["uyeid"] = login.UyeId;
                Session["kullaniciadi"] = login.KullaniciAdi;
                Session["yetkiid"] = login.YetkiId;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Uyari = "Kullanıcı adı, mail ya da şifrenizi kontrol ediniz!";
                return View();
            }
            
        }
        public ActionResult Logout()
        {
            Session["uyeid"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");

        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Uye uye, HttpPostedFileBase Fotograf)
        {
            if (ModelState.IsValid)
            {
                if (Fotograf != null)
                {
                    WebImage img = new WebImage(Fotograf.InputStream);
                    FileInfo fotografinfo = new FileInfo(Fotograf.FileName);
                    string newfotograf = Guid.NewGuid().ToString() + fotografinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/uyeFotograf/" + newfotograf);
                    uye.Fotograf = "/Uploads/UyeFotograf/" + newfotograf;
                    uye.YetkiId = 2;
                    db.Uyes.Add(uye);
                    db.SaveChanges();
                    Session["uyeid"] = uye.UyeId;
                    Session["kullaniciadi"] = uye.KullaniciAdi;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Fotoğraf", "Fotoğraf Seçiniz.");
                }
            }
            return View(uye);
        }
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye, int id, HttpPostedFileBase Fotograf)
        {
            if (ModelState.IsValid)
            {
                var uyes = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
                if (Fotograf != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uye.Fotograf)))
                    {
                        System.IO.File.Delete(Server.MapPath(uye.Fotograf));
                    }
                    WebImage img = new WebImage(Fotograf.InputStream);
                    FileInfo fotografinfo = new FileInfo(Fotograf.FileName);
                    string newfotograf = Guid.NewGuid().ToString() + fotografinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFotograf/" + newfotograf);
                    uye.Fotograf = "/Uploads/UyeFotograf/" + newfotograf;

                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.KullaniciAdi = uye.KullaniciAdi;
                uyes.sifre = uye.sifre;
                uyes.email = uye.email;
                db.SaveChanges();
                Session["kullaniciadi"] = uye.KullaniciAdi;
                return RedirectToAction("Index", "Home", new { id = uyes.UyeId });

                
            }
            return View();
        }
        public ActionResult UyeProfil(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            return View(uye);
        }
    }
}