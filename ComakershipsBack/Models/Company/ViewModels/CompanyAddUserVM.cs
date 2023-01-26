using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    public class CompanyAddUserVM {
        /// <summary>
        /// email of the user to add to the company
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// whether to make the user admin of the company, optional and false by default
        /// </summary>
        public bool MakeAdmin { get; set; } = false;
    }
}
