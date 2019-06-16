using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EstateInvestmentWebApplication.Data;
using EstateInvestmentWebApplication.Models;

namespace EstateInvestmentWebApplication.Controllers
{
    public class EstateController : Controller
    {
        ApplicationDbContext _db;

        public EstateController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult CreateEstate()
        {
            Estate estateModel = new Estate();

            return View(estateModel);
        }
    }
}