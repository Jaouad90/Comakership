using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Collections.ObjectModel;

namespace Models
{
    /// <summary>
    /// The TeamComplete object
    /// </summary>
    public class TeamComplete
    {
        /// <summary>
        /// The ID of the Team
        /// </summary>
        /// <example>2</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Team
        /// </summary>
        /// <example>Team xXCloudMobileXx</example>
        public string Name { get; set; }

        /// <summary>
        /// The description of the Team
        /// </summary>
        /// <example>Hi we are Joey, Niek and Stefan.</example>
        public string Description { get; set; }

        /// <summary>
        /// The related students who are part of this team
        /// </summary>
        public Collection<StudentView> Members { get; set; }

    }
}
