using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using EstateInvestmentWebApplication.Models.ViewModels;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace EstateInvestmentWebApplication.Controllers
{
    public class EstateProjectController : Controller
    {
        ApplicationDbContext _dbContext;
        IHostingEnvironment _hostingEnvironment;
        UserManager<IdentityUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public EstateProjectController(IHostingEnvironment he, ApplicationDbContext db, UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        {
            _dbContext = db;
            _hostingEnvironment = he;
            _userManager = um;
            _roleManager = rm;
        }

        [HttpGet]
        [Route("them-du-an")]
        public IActionResult CreateEstate()
        {
            ViewBag.listCatalog = _dbContext.EstateCatalogs.ToList();
            return View();
        }

        [HttpPost]
        [Route("them-du-an")]
        public IActionResult CreateEstate(CreateEstateViewModel model)
        {
            if (ModelState.IsValid)
            {
                EstateProject estateProject = new EstateProject();

                //Upload Thumbnail File Image
                var imageUploadResult = UploadImage(model.Image, "thumbnails");
                string imagePath = imageUploadResult.Value.GetType().GetProperty("url").GetValue(imageUploadResult.Value, null).ToString();

                estateProject.ImagePath = imagePath;
                estateProject.Title = model.Title;
                estateProject.ShortDescription = model.ShortDescription;
                estateProject.Content = model.Content;
                estateProject.CatalogId = model.CatalogId;
                estateProject.UserId = _userManager.GetUserId(User);

                _dbContext.EstateProjects.Add(estateProject);
                _dbContext.SaveChanges();

                //return RedirectToAction("Index", "Home");
                return RedirectToAction("DetailEstate", "EstateProject", new { id = estateProject.Id });
            }

            ViewBag.listCatalog = _dbContext.EstateCatalogs.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult EditEstate(int id)
        {
            var estate = _dbContext.EstateProjects.Find(id);

            CreateEstateViewModel createModel = new CreateEstateViewModel();
            createModel.Id = estate.Id;
            createModel.CatalogId = estate.CatalogId;
            createModel.Content = estate.Content;
            createModel.CreateDate = estate.CreateDate;
            createModel.ShortDescription = estate.ShortDescription;
            createModel.Title = estate.Title;
            createModel.ImagePath = estate.ImagePath;

            ViewBag.listCatalog = _dbContext.EstateCatalogs.ToList();
            return View(createModel);
        }



        [HttpPost]
        public IActionResult EditEstate(CreateEstateViewModel model)
        {
            if (model.Image == null)
            {
                ModelState["Image"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                var estate = _dbContext.EstateProjects.Find(model.Id);
                estate.CatalogId = model.CatalogId;
                estate.Title = model.Title;
                estate.ShortDescription = model.ShortDescription;
                estate.Content = model.Content;
                if (model.Image != null)
                {
                    //Delete Old Thumbnail File Image
                    var filePathArr = estate.ImagePath.Split('/');
                    string fileName = filePathArr[filePathArr.Length - 1];
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "thumbnails", fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    //Upload New Thumbnail File Image
                    var imageUploadResult = UploadImage(model.Image, "thumbnails");
                    string imagePath = imageUploadResult.Value.GetType().GetProperty("url").GetValue(imageUploadResult.Value, null).ToString();
                    estate.ImagePath = imagePath;

                }
                _dbContext.Entry(estate).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return RedirectToAction("ListEstate");
            }

            ViewBag.listCatalog = _dbContext.EstateCatalogs.ToList();
            return View(model);
        }

        [Route("du-an/{id}")]
        public IActionResult DetailEstate(int id)
        {
            var estate = _dbContext.EstateProjects.Find(id);

            return View(estate);
        }


        [HttpPost]
        public IActionResult DeleteEstate([FromBody]int id)
        {
            var estate = _dbContext.EstateProjects.Find(id);

            //Read HTML content Find Img tag and get attribute src to delete image
            HtmlDocument doc = new HtmlDocument();
            byte[] byteArray = Encoding.ASCII.GetBytes(estate.Content);
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
            string[] arrPath = estate.ImagePath.Split('/');
            string fileName = arrPath[arrPath.Length - 1];
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "thumbnails", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _dbContext.EstateProjects.Remove(estate);
            _dbContext.SaveChanges();
            return RedirectToAction("ListEstate");
        }



        [HttpGet]
        public IActionResult ListEstate()
        {
            var listEstate = _dbContext.EstateProjects.ToList();
            return View(listEstate);
        }

        [HttpPost]
        public JsonResult UploadImage(IFormFile upload, string imageFolder = "")
        {

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + upload.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", imageFolder, fileName);

            var fileStream = new FileStream(filePath, FileMode.Create);
            upload.CopyToAsync(fileStream).Wait();

            fileStream.Dispose();

            return Json(new { uploaded = 1, fileName = fileName, url = "/images/" + imageFolder + "/" + fileName });
        }

        public IActionResult FileBrowser(IFormFile upload)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "imagescontent");

            var dir = new DirectoryInfo(filePath);
            ViewBag.fileInfos = dir.GetFiles();

            return View();
        }


    }
}