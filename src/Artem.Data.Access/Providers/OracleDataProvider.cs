using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for Oracle using Oracle .Net provider.
    /// </summary>
    class OracleDataProvider : DataProvider {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "OracleDatabaseProvider";

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

        #region Methods /////////////////////////////////////////////////////////////////

        public override System.Data.IDbCommand CreateCommand(string commandText, System.Data.CommandType commandType, System.Data.IDbConnection connection, System.Data.IDbTransaction transaction) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbConnection CreateConnection() {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbDataAdapter CreateDataAdapter() {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbDataParameter CreateParameter() {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbDataParameter CreateParameter(string name, object value) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbDataParameter CreateParameter(string name, int dbType, System.Data.ParameterDirection direction, object value) {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.IDbDataParameter CreateParameter(string name, int dbType, System.Data.ParameterDirection direction, int size, object value) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
