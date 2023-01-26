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
    /// The DeliverableGet object
    /// </summary>
    public class DeliverableGet
    {
        /// <summary>
        /// The ID of the Deliverable
        /// </summary>
        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Deliverable
        /// </summary>
        /// <example>Do some stuff</example>
        public string Name { get; set; }

        /// <summary>
        /// If the Deliverable is finished or not
        /// </summary>
        /// <example>false</example>
        public bool Finished { get; set; }
    }
}