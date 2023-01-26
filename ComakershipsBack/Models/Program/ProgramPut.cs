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
    /// The ProgramPut object
    /// </summary>
    public class ProgramPut
    {
        /// <summary>
        /// The id of the program
        /// </summary>
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the program
        /// </summary>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
    }
}