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
    /// The TeamInviteGet object
    /// </summary>
    public class TeamInviteGet
    {
        /// <summary>
        /// The ID of the Team sending the invite
        /// </summary>
        /// <example>2</example>
        [Required]
        public int TeamId { get; set; }

        /// <summary>
        /// The ID of the StudentUser receiving the invite
        /// </summary>
        /// <example>3</example>
        [Required]
        public int StudentUserId { get; set; }


        // navigation props

        /// <summary>
        /// The related studentuser
        /// </summary>
        public StudentView StudentUser { get; set; }

        /// <summary>
        /// The related team
        /// </summary>
        public TeamBasic Team { get; set; }
    }
}