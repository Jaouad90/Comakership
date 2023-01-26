using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace Models
{   /// <summary>
    /// This class represents the Review part of a company or student.
    /// </summary>
    public class Review
    {
        /// <summary>
		/// Contains the id of the review.
		/// </summary>
		/// <example>1</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int Id { get; set; }

        /// <summary>
		/// Contains the id of the Company.
		/// </summary>
		/// <example>1</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int CompanyId { get; set; }

        /// <summary>
		/// Contains the id of the StudentUser.
		/// </summary>
		/// <example>3</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int StudentUserId { get; set; }

        /// <summary>
		/// Contains the name of the reviewer.
		/// </summary>
		/// <example>Jan de Vries</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string ReviewersName { get; set; }

        /// <summary>
		/// Contains the rating of the review
		/// </summary>
		/// <example>8</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int Rating { get; set; }

        /// <summary>
		/// Contains the (optional) comment of the review
		/// </summary>
		/// <example>"Dit bedrijf is top!"</example>
        [Required]
        [JsonRequired(RequirementPolicy.Never)]
        public string Comment { get; set; }

        /// <summary>
		/// True if given to company, false if given to student
		/// </summary>
		/// <example>true</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public bool ForCompany { get; set; }

    }
}
