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
    /// The Deliverable object
    /// </summary>
    public class Deliverable :IEntityBase
    {
        /// <summary>
        /// The ID of the Deliverable
        /// </summary>
        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the Comakership that the Deliverable belongs to
        /// </summary>
        /// <example>1</example>
        [Required]
        public int ComakershipId { get; set; }

        /// <summary>
        /// The name of the Deliverable
        /// </summary>
        /// <example>Do some stuff</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// If the Deliverable is finished or not
        /// </summary>
        /// <example>false</example>
        [Required]
        public bool Finished { get; set; }


        // navigation props

        /// <summary>
        /// The related comakership that the deliverable belongs to
        /// </summary>
        public Comakership Comakership { get; set; }
    }
}