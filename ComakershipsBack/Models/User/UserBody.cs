using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Models {
    /// <summary>
    /// Base abstract object that makes up a user
    /// </summary>
    public class UserBody : IEntityBase {

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
		/// Contains the encrypted password of the user.
		/// </summary>
		/// <example>regkjuheswgiew23423547ew!!"^%ygyfu</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        [Newtonsoft.Json.JsonIgnore]
        public string Password { get; set; }

        /// <summary>
		/// If the user is deleted or not, default=false
		/// </summary>
		/// <example>false</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public bool Deleted { get; set; } = false;
    }   
}
