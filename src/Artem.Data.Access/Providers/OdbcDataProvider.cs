using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;


namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for ODBC
    /// </summary>
    public class OdbcDataProvider : DataProvider {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "OdbcDatabaseProvider";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override IDbCommand CreateCommand(
            string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction) {

            ProvidersHelper.CheckConnection(connection, typeof(OdbcConnection));
            ProvidersHelper.CheckTransaction(transaction, typeof(OdbcTransaction));
            OdbcCommand __command = new OdbcCommand(commandText, (OdbcConnection)connection);
            __command.CommandType = commandType;
            if (transaction != null) {
                __command.Transaction = (OdbcTransaction)transaction;
            }
            return __command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbConnection CreateConnection() {
            return new OdbcConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter() {
            return new OdbcDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter() {
            return new OdbcParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string name, object value) {
            return new OdbcParameter(name, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, object value) {

            OdbcParameter __parameter = new OdbcParameter(name, (OdbcType)dbType);
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, int size, object value) {

            OdbcParameter __parameter = new OdbcParameter(name, (OdbcType)dbType, size);
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }
        #endregion
    }
}
