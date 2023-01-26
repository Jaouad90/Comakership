using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    public class StudentPutVM {

        /// <summary>
        /// Name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the student
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// Links from the student
        /// </summary>
        public ICollection<string> Links { get; set; }

        /// <summary>
        /// Nickname of the student
        /// </summary>
        /// <example>xXxCoolStudentxXx</example>
        public string Nickname { get; set; }
    }
}
