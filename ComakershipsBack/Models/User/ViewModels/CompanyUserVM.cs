using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class CompanyUserVM
    {
        /// <summary>
        /// Contains the id of the user.
        /// </summary>
        /// <example>1</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }
        /// <summary>
        /// Contains the name of the user.
        /// </summary>
        /// <example>henk</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
        /// <summary>
		/// Contains the email of the user.
		/// </summary>
		/// <example>henk@inholland.nl</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Email { get; set; }
        /// <summary>
        /// if the user is an admin of the company he is a part of
        /// </summary>
        public bool IsCompanyAdmin { get; set; } = false;
    }
}
