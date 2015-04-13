using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Artem.Data.Access.Config {

    /// <summary>
    /// The database access configuration section handler.
    /// </summary>
    public class DataAccessSection : ConfigurationSection {

        #region Fields ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// The name of database access configuration section.
        /// </summary>
        public const string SectionName = "database.access";

        #endregion

        #region Properties ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Handle the defaultProvider configuration attribute.
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultProvider",
            DefaultValue = DataAccess.Default.ProviderName)]
        public string DefaultProvider {
            get { return (string)base["defaultProvider"]; }
            set { base["defaultProvider"] = value; }
        }

        /// <summary>
        /// Handle the defaultCommandType configuration attribute.
        /// Default value is 'StoredProcedure', otherwise can be set to 'StoredProcedure',
        /// 'TableDirect' or 'Text'.
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultCommandType",
           DefaultValue = DataAccess.Default.CommandType)]
        public string DefaultCommandType {
            get { return (string)base["defaultCommandType"]; }
            set { base["defaultCommandType"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [StringValidator(MinLength = 1)]
        [ConfigurationProperty("defaultConnectionName",
           DefaultValue = DataAccess.Default.ConnectionName)]
        public string DefaultConnectionName {
            get { return (string)base["defaultConnectionName"]; }
            set { base["defaultConnectionName"] = value; }
        }

        ///// <summary>
        ///// Handle the defaultRollbackBehaviour configuration attribute.
        ///// Default value is <code>RollbackBehaviour.RollbackCurrent</code>
        ///// 	<see cref="Artem.Data.Access.RollbackBehaviour"/>
        ///// </summary>
        ///// <value>The default rollback behaviour.</value>
        //[StringValidator(MinLength = 1)]
        //[ConfigurationProperty("defaultRollbackBehaviour",
        //    DefaultValue = Database.Default.RollbackBehaviour)]
        //public string DefaultRollbackBehaviour {
        //    get { return (string)base["defaultRollbackBehaviour"]; }
        //    set { base["defaultRollbackBehaviour"] = value; }
        //}

        /// <summary>
        /// Handle the providers' collection configuration element.
        /// </summary>
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }
        #endregion        
    }
}
