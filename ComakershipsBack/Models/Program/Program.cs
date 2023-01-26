using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// The Program object
    /// </summary>
    public class Program :IEntityBase
    {
        /// <summary>
        /// The id of the program
        /// </summary>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the program
        /// </summary>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        
        // navigation props       

        /// <summary>
        /// The related collection of ComakershipProgram objects
        /// </summary>
        public ICollection<ComakershipProgram> LinkedComakerships { get; set; }        
    }
}
