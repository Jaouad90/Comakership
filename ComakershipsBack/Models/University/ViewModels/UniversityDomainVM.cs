using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    /// <summary>
    /// This is the viewmodel that is being returned when the universities/domains get queried.
    /// </summary>
    public class UniversityDomainVM
    {
        /// <summary>
        /// Contains the id of the university.
        /// </summary>
        /// <example>66</example>
        public int Id { get; set; }

        /// <summary>
		/// Contains the name of the university.
		/// </summary>
		/// <example>Inholland</example>
        public string Name { get; set; }

        /// <summary>
        /// Contains the domain of the university in the Comakerships App.
        /// </summary>
        /// <example>student.inholland.nl</example>
        public string Domain { get; set; }
    }
}
