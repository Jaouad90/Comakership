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
    /// The StudentTeamUpdate object
    /// </summary>
    public class StudentTeamUpdate
    {
        /// <summary>
        /// The ID of the StudentUser
        /// </summary>
        /// <example>3</example>
        public int StudentUserId { get; set; }

        /// <summary>
        /// The ID of the Team
        /// </summary>
        /// <example>2</example>
        public int TeamId { get; set; }
    }
}