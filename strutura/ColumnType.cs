using System;
using System.Collections.Generic;
using System.Text;

namespace Gerador.strutura
{

    public enum ColumnType
    {
        /// <summary>
        /// Coluna do tipo string
        /// </summary>
        BOColumnString,
        /// <summary>
        /// Coluna do tipo Long
        /// </summary>
        BOColumnLong,
        /// <summary>
        /// Coluna do tipo Double
        /// </summary>
        BOColumnDouble,
        /// <summary>
        /// Coluna do tipo Date
        /// </summary>
        BOColumnDate,
        /// <summary>
        /// Coluna do tipo Time
        /// </summary>
//        BOColumnTime,
        /// <summary>
        /// Coluna do tipo DateTime
        /// </summary>
        BOColumnDateTime,
        /// <summary>
        /// Coluna do tipo Bool
        /// </summary>
        BOColumnBool
    }
}
