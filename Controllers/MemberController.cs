using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheOceanWeb.Models;


namespace TheOceanWeb.Controllers
{
    public class MemberController : Controller
    {
        //建立可存取資料庫的 (資料庫名稱)Entities 類別物件 db
        TheOceanDbEntities db = new TheOceanDbEntities();

        // GET: Member/Index
        public ActionResult Index()
        {
            //取得所有產品放入products
            var products = db.tProduct
                .OrderByDescending(m => m.fId).ToList();
            return View(products);
        }

        //Get: Member/Login
        public ActionResult Login()
        {
            return View();
        }

        //Post: Member/Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var shaPwd = sha256.ComputeHash(Encoding.UTF8.GetBytes(fPwd));
            fPwd = BitConverter.ToString(shaPwd).Replace("-", string.Empty);
            

            // 依帳密取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                .FirstOrDefault();

            //若member為null，表示會員未註冊
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }


            //判定是否為店長admin登入
            if(member.fUserId == "admin")
            {
                //使用Session變數紀錄歡迎詞
                Session["Welcome"] = "您好" + member.fName + "店長";
                string roles = "admin";
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    member.fName,
                    DateTime.Now,
                    DateTime.Now.AddYears(20),
                    false,
                    roles //寫入使用者角色
                    );
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                //RedirectToAction({action},{controller})
                return RedirectToAction("Index", "Admin");
            }


            //判斷使用者是否登入

            //使用Session變數紀錄歡迎詞
            Session["Welcome"] = "您好" + member.fName;
            //記錄當前登入的使用者名稱
            Session["loginUser"] = member.fUserId;


            //抽獎次數
            Session["drawOnce"] = "1";



            //指定使用者帳號通過登錄驗證
            FormsAuthentication.RedirectFromLoginPage(fUserId, true);
            return RedirectToAction("home", "Shopping");
        }

        //Get:Home/Register
        public ActionResult Register()
        {
            return View();
        }

        //Post:Home/Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            //若模型沒有通過驗證則顯示目前的View
            //if (ModelState.IsValid == false)
            //{
            //    return View();
            //}
            // 依帳號取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == pMember.fUserId)
                .FirstOrDefault();

            //Add 註冊成功頁面
            //若member為null，表示會員未註冊
            if (member == null)
            {

                var sha256 = System.Security.Cryptography.SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(pMember.fPwd));

                pMember.fPwd = BitConverter.ToString(hash).Replace("-", string.Empty);


                //將會員記錄新增到tMember資料表
                db.tMember.Add(pMember);
                db.SaveChanges();

                Session["registerSuccess"] = "註冊成功";
                TempData["regSuccess"] = "註冊成功";

                //執行Home控制器的Login動作方法
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";

            return View();
        }

    }
}
