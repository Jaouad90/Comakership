using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using Models;

namespace Models
{   /// <summary>
    /// This object contains the various fields that together make up a "Company"
    /// </summary>
    public class CompanyVM
    {
        /// <summary>
		/// Contains the id of the company.
		/// </summary>
		/// <example>1234</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
		/// Contains the name of the company.
		/// </summary>
		/// <example>TestCompany</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
		/// Contains the description of the company.
		/// </summary>
		/// <example>Insert very interesting description here</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Contains the registrationdate of the company in the Comakerships App.
        /// </summary>
        /// <example>2020-09-20T13:05:08.544Z</example>
        [JsonRequired(RequirementPolicy.Always)]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Contains the reviews given to the company by students previously participated in a comakership from the company.
        /// </summary>
        /// <example></example>
        [JsonRequired(RequirementPolicy.Never)]
        public List<Review> Reviews { get; set; }

        /// <summary>
		/// Contains the streetname the company is located at.
		/// </summary>
		/// <example>Teststreet 13</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Street { get; set; }

        /// <summary>
		/// Contains the city the company is located in.
		/// </summary>
		/// <example>Testcity</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string City { get; set; }

        /// <summary>
		/// Contains the zipcode of the company.
		/// </summary>
		/// <example>1834TEST</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Zipcode { get; set; }
    }
}

