using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using EstateInvestmentWebApplication.Models.ViewModels;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstateInvestmentWebApplication.Controllers
{
    public class NewController : Controller
    {

        ApplicationDbContext _dbContext;
        IHostingEnvironment _hostingEnvironment;
        UserManager<IdentityUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public NewController(IHostingEnvironment he, ApplicationDbContext db, UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        {
            _dbContext = db;
            _hostingEnvironment = he;
            _userManager = um;
            _roleManager = rm;
        }

        [HttpGet]
        public IActionResult CreateNew()
        {
            return View();
        }



        [HttpPost]
        public IActionResult CreateNew(CreateNewViewModel model)
        {
            if (ModelState.IsValid)
            {
                New news = new New();

                //Upload Thumbnail File Image
                EstateProjectController estateProjectController = new EstateProjectController(_hostingEnvironment,_dbContext,_userManager,_roleManager);
                var imageUploadResult = estateProjectController.UploadImage(model.Image, "thumbnails");
                string imagePath = imageUploadResult.Value.GetType().GetProperty("url").GetValue(imageUploadResult.Value, null).ToString();

                news.ImagePath = imagePath;
                news.Title = model.Title;
                news.ShortDescription = model.ShortDescription;
                news.Content = model.Content;
                news.UserId = _userManager.GetUserId(User);

                _dbContext.News.Add(news);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Home");

            }

            return View(model);
        }


        [HttpGet]
        public IActionResult EditNew(int id)
        {
            var news = _dbContext.News.Find(id);

            CreateNewViewModel createModel = new CreateNewViewModel();
            createModel.Id = news.Id;
            createModel.Content = news.Content;
            createModel.CreateDate = news.CreateDate;
            createModel.ShortDescription = news.ShortDescription;
            createModel.Title = news.Title;
            createModel.ImagePath = news.ImagePath;

            return View(createModel);
        }



        [HttpPost]
        public IActionResult EditNew(CreateNewViewModel model)
        {
            if (model.Image == null)
            {
                ModelState["Image"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                var news = _dbContext.News.Find(model.Id);
                news.Title = model.Title;
                news.ShortDescription = model.ShortDescription;
                news.Content = model.Content;
                if (model.Image != null)
                {
                    //Delete Old Thumbnail File Image
                    var filePathArr = news.ImagePath.Split('/');
                    string fileName = filePathArr[filePathArr.Length - 1];
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "thumbnails", fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    //Upload New Thumbnail File Image
                    EstateProjectController estateProjectController = new EstateProjectController(_hostingEnvironment, _dbContext, _userManager, _roleManager);
                    var imageUploadResult = estateProjectController.UploadImage(model.Image, "thumbnails");
                    string imagePath = imageUploadResult.Value.GetType().GetProperty("url").GetValue(imageUploadResult.Value, null).ToString();
                    news.ImagePath = imagePath;

                }
                _dbContext.Entry(news).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return RedirectToAction("ListNew");
            }

            return View(model);
        }



        public IActionResult DetailNew(int id)
        {
            var news = _dbContext.News.Find(id);

            return View(news);
        }

        [HttpGet]
        public IActionResult ListNew()
        {
            var listNews = _dbContext.News.ToList();
            return View(listNews);
        }


        [HttpPost]
        public IActionResult DeleteNew([FromBody]int id)
        {
            var news = _dbContext.News.Find(id);

            //Read HTML content Find Img tag and get attribute src to delete image
            HtmlDocument doc = new HtmlDocument();
            byte[] byteArray = Encoding.ASCII.GetBytes(news.Content);
            MemoryStream stream = new MemoryStream(byteArray);
            doc.Load(stream);

            var lstTagImg = doc.DocumentNode.SelectNodes("/p/img");

            //Delete Image Content
            foreach (var tag in lstTagImg)
            {
                var imgSrc = tag.Attributes["src"];
                string[] arrPathImg = imgSrc.Value.Split('/');
                string fileNameImg = arrPathImg[arrPathImg.Length - 1];
                string filePathImg = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "imagescontent", fileNameImg);
                if (System.IO.File.Exists(filePathImg))
                {
                    System.IO.File.Delete(filePathImg);
                }
            }

            //Delete Image Thumnails
            string[] arrPath = news.ImagePath.Split('/');
            string fileName = arrPath[arrPath.Length - 1];
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "thumbnails", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _dbContext.News.Remove(news);
            _dbContext.SaveChanges();
            return RedirectToAction("ListNew");
        }



      




    }
}