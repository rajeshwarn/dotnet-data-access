using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    public partial class DataAccess {

        #region Class Members ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        internal struct Default {
            public const int CommandsSize = 8;
            public const string ConnectionName = "connection.string";
            public const string ProviderName = "SqlDataProvider";
            public const string CommandType = "StoredProcedure";
            public const string RollbackBehaviour = "Current";
        }
        #endregion

        #region Static Fields ///////////////////////////////////////////////////////////

        static readonly object _SyncLock = new object();

        static DataProvider _Provider;
        static DataProviderCollection _Providers;
        static Exception _InitializeException = null;
        static bool _IsInitialized = false;
        static ConnectionStringSettingsCollection _ConnectionStringSettingsCollection;
        static string _DefaultProviderName;
        static bool _TraceEnabled;

        #endregion

        #region Static Properties ///////////////////////////////////////////////////////

        /// <summary>
        /// Gets the connection string settings.
        /// </summary>
        /// <value>The connection string settings.</value>
        public static ConnectionStringSettingsCollection ConnectionStringSettings {
            get {
                if (_ConnectionStringSettingsCollection == null) {
                    _ConnectionStringSettingsCollection = new ConnectionStringSettingsCollection();
                    foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings) {
                        _ConnectionStringSettingsCollection.Add(settings);
                    }
                }
                return _ConnectionStringSettingsCollection;
            }
        }

        /// <summary>
        /// Gets or sets the name of the default provider.
        /// </summary>
        /// <value>The name of the default provider.</value>
        public static string DefaultProviderName {
            get {
                if (string.IsNullOrEmpty(_DefaultProviderName)) {
                    _DefaultProviderName = Default.ProviderName;
                }
                return _DefaultProviderName;
            }
            set {
                _DefaultProviderName = value;
                _InitializeException = null;
                _IsInitialized = false;
            }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public static DataProvider Provider {
            get {
                Initialize();
                return _Provider;
            }
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public static DataProviderCollection Providers {
            get {
                Initialize();
                return _Providers;
            }
        }

        /// <summary>
        /// Gets the trace.
        /// </summary>
        /// <value>The trace.</value>
        public static System.Web.TraceContext Trace {
            get {
                if (System.Web.HttpContext.Current != null) {
                    return System.Web.HttpContext.Current.Trace;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [trace enabled].
        /// </summary>
        /// <value><c>true</c> if [trace enabled]; otherwise, <c>false</c>.</value>
        public static bool TraceEnabled {
            get { return _TraceEnabled; }
        }
        #endregion

        #region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter() {

            return DataAccess.Provider.CreateParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter(string name, object value) {

            return DataAccess.Provider.CreateParameter(name, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, object value) {

            return DataAccess.Provider.CreateParameter(name, dbType, direction, value);
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
        public static IDbDataParameter CreateParameter(
            string name, int dbType, ParameterDirection direction, int size, object value) {

            return DataAccess.Provider.CreateParameter(name, dbType, direction, size, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateReturnParameter(string name, int dbType) {

            return DataAccess.Provider.CreateParameter(
                name, dbType, ParameterDirection.ReturnValue, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateReturnParameter(string name, object value) {

            IDbDataParameter __parameter = DataAccess.Provider.CreateParameter(name, value);
            __parameter.Direction = ParameterDirection.ReturnValue;
            return __parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void Initialize() {

            if (_IsInitialized) {
                ThrowOnInitializeException();
            }
            else {
                ThrowOnInitializeException();
                lock (_SyncLock) {
                    if (_IsInitialized) {
                        ThrowOnInitializeException();
                    }
                    else {
                        System.Web.HttpContext context = System.Web.HttpContext.Current;
                        _TraceEnabled = (context != null && context.Trace.IsEnabled) ? true : false;
                        Config.DataAccessSection __section = (Config.DataAccessSection)
                            ConfigurationManager.GetSection(Config.DataAccessSection.SectionName);
                        _Providers = new DataProviderCollection();
                        string __defaultProvider;
                        // will not throw exception, but apply the general default provider rule
                        if (__section == null) {
                            __section = new Config.DataAccessSection();
                        }
                        else {
                            ProvidersHelper.Instantiate(
                                __section, _Providers, typeof(DataProvider));
                        }
                        // set default provider
                        __defaultProvider = __section.DefaultProvider;
                        _Provider = _Providers[__defaultProvider];
                        if (_Provider == null) {
                            // try to setup from implemented set of providers
                            TrySetupDefaultProvider(__defaultProvider, __section);
                        }
                        ThrowOnNullDefaultProvider();
                        _IsInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsTableEmpty(string tableName) {
            return Provider.IsTableEmpty(tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ThrowOnInitializeException() {

            if (_InitializeException != null) {
                throw _InitializeException;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ThrowOnNullDefaultProvider() {

            if (_Provider == null) {
                throw new ProviderException("Unable to load default DatabaseProvider");
            }
        }

        /// <summary>
        /// Tries the setup default provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="config">The config.</param>
        private static void TrySetupDefaultProvider(string providerName, Config.DataAccessSection config) {

            // try to setup from implemented set of providers
            switch (providerName.Trim()) {
#if FB_BUILD
                case "FbDataProvider":
                    _Provider = new Providers.FbDataProvider();
                    break;
#endif
                case "MsOracleDataProvider":
                    _Provider = new Providers.MsOracleDataProvider();
                    break;
                case "MySqlDataProvider":
                    _Provider = new Providers.MySqlDataProvider();
                    break;
                case "OdbcDataProvider":
                    _Provider = new Providers.OdbcDataProvider();
                    break;
                case "OleDbDataProvider":
                    _Provider = new Providers.OleDbDataProvider();
                    break;
                case "OracleDataProvider":
                    _Provider = new Providers.OracleDataProvider();
                    break;
                case "SqlDataProvider":
                    _Provider = new Providers.SqlDataProvider();
                    break;
                default:
                    // not found exit;
                    return;
            }
            NameValueCollection __providerConfig = new NameValueCollection();
            __providerConfig["connectionName"] = config.DefaultConnectionName;
            __providerConfig["defaultCommandType"] = config.DefaultCommandType;
            //__providerConfig["defaultRollbackBehaviour"] = config.DefaultRollbackBehaviour;
            _Provider.Initialize(config.DefaultProvider, __providerConfig);
            // add it to collection
            _Providers.Add(_Provider);
        }
        #endregion
    }
}
