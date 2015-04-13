using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Artem.Data.Access.Providers {

    /// <summary>
    /// 
    /// </summary>
    public class DataProviderBase : DataProvider {

        #region Fields //////////////////////////////////////////////////////////////////

        string _assembly;
        string _connectionTypeName;
        string _commandTypeName;
        string _adapterTypeName;
        string _parameterTypeName;

        #endregion

        #region Construct ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProviderBase"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="connectionTypeName">Name of the connection type.</param>
        /// <param name="commandTypeName">Name of the command type.</param>
        /// <param name="adapterTypeName">Name of the adapter type.</param>
        /// <param name="parameterTypeName">Name of the parameter type.</param>
        public DataProviderBase(string assembly, string connectionTypeName,
            string commandTypeName, string adapterTypeName, string parameterTypeName)
            : base() {

            _assembly = assembly;
            _connectionTypeName = connectionTypeName;
            _commandTypeName = commandTypeName;
            _adapterTypeName = adapterTypeName;
            _parameterTypeName = parameterTypeName;
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

            Type __type = this.ActivateType(_connectionTypeName);
            ProvidersHelper.CheckConnection(connection, __type);
            IDbCommand __command = (IDbCommand)this.ActivateObject(
                _commandTypeName, new object[] { commandText, connection });
            __command.CommandType = commandType;
            if (transaction != null) {
                __command.Transaction = transaction;
            }
            return __command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbConnection CreateConnection() {

            return (IDbConnection)this.ActivateObject(_connectionTypeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataAdapter CreateDataAdapter() {

            return (IDbDataAdapter)this.ActivateObject(_adapterTypeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter() {

            return (IDbDataParameter)this.ActivateObject(_parameterTypeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string name, object value) {

            return (IDbDataParameter)this.ActivateObject(
                _parameterTypeName, new object[] { name, value });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string name, int dbType, ParameterDirection direction, object value) {

            IDbDataParameter __parameter = (IDbDataParameter)
                this.ActivateObject(_parameterTypeName, new object[] { name, dbType });
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

            IDbDataParameter __parameter = (IDbDataParameter)
                this.ActivateObject(_parameterTypeName, new object[] { name, dbType, size });
            __parameter.Direction = direction;
            __parameter.Value = value;
            return __parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override bool IsTableEmpty(string tableName) {

            string __sql = string.Format("select count(*) from {0}", tableName);
            using (IDbConnection __connection = this.CreateConnection()) {
                __connection.ConnectionString = DataAccess.ConnectionStringSettings[this.ConnectionName].ConnectionString;
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

        #region Utility Methods 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="activationAttributes"></param>
        /// <returns></returns>
        private object ActivateObject(string name, params object[] activationAttributes) {

            //Type type = Type.GetType(BuildQualifiedName(name));
            //System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            //if (constructor != null) {
            //    return constructor.Invoke(null);
            //}
            //return null;
            return Activator.CreateInstanceFrom(
                _assembly, this.BuildQualifiedName(name), activationAttributes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Type ActivateType(string name) {

            return Type.GetType(BuildFullQualifiedName(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string BuildQualifiedName(string name) {

            return string.Format("{0}.{1}", _assembly, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string BuildFullQualifiedName(string name) {

            return string.Format("{0}.{1}, {0}", _assembly, name);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //private string CheckParameterName(string name) {

        //    if (!name.StartsWith("@")) {
        //        return string.Concat("@", name);
        //    }
        //    return name;
        //}
        #endregion
        #endregion
    }
}
