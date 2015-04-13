using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// The base class for database providers.
    /// </summary>
    public abstract class DataProvider : ProviderBase {

        #region Fields //////////////////////////////////////////////////////////////////

        string _connectionName;
        CommandType _defaultCommandType = CommandType.Text;
        RollbackBehaviour _defaultRollbackBehaviour = RollbackBehaviour.RollbackCurrent;

        #endregion

        #region Properties //////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionName {
            get {
                if (string.IsNullOrEmpty(_connectionName)) {
                    _connectionName = DataAccess.Default.ConnectionName;
                }
                return _connectionName;
            }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    this._connectionName = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual CommandType DefaultCommandType {
            get { return _defaultCommandType; }
            set { _defaultCommandType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual RollbackBehaviour DefaultRollbackBehaviour {
            get { return _defaultRollbackBehaviour; }
            set { _defaultRollbackBehaviour = value; }
        }
        #endregion

        #region Construct ///////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        protected DataProvider() {
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
        public abstract IDbCommand CreateCommand(
            string commandText, CommandType commandType, 
            IDbConnection connection, IDbTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand(
            string commandText, CommandType commandType, IDbConnection connection) {

            return this.CreateCommand(commandText, commandType, connection, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDbConnection CreateConnection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDbDataParameter CreateParameter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract IDbDataParameter CreateParameter(string name, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, int size, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config) {
            base.Initialize(name, config);
            ProvidersHelper.Initialize(this, config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual bool IsTableEmpty(string tableName) {

            string __sql = string.Format("select count(*) from {0}", tableName);
            using (IDbConnection __connection = this.CreateConnection()) {
                __connection.ConnectionString = DataAccess.ConnectionStringSettings[this.ConnectionName].ConnectionString;
                __connection.Open();
                try {
                    using (IDbCommand __command = this.CreateCommand(__sql, CommandType.Text, __connection)) {
                        __command.CommandType = CommandType.Text;
                        return DbDataConvert.ToInt(__command.ExecuteScalar()) == 0;
                    }
                }
                finally {
                    try {
                        __connection.Close();
                    }
                    catch { }
                }
            }
        }
        #endregion
    }
}
