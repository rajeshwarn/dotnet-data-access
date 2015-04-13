using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Text;

namespace Artem.Data.Access {
    
    /// <summary>
    /// 
    /// </summary>
    internal static class ProvidersHelper {

        #region Static Methods //////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="type"></param>
        public static void CheckConnection(IDbConnection connection, Type type) {

            if (connection == null)
                throw new DataAccessException(SR.ERR_0);
            //if (connection.State != ConnectionState.Open)
            //    throw DatabaseProviderException.ConnectionIsNotOpen;
            Type __connectionType = connection.GetType();
            if (__connectionType != type)
                throw new DataAccessException(DataAccessError.Connection_WrongType, __connectionType, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="type"></param>
        public static void CheckTransaction(IDbTransaction transaction, Type type) {

            if (transaction != null) {
                Type __transactionType = transaction.GetType();
                if (__transactionType != type) {
                    throw new DataAccessException(DataAccessError.Transaction_WrongType, __transactionType, type);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="config"></param>
        public static void Initialize(DataProvider provider, NameValueCollection config) {

            if (config == null) {
                throw new ArgumentNullException("config");
            }
            // get connection string
            string __connectionName = config["connectionName"];
            if (string.IsNullOrEmpty(__connectionName)) {
                __connectionName = DataAccess.Default.ConnectionName;
            }
            else {
                __connectionName = __connectionName.Trim();
            }
            ConnectionStringSettings __connectionSettings =
                ConfigurationManager.ConnectionStrings[__connectionName];
            //if (__connectionSettings == null || string.IsNullOrEmpty(__connectionSettings.ConnectionString)) {
                
            //    throw new ProviderException(string.Format(
            //        SR.ERR_2 + "<br>" + AppDomain.CurrentDomain.BaseDirectory + "<br>" + 
            //        AppDomain.CurrentDomain.DynamicDirectory + "<br>" +
            //        AppDomain.CurrentDomain.RelativeSearchPath + "<br><br>" +
            //        AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "<br>" +
            //        AppDomain.CurrentDomain.SetupInformation.ApplicationName + "<br>" +
            //        AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, __connectionName));
            //}
            provider.ConnectionName = __connectionName;
            // get default command type
            provider.DefaultCommandType = ParseCommandType(config["defaultCommandType"]);
            // get default rollback behaviour            
            //provider.DefaultRollbackBehaviour = ParseRollbackBehaviour(config["defaultRollbackBehaviour"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ProviderBase Instantiate(
            Config.DataAccessSection config, ProviderSettings settings, Type providerType) {

            ProviderBase __base = null;
            try {
                string __typeName = (settings.Type == null) ? null : settings.Type.Trim();
                if (string.IsNullOrEmpty(__typeName)) {
                    throw new ArgumentException("Provider doesn't have type name");
                }
                Type __type = Type.GetType(__typeName); //ConfigUtil.GetType(text1, "type", settings);
                if (!providerType.IsAssignableFrom(__type)) {
                    throw new ArgumentException(string.Format(
                        "Provider must implement type {0}", providerType.ToString()));
                }
                __base = (ProviderBase)Activator.CreateInstance(__type);
                NameValueCollection __parameters = settings.Parameters;
                NameValueCollection __providerConfig = new NameValueCollection(__parameters.Count, StringComparer.InvariantCulture);
                foreach (string __param in __parameters) {
                    __providerConfig[__param] = __parameters[__param];
                }
                if (__providerConfig["connectionName"] == null) {
                    __providerConfig["connectionName"] = config.DefaultConnectionName;
                }
                if (__providerConfig["defaultCommandType"] == null) {
                    __providerConfig["defaultCommandType"] = config.DefaultCommandType;
                }
                //if (__providerConfig["defaultRollbackBehaviour"] == null) {
                //    __providerConfig["defaultRollbackBehaviour"] = config.DefaultRollbackBehaviour;
                //}
                __base.Initialize(settings.Name, __providerConfig);
            }
            catch (Exception exception1) {
                if (exception1 is ConfigurationException) {
                    throw;
                }
                throw new ConfigurationErrorsException(exception1.Message, settings.ElementInformation.Properties["type"].Source, settings.ElementInformation.Properties["type"].LineNumber);
            }
            return __base;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Instantiate(
            Config.DataAccessSection config, ProviderCollection providers, Type providerType) {

            foreach (ProviderSettings settings in config.Providers) {
                providers.Add(Instantiate(config, settings, providerType));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static CommandType ParseCommandType(string commandType) {

            if (string.IsNullOrEmpty(commandType)) {
                commandType = DataAccess.Default.CommandType;
            }
            switch (commandType.Trim().ToLower()) {
                case "storedprocedure":
                    return System.Data.CommandType.StoredProcedure;
                case "tabledirect":
                    return System.Data.CommandType.TableDirect;
                case "text":
                    return System.Data.CommandType.Text;
                default:
                    throw new ProviderException(string.Format(
                        SR.ERR_200, commandType));
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="rollbackBehaviour"></param>
        ///// <returns></returns>
        //public static RollbackBehaviour ParseRollbackBehaviour(string rollbackBehaviour) {

        //    if (string.IsNullOrEmpty(rollbackBehaviour)) {
        //        rollbackBehaviour = Database.Default.RollbackBehaviour;
        //    }
        //    switch (rollbackBehaviour.Trim().ToLower()) {
        //        case "all":
        //            return RollbackBehaviour.RollbackAll;
        //        case "current":
        //            return RollbackBehaviour.RollbackCurrent;
        //        default:
        //            throw new ProviderException(string.Format(
        //                SR.Rollback_behaviour_is_not_correct, rollbackBehaviour));
        //    }
        //}
        #endregion
    }
}