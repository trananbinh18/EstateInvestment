using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.DatabaseEntitiesModel
{
    public class NumberUserAccess
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int UserNumber {get; set; }
    }
}
