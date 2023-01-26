using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;

namespace Models
{
    /// <summary>
    /// This object contains all the data related to the ComakershipPostModel
    /// </summary>
    public class ComakershipPost
    {
        /// <summary>
        /// The name of the Comakership
        /// </summary>
        /// <example>An awesome Comakership</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the Comakership
        /// </summary>
        /// <example>We would like you to do some stuff</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Is it possible to earn EC's?
        /// </summary>
        /// <example>False</example>
        [JsonRequired(RequirementPolicy.Always)]
        public bool Credits { get; set; }

        /// <summary>
        /// Is there a bonus available on completion?
        /// </summary>
        /// <example>True</example>
        [JsonRequired(RequirementPolicy.Always)]
        public bool Bonus { get; set; }

        /// <summary>
        /// The tasks that need to be done for the Comakership
        /// </summary>
        /// <example></example>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public Collection<DeliverablePost> Deliverables { get; set; }


        /// <summary>
        /// The skills that are required for the Comakership
        /// </summary>
        /// <example>Java</example>
        [JsonRequired(RequirementPolicy.AllowNull)]
        public Collection<SkillPost> Skills { get; set; }

        /// <summary>
        /// The programs that the Comakership is aimed at
        /// </summary>
        /// <example>Information Technology</example>
        [JsonRequired(RequirementPolicy.Always)]
        public Collection<int> ProgramIds { get; set; }

        /// <summary>
        /// The purchasekey that will be used to validate the post
        /// </summary>
        /// <example>ABC123</example>
        [JsonRequired(RequirementPolicy.Always)]
        public string PurchaseKey { get; set; }
    }
}
