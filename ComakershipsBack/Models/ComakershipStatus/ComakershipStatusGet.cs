using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// The ComakershipStatusGet object
    /// </summary>
    public class ComakershipStatusGet
    {
        /// <summary>
        /// The ID the ComakershipStatus
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the status
        /// </summary>
        /// <example>Not started</example>
        public string Name { get; set; }
    }
}
