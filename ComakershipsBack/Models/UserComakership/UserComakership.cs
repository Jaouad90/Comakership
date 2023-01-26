using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    /// <summary>
    /// The UserComakership object
    /// </summary>
    public class UserComakership {

        /// <summary>
        /// id for the user
        /// </summary>
        [Required]
        public int StudentUserId { get; set; }

        /// <summary>
        /// id for the comakership
        /// </summary>
        [Required]
        public int ComakershipId { get; set; }

        /// <summary>
        /// The related studentuser
        /// </summary>
        public StudentUser StudentUser { get; set; }

        /// <summary>
        /// The related comakership
        /// </summary>
        public Comakership Comakership { get; set; }
    }
}
