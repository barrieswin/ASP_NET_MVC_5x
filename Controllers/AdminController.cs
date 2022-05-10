using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TheOceanWeb.Models;
using TheOceanWeb.ViewModel;


namespace TheOceanWeb.Controllers
{

    [Authorize(Roles = "admin" )]  //Only "admin" is authorized to acces this controller

    public class AdminController : Controller
    {

        private TheOceanDbEntities objTheOceanDbEntities;
        public AdminController()
        {
            objTheOceanDbEntities = new TheOceanDbEntities();
        }


        //GET: Index Page
        public ActionResult Index()
        {
            //Map the data from DbEntities into TProductViewModel
            IEnumerable<TProductViewModel> listOfTProductViewModel = (from objItem in objTheOceanDbEntities.tProduct
                                                                      select new TProductViewModel
                                                                      {
                                                                          FId = objItem.fId,
                                                                          FCategory = objItem.fCategory,
                                                                          FPid = objItem.fPId,
                                                                          FName = objItem.fName,
                                                                          FDescription = objItem.fDescription,
                                                                          FPrice = objItem.fPrice,
                                                                          FImagePath = objItem.fImagePath,
                                                                          FIsActiveFlag = objItem.fIsActiveFlag
                                                                      }
                                                                       ).OrderByDescending(m => m.FId).ToList();


            return View(listOfTProductViewModel);
        }


        //GET : Add product into Database
        public ActionResult Create()
        {
            //LINQ to get the data from Table of TCategories
            //Sets the CategoryselectListItem
            TProductViewModel objTProductViewModel = new TProductViewModel();
            objTProductViewModel.CategoryselectListItem = (from objCat in objTheOceanDbEntities.tCategory
                                                           select new SelectListItem()
                                                           {
                                                               Text = objCat.fCategoryName,
                                                               Value = objCat.fCategory.ToString(),
                                                               Selected = true
                                                           });
            return View(objTProductViewModel);
        }

        //POST : Create Product
        [HttpPost]
        public ActionResult Create(TProductViewModel objTProductViewModel)
        {
            //Upload File
            string NewImage = Guid.NewGuid() + Path.GetExtension(objTProductViewModel.FImage.FileName);
            objTProductViewModel.FImage.SaveAs(Server.MapPath("~/Images/" + NewImage));

            //Map Data from ViewModel into Models
            tProduct objItem = new tProduct();
            objItem.fCategory = objTProductViewModel.FCategory;
            objItem.fPId = objTProductViewModel.FPid;
            //objItem.FPid = Guid.NewGuid().ToString();  //this is to produce Guid for FPid
            objItem.fName = objTProductViewModel.FName;
            objItem.fDescription = objTProductViewModel.FDescription;
            objItem.fPrice = objTProductViewModel.FPrice;
            objItem.fImagePath = "~/Images/" + NewImage;
            objItem.fIsActiveFlag = true;

            //Add data to Database & save
            objTheOceanDbEntities.tProduct.Add(objItem);
            objTheOceanDbEntities.SaveChanges();

            //Display Success message 
            Session["createSuccess"] = "商品" + objItem.fName + "已新增成功";

            return Redirect("/Admin/Index");
        }


        //GET : Edit
        //This method uses .Find(Object[]) 方法
        public ActionResult Edit(int? id)
        {
            //Get the data on the FId of tProduct
            var objItem = objTheOceanDbEntities.tProduct.Find(id);

            //Check if the input from the source is valid
            if (objItem == null)
            {
                return HttpNotFound();
            } 
            return View(objItem);
        }

        //POST : Edit
        //This method uses .Find(Object[]) 方法
        [HttpPost]
        public ActionResult Edit(tProduct TProductModel)
        {
            //Map data of Edited items
            tProduct objItem = objTheOceanDbEntities.tProduct.Find(TProductModel.fId);
            objItem.fPId = TProductModel.fPId;
            objItem.fName = TProductModel.fName;
            objItem.fDescription = TProductModel.fDescription;
            objItem.fPrice = TProductModel.fPrice;
            objItem.fIsActiveFlag = TProductModel.fIsActiveFlag;

            //Save Into database
            objTheOceanDbEntities.SaveChanges();

            //Display Success message 
            Session["editSuccess"] = "商品" + objItem.fName + "已修改成功";

            return Redirect("/Admin/Index");
        }


        //Get: Shoppong/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();   // 登出
            return RedirectToAction("home", "Home");
        }





        //GET : Delete
        public ActionResult Delete(int? id)
        {
            //Get the data on the FId of tProduct
            var objItem = objTheOceanDbEntities.tProduct.Find(id);

            //Check if the input is null
            if (objItem == null)
            {
                return HttpNotFound();
            }
            return View(objItem);
        }

        //POST : Delete
        //Show the selected product, confirm on changing status
        [HttpPost]
        public ActionResult Delete(tProduct TProductModel)
        {
            //map data
            tProduct objItem = objTheOceanDbEntities.tProduct.Find(TProductModel.fId);
            objItem.fIsActiveFlag = TProductModel.fIsActiveFlag;

            objTheOceanDbEntities.SaveChanges();
            return Redirect("/Admin/Index");
        }




        // GET: Admin/OrderDetail
        public ActionResult OrderDetail()
        {
            //取得所有訂單放入OrderDetail
            var orderDetails = objTheOceanDbEntities.tOrderDetail
                .OrderByDescending(m => m.fId).ToList();
            return View(orderDetails);
        }

    }
}