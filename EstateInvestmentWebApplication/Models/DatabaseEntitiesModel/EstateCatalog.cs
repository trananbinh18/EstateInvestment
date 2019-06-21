using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EstateInvestmentWebApplication.Models.DatabaseEntitiesModel
{
    public class EstateCatalog
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<EstateProject> EstateProjects { get; set; }
    }
}
