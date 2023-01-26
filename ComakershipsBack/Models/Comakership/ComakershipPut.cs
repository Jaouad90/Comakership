using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// This object contains all the data related to the ComakershipPutModel
    /// </summary>
    public class ComakershipPut
    {
        /// <summary>
        /// The id of the Comakership
        /// </summary>
        /// <example>2</example>
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Comakership
        /// </summary>
        /// <example>An awesome Comakership</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the Comakership
        /// </summary>
        /// <example>We would like you to do some stuff</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Is it possible to earn EC's?
        /// </summary>
        /// <example>False</example>
        [JsonRequired(RequirementPolicy.Always)]
        public bool Credits { get; set; }

        /// <summary>
        /// Is there a bonus available on completion?
        /// </summary>
        /// <example>True</example>
        [JsonRequired(RequirementPolicy.Always)]
        public bool Bonus { get; set; }

        /// <summary>
        /// The id of the ComakershipStatus entity
        /// </summary>
        [JsonRequired(RequirementPolicy.Always)]
        public int ComakershipStatusId { get; set; }
    }
}
