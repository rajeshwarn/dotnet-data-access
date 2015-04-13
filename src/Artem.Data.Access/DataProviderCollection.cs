using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    public class DataProviderCollection : ProviderCollection {

        #region Properties ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new DataProvider this[string name] {
            get { return(DataProvider)base[name]; }
        }
        #endregion        

        #region Methods //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public override void Add(ProviderBase provider) {

            if (provider == null)
                throw new ArgumentNullException("provider");
            if (!(provider is DataProvider))
                throw new ArgumentException("Invalid provider type", "provider");
            base.Add(provider);
        }
        #endregion
    }
}
