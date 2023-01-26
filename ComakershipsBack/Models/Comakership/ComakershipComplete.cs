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
    /// The ComakershipComplete object
    /// </summary>
    public class ComakershipComplete
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
        /// The current status of the Comakership
        /// </summary>
        /// <example>Not started</example>
        public ComakershipStatusGet Status { get; set; }

        /// <summary>
        /// The required skills
        /// </summary>
        public Collection<SkillGet> Skills { get; set;}

        /// <summary>
        /// The programs that the comakership is aimed at
        /// </summary>
        public Collection<ProgramGet> Programs { get; set; }

        /// <summary>
        /// The deliverables
        /// </summary>
        public Collection<DeliverableGet> Deliverables { get; set; }

        /// <summary>
        /// The students working on the comakership
        /// </summary>
        public Collection<StudentView> Students { get; set; }

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
