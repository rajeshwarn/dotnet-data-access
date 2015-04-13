using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//using System.Data.OracleClient;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for Oracle using MS .Net provider.
    /// </summary>
    public class MsOracleDataProvider : DataProviderBase {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "MsOracleDatabaseProvider";

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
        public MsOracleDataProvider()
            : base("System.Data.OracleClient", 
            "OracleConnection", "OracleCommand", "OracleDataAdapter", "OracleParameter") {
        }
        #endregion
    }
}
