using Dahomey.Json.Attributes;

namespace Models {
    /// <summary>
    /// The object for supplying login information
    /// </summary>
    public class Login {
        /// <summary>
        /// The email for logging in
        /// </summary>
        /// <example>henk@inholland.nl</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Email { get; set; }

        /// <summary>
        /// The password for this user
        /// </summary>
        /// <example>MySecretPassword</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Password { get; set; }
    }

}
