using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// The ComakershipStatusPut object
    /// </summary>
    public class ComakershipStatusPut
    {
        /// <summary>
        /// The id of the status
        /// </summary>
        /// <example>1</example>
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the status
        /// </summary>
        /// <example>Not started</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
    }
}
