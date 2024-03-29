﻿using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using EstateInvestmentWebApplication.Models.ViewModels;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Route("admin/du-an/them-duan")]
        public IActionResult CreateEstate()
        {
            ViewBag.listCatalog = _dbContext.EstateCatalogs.ToList();
            return View();
        }

        [HttpPost]
        [Route("admin/du-an/them-duan")]
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
                estateProject.Visible = true;
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
        [Route("admin/du-an/chinh-sua/{id}")]
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
        [Route("admin/du-an/doi-trang-thai")]
        public void ChangeVisible([FromForm]int id, [FromForm]bool value)
        {
            var estate = _dbContext.EstateProjects.Find(id);
            estate.Visible = value;
            _dbContext.Entry(estate).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }


        [HttpPost]
        [Route("admin/du-an/chinh-sua/{id}")]
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
        public IActionResult DeleteEstate([FromForm]int id)
        {
            var estate = _dbContext.EstateProjects.Find(id);

            //Read HTML content Find Img tag and get attribute src to delete image
            HtmlDocument doc = new HtmlDocument();
            byte[] byteArray = Encoding.ASCII.GetBytes(estate.Content);
            MemoryStream stream = new MemoryStream(byteArray);
            doc.Load(stream);
            stream.Dispose();

            var lstTagImg = doc.DocumentNode.SelectNodes("/p/img");

            if (lstTagImg != null)
            {
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
        [Route("admin/du-an/{page?}")]
        public async Task<IActionResult> ListEstate(int page = 1)
        {
            var listEstate = _dbContext.EstateProjects.OrderByDescending(x => x.CreateDate);
            var model = await PagingList.CreateAsync(listEstate, 10, page);
            return View(model);
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


        [HttpPost]
        public JsonResult DeleteImage([FromBody]string data)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), _hostingEnvironment.WebRootPath, "images", "imagescontent", data);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Json(new { status = "success", message = "Xoá ảnh thành công" });
                }

                return Json(new { status = "fail", message = "Không tìm thấy ảnh" });

            }
            catch (Exception e)
            {
                return Json(new { status = "fail", message = "Không thể xoá ảnh" });
            }

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