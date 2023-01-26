using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dahomey.Json.Attributes;

namespace Models
{
    /// <summary>
    /// The StudentView object
    /// </summary>
    public class StudentView
    {
        /// <summary>
        /// The ID of the Student
        /// </summary>
        /// <example>2</example>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Student
        /// </summary>
        /// <example>Bob</example>
        public string Name { get; set; }

        /// <summary>
        /// The email of the Student
        /// </summary>
        /// <example>bob@email.com</example>
        public string Email { get; set; }
    }
}
