using System;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace  Models
{   /// <summary>
    /// This object contains the various fields that together make up a "University"
    /// </summary>
    public class University
    {
        /// <summary>
		/// Contains the id of the university.
		/// </summary>
		/// <example>66</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int? Id { get; set; }

        /// <summary>
		/// Contains the name of the university.
		/// </summary>
		/// <example>Inholland</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Contains the registrationdate of the university in the Comakerships App.
        /// </summary>
        /// <example>2020-09-20T12:05:08.544Z</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Contains the domain of the university in the Comakerships App.
        /// </summary>
        /// <example>@student.inholland.nl</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Domain { get; set; }

        /// <summary>
        /// Contains the streetname the university is located at.
        /// </summary>
        /// <example>Teststreet 13</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Street { get; set; }

        /// <summary>
        /// Contains the city the university is located in.
        /// </summary>
        /// <example>Testcity</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string City { get; set; }

        /// <summary>
        /// Contains the zipcode of the university.
        /// </summary>
        /// <example>1834TEST</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Zipcode { get; set; }
    }
}
