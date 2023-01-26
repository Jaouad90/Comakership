using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// The comakership object
    /// </summary>
    public class Comakership : IEntityBase
    {
        /// <summary>
        /// The id of the comakership
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the comakership
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The description of the comakership
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// The id of the ComakershipStatus entity
        /// </summary>
        [Required]
        public int ComakershipStatusId { get; set; }

        /// <summary>
        /// If a student can earn EC's
        /// </summary>
        public bool Credits { get; set; }

        /// <summary>
        /// If a student can earn a bonus
        /// </summary>
        public bool Bonus { get; set; }

        /// <summary>
        /// The id of the Company entity
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// The date the Comakership was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }


        // navigation props

        /// <summary>
        /// The related company object
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// The related comakershipstatus object
        /// </summary>
        public ComakershipStatus Status { get; set; }

        /// <summary>
        /// The related collection of deliverable objects
        /// </summary>
        public ICollection<Deliverable> Deliverables { get; set; }

        /// <summary>
        /// The related collection of ComakershipSkill objects
        /// </summary>
        /// Linking table between Comakerships and Skills
        public ICollection<ComakershipSkill> LinkedSkills { get; set; }

        /// <summary>
        /// The related collection of ComakershipProgram objects
        /// </summary>
        /// Linking table between Comakerships and Programs
        public ICollection<ComakershipProgram> LinkedPrograms { get; set; }

        /// <summary>
        /// users working on the comakership
        /// </summary>
        public ICollection<UserComakership> StudentUsers { get; set; } = new List<UserComakership>();

        /// <summary>
        /// The collection of skills used to create LinkedSkills when posting a new comakership
        /// </summary>
        [NotMapped]
        public ICollection<Skill> Skills { get; set; }

        /// <summary>
        /// The collection of programs used to create LinkedPrograms when posting a new comakership
        /// </summary>
        [NotMapped]
        public ICollection<int> ProgramIds { get; set; }

        /// <summary>
        /// teams that applied for this comakership
        /// </summary>
        public ICollection<TeamComakership> Applications { get; set; }
    }
}
