using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    /// <summary>
    /// The TeamComakership object
    /// </summary>
    public class TeamComakership {

        /// <summary>
        /// id of the team
        /// </summary>
        [Required]
        public int TeamId { get; set; }

        /// <summary>
        /// id of the comakership
        /// </summary>
        [Required]
        public int ComakershipId { get; set; }

        /// <summary>
        /// The related Team
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// The related Comakership
        /// </summary>
        public Comakership Comakership { get; set; }
    }
}
