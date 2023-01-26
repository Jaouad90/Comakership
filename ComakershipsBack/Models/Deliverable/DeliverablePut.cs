using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// The DeliverablePut object
    /// </summary>
    public class DeliverablePut
    {
        /// <summary>
        /// The id of the Deliverable
        /// </summary>
        /// <example>1</example>
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Deliverable
        /// </summary>
        /// <example>Do some stuff</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// If the Deliverable is finished or not
        /// </summary>
        /// <example>false</example>
        [JsonRequired(RequirementPolicy.Always)]
        public bool Finished { get; set; }
    }
}