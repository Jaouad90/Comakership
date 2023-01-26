using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    /// <summary>
    /// The PurchaseKeyPost object
    /// </summary>
    public class PurchaseKeyPost
    {        
        /// <summary>
        /// The actual key of the PurchaseKey
        /// </summary>
        [Required]
        public string Key { get; set; }        
    }
}
