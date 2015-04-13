using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for MySQL.
    /// </summary>
    public class MySqlDataProvider : DataProviderBase {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "MySqlDatabaseProvider";

        #endregion

        #region Properties //////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public override string Name {
            get {
                return (base.Name != null) ? base.Name : DefaultName;
            }
        }
        #endregion

        #region Construct ///////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public MySqlDataProvider()
            : base("MySql.Data", 
            "MySqlConnection", "MySqlCommand", "MySqlDataAdapter", "MySqlParameter") {
        }
        #endregion
    }
}
