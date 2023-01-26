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
    /// The ProgramPost object
    /// </summary>
    public class ProgramPost
    {
        /// <summary>
        /// The name of the program
        /// </summary>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }
    }
}