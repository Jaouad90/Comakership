using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class UniversityPostVM
    {

        /// <summary>
        /// Contains the name of the university.
        /// </summary>
        /// <example>Inholland</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Contains the domain of the university in the Comakerships App.
        /// </summary>
        /// <example>@student.inholland.nl</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Domain { get; set; }

        /// <summary>
        /// Contains the registrationdate of the university in the Comakerships App.
        /// </summary>
        /// <example>2020-09-20T12:05:08.544Z</example>
        public DateTime RegistrationDate { get; set; }

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
