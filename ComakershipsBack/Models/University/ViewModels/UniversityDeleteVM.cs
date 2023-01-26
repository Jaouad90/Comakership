using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    /// <summary>
    /// ViewModel for a DELETE request.
    /// </summary>
    public class UniversityDeleteVM
    {
        /// <summary>
        /// Ctor to create a new Delete ViewModel.
        /// </summary>
        /// <param name="id"></param>
        public UniversityDeleteVM(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Contains the id of the university.
        /// </summary>
        /// <example>66</example>
        public int Id { get; set; }
    }
}
