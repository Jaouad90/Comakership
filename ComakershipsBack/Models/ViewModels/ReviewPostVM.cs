using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    public class ReviewPostVM
    {
        public int CompanyId { get; set; }
        public int StudentUserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool ForCompany { get; set; }
    }
}
