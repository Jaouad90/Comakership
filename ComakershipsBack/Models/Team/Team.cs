using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace Models
{
    /// <summary>
    /// The Team object
    /// </summary>
    public class Team : IEntityBase
    {
        /// <summary>
        /// The ID of the Team
        /// </summary>
        /// <example>2</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Team
        /// </summary>
        /// <example>Team xXCloudMobileXx</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The description of the Team
        /// </summary>
        /// <example>Hi we are Joey, Niek and Stefan.</example>
        public string Description { get; set; }

        // navigation prop to linktable
        /// <summary>
        /// The related student who are part of this team
        /// </summary>
        public ICollection<StudentTeam> LinkedStudents { get; set; }

        /// <summary>
        /// Comakerships the team applied for but haven't been accepted or rejected yet
        /// </summary>
        public ICollection<TeamComakership> AppliedComakerships { get; set; } = new List<TeamComakership>();

        /// <summary>
        /// The related students applies for this team
        /// </summary>
        public ICollection<JoinRequest> JoinRequests { get; set; }

        /// <summary>
        /// The related studenst who are invited to join this team
        /// </summary>
        public ICollection<TeamInvite> TeamInvites { get; set; }
    }
}
