#if FB_BUILD
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// A database provider for Firebird.
    /// </summary>
    public class FbDataProvider : DataProvider {

        #region Static Fields ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public const string DefaultName = "FbDataProvider";

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

        //#region Construct ///////////////////////////////////////////////////////////////

        ///// <summary>
        ///// 
        ///// </summary>
        //public FbDataProvider()
        //    : base("FirebirdSql.Data.FirebirdClient",
        //    "FbConnection", "FbCommand", "FbDataAdapter", "FbParameter") {
        //}
        //#endregion

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

            ProvidersHelper.CheckConnection(connection, typeof(FbConnection));
            if (transaction != null) {
                ProvidersHelper.CheckTransaction(transaction, typeof(FbTransaction));
            }
            FbCommand __command = new FbCommand(commandText, (FbConnection)connection);
            __command.CommandType = commandType;
            if (transaction != null) {
                __command.Transaction = (FbTransaction)transaction;
            }
            return __command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbConnection CreateConnection() {

            return new FbConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbDataAdapter CreateDataAdapter() {

            return new FbDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Data.IDbDataParameter CreateParameter() {

            return new FbParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override System.Data.IDbDataParameter CreateParameter(string name, object value) {

            return new FbParameter(name, value);
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

            FbParameter __parameter = new FbParameter(name, (SqlDbType)dbType);
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

            FbParameter __parameter = new FbParameter(name, (FbDbType)dbType, size);
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }
        #endregion
    }
}
#endif