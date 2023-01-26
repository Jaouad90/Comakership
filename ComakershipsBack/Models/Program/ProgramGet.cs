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
    /// The ProgramGet object
    /// </summary>
    public class ProgramGet
    {
        /// <summary>
        /// The id of the program
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the program
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}