using Dahomey.Json.Attributes;
using Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Models {
    /// <summary>
    /// object derived from Userbody which makes a student
    /// </summary>
    public class StudentUser : UserBody {

        /// <summary>
		/// Contains the studentnumber of the student.
		/// </summary>
		/// <example>123456</example>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public int StudentNumber { get; set; }

        /// <summary>
		/// A description of the student
		/// </summary>
		/// <example>I am a very good student with experience in multiple programming languages.</example>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public string About { get; set; }

        /// <summary>
        /// Nickname of the student
        /// </summary>
        /// <example>xXxStoereStudentxXx</example>
        public string Nickname { get; set; }

        /// <summary>
		/// Contains the University of the student.
		/// </summary>
        [JsonRequired(RequirementPolicy.Always)]
        public University University { get; set; }

        /// <summary>
        /// Contains the Id of this students university
        /// </summary>
        [Required]
        public int UniversityId { get; set; }

        /// <summary>
        /// Contains the Id of this student Program
        /// </summary>
        public int ProgramId { get; set; }

        /// <summary>
        /// Contains this students program
        /// </summary>
        public virtual Program Program { get; set; }

        /// <summary>
        /// Contains the Id of the one team of the student that is private
        /// </summary>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public int? PrivateTeamId { get; set; }

        /// <summary>
        /// Contains the private team of this student
        /// </summary>
        [JsonRequired(RequirementPolicy.Never)]
        public Team PrivateTeam { get; set; }

        /// <summary>
		/// A list of various links of the student, for example social media or github accounts
		/// </summary>
        [JsonRequired(RequirementPolicy.AllowNull)]
        [NotMapped]
        public ICollection<string> Links { get; set; } = new List<string>();

        //workaround to store lists with primitive types in the database, its stored as a JSON string in the db, but can be used in code like a normal list
        public string LinksJson
        {
            get
            {
                return Links == null || Links.Count == 0
                   ? null
                   : JsonSerializer.Serialize(Links);
            }
            set
            {
                if (value == null)
                    Links.Clear();
                else
                    Links = JsonSerializer.Deserialize<List<string>>(value);
            }
        }

        /// <summary>
		/// list of skills of the student, for example programming languages, spoken languages etc.
		/// </summary>
        [JsonRequired(RequirementPolicy.AllowNull)]
        [NotMapped]
        public ICollection<string> Skills { get; set; } = new List<string>();

        //workaround to store lists with primitive types in the database, its stored as a JSON string in the db, but can be used in code like a normal list
        public string SkillsJson
        {
            get
            {
                return Skills == null || Skills.Count == 0
                   ? null
                   : JsonSerializer.Serialize(Skills);
            }
            set
            {
                if (value == null)
                    Skills.Clear();
                else
                    Skills = JsonSerializer.Deserialize<List<string>>(value);
            }
        }

        // navigation prop to linktable

        /// <summary>
        /// Contains the reviews of companies given to the student.
        /// </summary>
        /// <example>TODO</example>
        [JsonRequired(RequirementPolicy.Never)]
        public IEnumerable<Review> Reviews { get; set; }
        /// <summary>
        /// Contains the reviews of companies given by the student.
        /// </summary>
        /// <example>TODO</example>
        [JsonRequired(RequirementPolicy.Never)]
        public ICollection<StudentTeam> LinkedTeams { get; set; }

        /// <summary>
        /// The related comakerships the student is working on
        /// </summary>
        public ICollection<UserComakership> Comakerships { get; set; }

        /// <summary>
        /// The related teams who invited the student
        /// </summary>
        public ICollection<TeamInvite> TeamInvites { get; set; }

        /// <summary>
        /// The related teams the student applied for
        /// </summary>
        public ICollection<JoinRequest> JoinRequests { get; set; }

    }
}
