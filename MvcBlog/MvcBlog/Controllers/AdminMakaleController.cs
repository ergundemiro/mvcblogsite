using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcBlog.Models;
using PagedList;
using PagedList.Mvc;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB db = new mvcblogDB();
        // GET: AdminMakale
        public ActionResult Index(int Page=1)
        {
            var makales = db.Makales.OrderByDescending(m=>m.MakaleId).ToPagedList(Page, 10);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris,"KategoriId","KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale,string etiketler,HttpPostedFileBase Fotograf)
        {
            try
            {
                if (Fotograf != null)
                {
                    WebImage img = new WebImage(Fotograf.InputStream);
                    FileInfo fotografinfo = new FileInfo(Fotograf.FileName);
                    string newfotograf = Guid.NewGuid().ToString() + fotografinfo.Extension;
                    img.Resize(600, 300);
                    img.Save("~/Uploads/MakaleFotograf/" + newfotograf);
                    makale.Fotograf = "/Uploads/MakaleFotograf/" + newfotograf;

                }
                if(etiketler != null )
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach(var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yenietiket);
                        makale.Etikets.Add(yenietiket);
                    }
                }
                makale.UyeId = Convert.ToInt32(Session["uyeid"]);
                makale.Okunma = 1;
                db.Makales.Add(makale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(makale);
            }
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();

            }
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Fotograf, Makale makale)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (Fotograf != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Fotograf)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Fotograf));
                    }
                    WebImage img = new WebImage(Fotograf.InputStream);
                    FileInfo fotografinfo = new FileInfo(Fotograf.FileName);
                    string newfotograf = Guid.NewGuid().ToString() + fotografinfo.Extension;
                    img.Resize(600, 300);
                    img.Save("~/Uploads/MakaleFotograf/" + newfotograf);
                    makales.Fotograf = "/Uploads/MakaleFotograf/" + newfotograf;
                    makales.Baslik = makale.Baslik;
                    makales.Icerik = makale.Icerik;
                    makales.KategoriId = makale.KategoriId;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();

            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Fotograf)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Fotograf));
                }
                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
