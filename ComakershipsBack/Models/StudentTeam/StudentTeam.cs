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
    /// The StudentTeam object
    /// </summary>
    public class StudentTeam
    {
        /// <summary>
        /// The ID of the StudentUser
        /// </summary>
        /// <example>3</example>
        [Required]
        public int StudentUserId { get; set; }

        /// <summary>
        /// The ID of the Team
        /// </summary>
        /// <example>2</example>
        [Required]
        public int TeamId { get; set; }


        // navigation props

        /// <summary>
        /// The related studentuser
        /// </summary>
        public StudentUser StudentUser { get; set; }

        /// <summary>
        /// The related team
        /// </summary>
        public Team Team { get; set; }
    }
}