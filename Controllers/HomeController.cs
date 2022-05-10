using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheOceanWeb.Models;

namespace TheOceanWeb.Controllers
{
    public class HomeController : Controller
    {
        //建立可存取資料庫的 (資料庫名稱)Entities 類別物件 db
        TheOceanDbEntities db = new TheOceanDbEntities();

        public ActionResult home()
        {
            //取得所有產品放入products
            //var products = db.tProduct
            //    .OrderByDescending(m => m.fId).ToList();

            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View();
            }

            return View("../home/home", "_LayoutMember");
        }

        public ActionResult product()
        {
            //取得所有產品放入products
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();
            return View(products);
        }
        public ActionResult Set()
        {
            //取得所有產品放入products
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();

            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View(products);
            }
            return View("../home/set", "_LayoutMember", products);
        }

        public ActionResult skimmer()
        {
            //取得所有產品放入products
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();

            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View(products);
            }
            return View("../home/skimmer", "_LayoutMember", products);
        }
        
        public ActionResult reeflight()
        {
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();
            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View(products);
            }
            return View("../home/reeflight", "_LayoutMember", products);
        }
        public ActionResult others()
        {
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();
            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View(products);
            }
            return View("../home/others", "_LayoutMember", products);
        }
        public ActionResult detail()
        {
            var products = db.tProduct.Find(35);
            //判斷是否登入狀態
            if (Session["loginUser"] == null)
            {
                return View(products);
            }
            return View("../home/detail", "_LayoutMember", products);
        }
    }
}