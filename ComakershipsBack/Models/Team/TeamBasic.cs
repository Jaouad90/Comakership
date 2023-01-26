using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace Models
{
    /// <summary>
    /// The TeamBasic object
    /// </summary>
    public class TeamBasic
    {
        /// <summary>
        /// The ID of the Team
        /// </summary>
        /// <example>2</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Team
        /// </summary>
        /// <example>Team xXCloudMobileXx</example>
        public string Name { get; set; }

        /// <summary>
        /// The description of the Team
        /// </summary>
        /// <example>Hi we are Joey, Niek and Stefan.</example>
        public string Description { get; set; }
    }
}
