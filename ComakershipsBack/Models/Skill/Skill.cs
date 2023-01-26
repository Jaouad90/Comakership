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
    /// The Skill object
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// The ID of the skill
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the skill
        /// </summary>
        /// <example>C#</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The related comakerships who require this skill
        /// </summary>
        public ICollection<ComakershipSkill> LinkedComakerships { get; set; }
    }
}