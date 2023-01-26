using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class CompanyLogoVM
    {
        public int Id { get; set; }
        public Guid LogoGuid { get; set; }
        public int CompanyId { get; set; }

        public Uri Uri { get; set; }
    }
}
