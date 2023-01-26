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
    /// The ComakershipSkill object
    /// </summary>
    public class ComakershipSkill
    {
        /// <summary>
        /// The ID of the Comakership
        /// </summary>
        /// <example>2</example>
        [Required]
        public int ComakershipId { get; set; }

        /// <summary>
        /// The ID of the skill
        /// </summary>
        /// <example>4</example>
        [Required]
        public int SkillId { get; set; }


        // navigation props

        /// <summary>
        /// The related comakership
        /// </summary>
        public Comakership Comakership { get; set; }

        /// <summary>
        /// The related skill
        /// </summary>
        public Skill Skill { get; set; }
    }
}