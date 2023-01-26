using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;

namespace Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class UniversityPutVM
    {
        /// <summary>
        /// Contains the id of the university
        /// </summary>
        [DefaultValue(null)]
        public int? Id { get; set; }

        /// <summary>
		/// Contains the name of the university.
		/// </summary>
		/// <example>Inholland</example>
        public string Name { get; set; }

        /// <summary>
        /// Contains the streetname the university is located at.
        /// </summary>
        /// <example>Teststreet 13</example>
        public string Street { get; set; }

        /// <summary>
		/// Contains the city the university is located in.
		/// </summary>
		/// <example>Testcity</example>
        public string City { get; set; }

        /// <summary>
		/// Contains the zipcode of the university.
		/// </summary>
		/// <example>1834TEST</example>
        public string Zipcode { get; set; }
    }
}
