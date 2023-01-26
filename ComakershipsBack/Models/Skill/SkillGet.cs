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
    /// The SkillGet object
    /// </summary>
    public class SkillGet
    {
        /// <summary>
        /// The name of the skill
        /// </summary>
        /// <example>C#</example>
        public string Name { get; set; }
    }
}