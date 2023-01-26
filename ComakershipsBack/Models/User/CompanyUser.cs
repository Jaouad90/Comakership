using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Dahomey.Json.Attributes;
using Models;
using System.ComponentModel.DataAnnotations;

namespace Models {
    /// <summary>
    /// object derived from Userbody which makes a companyuser
    /// </summary>
    public class CompanyUser : UserBody {
        /// <summary>
        /// The company the companyuser is a part of
        /// </summary>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public Company Company { get; set; }

        /// <summary>
        /// if the user is an admin of the company he is a part of
        /// </summary>
        public bool IsCompanyAdmin { get; set; } = false;

        /// <summary>
        /// Contains the id for this user's company
        /// </summary>
        public int? CompanyId { get; set; }
    }
}
