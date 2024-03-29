﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EstateInvestmentWebApplication.Models;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models.DatabaseEntitiesModel;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace EstateInvestmentWebApplication.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext db) 
        {
            _dbContext = db;
        }
        
        //[Route("trang-chu")]
        public async Task<IActionResult> Index(int id = 0,int page = 1)
        {
            IOrderedQueryable<EstateProject> listEstate;
            List<New> listNew;
            listNew = _dbContext.News.Where(x => x.Visible == true).OrderByDescending(x => x.CreateDate).Take(3).ToList();
            ViewBag.listNew = listNew;

            //Load Estate Project by catalog
            if (id != 0)
            {
                listEstate = _dbContext.EstateProjects.Where(x => x.Visible == true).Where(x => x.CatalogId == id).OrderByDescending(x => x.CreateDate);
            }
            //Default is all Estate Project of catalog
            else
            {
                listEstate = _dbContext.EstateProjects.Where(x => x.Visible == true).OrderByDescending(x => x.CreateDate);
            }

            var model = await PagingList.CreateAsync(listEstate, 6, page);

            return View(model);
        }

        [Route("gioi-thieu")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Route("lien-he")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Route("tin-tuc")]
        public async Task<IActionResult> News(int page = 1)
        {
            IOrderedQueryable<New> listNew;

            listNew = _dbContext.News.Where(x => x.Visible == true).OrderByDescending(x => x.CreateDate);
            var model = await PagingList.CreateAsync(listNew, 6, page);

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
