using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;

namespace Models
{
    /// <summary>
    /// The DeliverablePost object
    /// </summary>
    public class DeliverablePost
    {
        /// <summary>
        /// The name of the Deliverable
        /// </summary>
        /// <example>Do some stuff</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
    }
}