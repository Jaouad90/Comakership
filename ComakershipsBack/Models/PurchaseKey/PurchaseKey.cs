using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    /// <summary>
    /// The PurchaseKey object
    /// </summary>
    public class PurchaseKey : IEntityBase
    {
        /// <summary>
        /// The id of the PurchaseKey
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The actual key of the PurchaseKey
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// The state of the PurchaseKey
        /// </summary>
        [Required]
        public bool Claimed { get; set; }
    }
}
