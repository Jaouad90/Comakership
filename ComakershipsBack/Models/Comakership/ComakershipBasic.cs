using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;

namespace Models
{
    /// <summary>
    /// The ComakershipBasic object
    /// </summary>
    public class ComakershipBasic
    {
        /// <summary>
        /// The ID of the Comakership
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Comakership
        /// </summary>
        /// <example>An awesome Comakership</example>
        public string Name { get; set; }

        /// <summary>
        /// The description of the Comakership
        /// </summary>
        /// <example>We would like you to do some stuff</example>
        public string Description { get; set; }

        /// <summary>
        /// The company that the comakership belongs to
        /// </summary>
        /// <example></example>
        public CompanyVM Company { get; set; }

        /// <summary>
        /// The status that the comakership has
        /// </summary>
        /// <example></example>
        public ComakershipStatusGet Status { get; set; }

        /// <summary>
        /// The skills that are required for the Comakership
        /// </summary>
        /// <example>Java</example>
        public Collection<SkillGet> Skills { get; set; }

        /// <summary>
        /// The programs that the Comakership is aimed at
        /// </summary>
        /// <example>Java</example>
        public Collection<ProgramGet> Programs { get; set; }

        /// <summary>
        /// Is it possible to earn EC's?
        /// </summary>
        /// <example>False</example>
        public bool Credits { get; set; }

        /// <summary>
        /// Is there a bonus available on completion?
        /// </summary>
        /// <example>True</example>
        public bool Bonus { get; set; }

        /// <summary>
        /// The date the Comakership was created
        /// </summary>
        /// <example>2020-10-10</example>
        public DateTime CreatedAt { get; set; }
    }
}
