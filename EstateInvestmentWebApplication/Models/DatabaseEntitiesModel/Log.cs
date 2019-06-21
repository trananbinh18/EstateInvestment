using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.DatabaseEntitiesModel
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}
