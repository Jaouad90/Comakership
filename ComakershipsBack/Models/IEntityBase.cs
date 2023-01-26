using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    /// <summary>
    /// Base entity for all entities
    /// </summary>
    public interface IEntityBase {
        /// <summary>
        /// Id for this entity
        /// </summary>
        int Id { get; set; }
    }
}
