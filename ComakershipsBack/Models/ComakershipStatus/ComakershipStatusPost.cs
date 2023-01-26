using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// The ComakershipStatusPost object
    /// </summary>
    public class ComakershipStatusPost
    {
        /// <summary>
        /// The name of the status
        /// </summary>
        /// <example>Not started</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
    }
}
