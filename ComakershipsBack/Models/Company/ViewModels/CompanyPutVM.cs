using Dahomey.Json.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models;
using System;

namespace Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CompanyPutVM
    {
        /// <summary>
        /// Contains the id of the university
        /// </summary>
        [DefaultValue(null)]
        public int? Id { get; set; }

        /// <summary>
		/// Contains the name of the company.
		/// </summary>
		/// <example>TestCompany</example>
        //[Required]
        //[JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
		/// Contains the description of the company.
		/// </summary>
		/// <example>Insert very interesting description here</example>
        public string Description { get; set; }

        /// <summary>
        /// Contains the image of the company as base64
        /// </summary>
        // public string LogoAsBase64 { get; set; }

        /// <summary>
        /// Contains the GUID for the logo that resides on Azure Blob Storage
        /// </summary>
        public Guid LogoGuid { get; set; }

        /// <summary>
		/// Contains the streetname the company is located at.
		/// </summary>
		/// <example>Teststreet 13</example>
        public string Street { get; set; }

        /// <summary>
		/// Contains the city the company is located in.
		/// </summary>
		/// <example>Testcity</example>
        public string City { get; set; }

        /// <summary>
		/// Contains the zipcode of the company.
		/// </summary>
		/// <example>1834TEST</example>
        public string Zipcode { get; set; }
    }
}
