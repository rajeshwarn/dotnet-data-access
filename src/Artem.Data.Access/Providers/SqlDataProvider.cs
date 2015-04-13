using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for MS SQL.
    /// </summary>
    public class SqlDataProvider : DataProvider {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "SqlDataProvider";

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
        public override System.Data.IDbCommand CreateCommand(
            string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction) {

            ProvidersHelper.CheckConnection(connection, typeof(SqlConnection));
            if (transaction != null) {
                ProvidersHelper.CheckTransaction(transaction, typeof(SqlTransaction));
            }
            SqlCommand __command = new SqlCommand(commandText, (SqlConnection)connection);
            __command.CommandType = commandType;
            if (transaction != null) {
                __command.Transaction = (SqlTransaction)transaction;
            }
            return __command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbConnection CreateConnection() {

            return new SqlConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbDataAdapter CreateDataAdapter() {

            return new SqlDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbDataParameter CreateParameter() {

            return new SqlParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override System.Data.IDbDataParameter CreateParameter(string name, object value) {

            return new SqlParameter(name, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override System.Data.IDbDataParameter CreateParameter(
            string name, int dbType, System.Data.ParameterDirection direction, object value) {

            SqlParameter __parameter = new SqlParameter(name, (SqlDbType)dbType);
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
        public override System.Data.IDbDataParameter CreateParameter(
            string name, int dbType, System.Data.ParameterDirection direction, int size, object value) {

            SqlParameter __parameter = new SqlParameter(name, (SqlDbType)dbType, size);
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }
        #endregion
    }
}
