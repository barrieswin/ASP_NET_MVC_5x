using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheOceanWeb.Models;

namespace TheOceanWeb.Controllers
{

    [Authorize]  //指定Member控制器所有的動作方法必須通過授權才能執行。

    public class ShoppingController : Controller
    {

        //建立可存取資料庫的 (資料庫名稱)Entities 類別物件 db
        TheOceanDbEntities db = new TheOceanDbEntities();


        //Index, home, skimmer 要套用 LayoutMember

        // GET: Shopping/Index
        public ActionResult Index()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Member/Index", "_LayoutMember", products);
        }

        // GET: Shopping/home return redirect
        public ActionResult home()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/home", "_LayoutMember", products);
        }

        // GET: Shopping/set return redirect
        public ActionResult set()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/set", "_LayoutMember", products);
        }

        // GET: Shopping/skimmer return redirect
        public ActionResult skimmer()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/skimmer", "_LayoutMember", products);
        }

        // GET: Shopping/reeflight return redirect
        public ActionResult reeflight()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/reeflight", "_LayoutMember", products);
        }

        // GET: Shopping/others return redirect
        public ActionResult others()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/others", "_LayoutMember", products);
        }

        
        // GET: Shopping/product return redirect
        public ActionResult product()
        {
            //取得所有產品放入products
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            //Index.cshtml檢視套用_LayoutMember版面配置頁，同時使用products模型
            return View("../Home/product", "_LayoutMember", products);
        }



        //Get: Shoppong/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();   // 登出
            //清除使用者Session
            Session["loginUser"] = null;
            return RedirectToAction("home", "Home");
        }



        /// ///////////////////////////


        //Get:Member/ShoppingCart
        public ActionResult ShoppingCart()
        {
            //取得登入會員的帳號並指定給fUserId
            string fUserId = User.Identity.Name;
            //找出未成為訂單明細的資料，即購物車內容
            var orderDetails = db.tOrderDetail.Where
                (m => m.fUserId == fUserId && m.fIsApproved == "否")
                .ToList();
            //View使用orderDetails模型
            return View(orderDetails);
        }


        //Get:Member/AddCart
        public ActionResult AddCart(string fPId)
        {
            //取得會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;
            //找出會員放入訂單明細的產品，該產品的fIsApproved為"否"
            //表示該產品是購物車狀態
            var currentCart = db.tOrderDetail
                .Where(m => m.fPId == fPId && m.fIsApproved == "否" && m.fUserId == fUserId)
                .FirstOrDefault();
            //若currentCart等於null，表示會員選購的產品不是購物車狀態
            if (currentCart == null)
            {
                //找出目前選購的產品並指定給product
                var product = db.tProduct.Where(m => m.fPId == fPId).FirstOrDefault();
                //將產品放入訂單明細，因為產品的fIsApproved為"否"，表示為購物車狀態
                tOrderDetail orderDetail = new tOrderDetail();
                orderDetail.fUserId = fUserId;
                orderDetail.fPId = product.fPId;
                orderDetail.fName = product.fName;
                orderDetail.fPrice = product.fPrice;
                orderDetail.fQty = 1;
                orderDetail.fImagePath = product.fImagePath;
                orderDetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetail);
            }
            else
            {
                //若產品為購物車狀態，即將該產品數量加1
                currentCart.fQty += 1;
            }

            
            db.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }


        //Get:Member/DeleteCart
        public ActionResult DeleteCart(int fId)
        {
            // 依fId找出要刪除購物車狀態的產品
            var orderDetail = db.tOrderDetail.Where
                (m => m.fId == fId).FirstOrDefault();
            //刪除購物車狀態的產品
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("ShoppingCart");
        }


        //Post:Member/ShoppingCart
        [HttpPost]
        public ActionResult ShoppingCart(string fReceiver, string fEmail, string fAddress, string fPhone, string fPrize, int fTotal)
        {
            //找出會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;
            //建立唯一的識別值並指定給guid變數，用來當做訂單編號
            //tOrder的fOrderGuid欄位會關聯到tOrderDetail的fOrderGuid欄位
            //形成一對多的關係，即一筆訂單資料會對應到多筆訂單明細
            string guid = Guid.NewGuid().ToString();
            //建立訂單主檔資料
            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            order.fPhone = fPhone;
            order.fPrize = fPrize;  //傳入抽獎結果
            order.fTotal = fTotal; //紀錄計算金額
            db.tOrder.Add(order);

            //找出目前會員在訂單明細中是購物車狀態的產品
            var cartList = db.tOrderDetail
                .Where(m => m.fIsApproved == "否" && m.fUserId == fUserId)
                .ToList();
            //將購物車狀態產品的fIsApproved設為"是"，表示確認訂購產品
            foreach (var item in cartList)
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }
            //更新資料庫，異動tOrder和tOrderDetail
            //完成訂單主檔和訂單明細的更新
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }



        /// //////////////////////


        //Get:Member/OrderList
        public ActionResult OrderList()
        {
            //找出會員帳號並指定給fUserId
            string fUserId = User.Identity.Name;
            //找出目前會員的所有訂單主檔記錄並依照fDate進行遞增排序
            //將查詢結果指定給orders
            var orders = db.tOrder.Where(m => m.fUserId == fUserId)
                .OrderByDescending(m => m.fDate).ToList();
            //目前會員的訂單主檔OrderList.cshtml檢視使用orders模型
            return View(orders);
        }


        //Get:Member/OrderDetail
        public ActionResult OrderDetail(string fOrderGuid)
        {
            //根據fOrderGuid找出和訂單主檔關聯的訂單明細，並指定給orderDetails
            var orderDetails = db.tOrderDetail
                .Where(m => m.fOrderGuid == fOrderGuid).ToList();
            //目前訂單明細的OrderDetail.cshtml檢視使用orderDetails模型
            return View(orderDetails);
        }




    }
}