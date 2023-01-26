using Dahomey.Json.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Model for returning an JSON response.
    /// </summary>
    public class responseBody
    {
        /// <summary>
        /// Represents the statuscode of the response.
        /// </summary>
        /// <example>HttpStatusCode</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public int StatusCode { get; set; }

        /// <summary>
        /// Represents the additional info on the response
        /// </summary>
        /// <example>Example message indicating what went wrong.</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        public string Message { get; set; }
    }
}