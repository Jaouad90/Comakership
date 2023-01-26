using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CompanyDeleteVM
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public CompanyDeleteVM(int id)
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
