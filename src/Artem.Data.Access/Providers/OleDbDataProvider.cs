using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for OleDB
    /// </summary>
    public class OleDbDataProvider : DataProvider {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        static readonly string DefaultName = "OleDbDataProvider";
        static readonly string DefaultDescription = "Implementation of DataProvider for OleDb.";

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

            ProvidersHelper.CheckConnection(connection, typeof(OleDbConnection));
            ProvidersHelper.CheckTransaction(transaction, typeof(OleDbTransaction));
            OleDbCommand __command = new OleDbCommand(commandText, (OleDbConnection)connection);
            __command.CommandType = commandType;
            if (transaction != null) {
                __command.Transaction = (OleDbTransaction)transaction;
            }
            return __command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbConnection CreateConnection() {
            return new OleDbConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter() {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter() {
            return new OleDbParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string name, object value) {
            return new OleDbParameter(name, value);
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

            OleDbParameter __parameter = new OleDbParameter(name, (OleDbType)dbType);
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

            OleDbParameter __parameter = new OleDbParameter(name, (OleDbType)dbType, size);
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }

        #region - Init -

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config) {

            if (string.IsNullOrEmpty(name))
                name = DefaultName;
            if (config["description"] == null)
                config["description"] = DefaultDescription;
            ///
            base.Initialize(name, config);
        }
        #endregion
        #endregion
    }
}
