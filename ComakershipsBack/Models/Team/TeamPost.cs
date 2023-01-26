using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace Models
{
    /// <summary>
    /// This object contains all the data related to the Team object
    /// </summary>
    public class TeamPost
    {
        /// <summary>
        /// The name of the Team
        /// </summary>
        /// <example>Team xXCloudMobileXx</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the Team
        /// </summary>
        /// <example>Hi we are Joey, Niek and Stefan.</example>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public string Description { get; set; }
    }
}
