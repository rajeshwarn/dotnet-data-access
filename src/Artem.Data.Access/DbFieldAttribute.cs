using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbFieldAttribute : Attribute {

        #region Fields ////////////////////////////////////////////////////////

        private string _fieldName;
        private string _parameterName;

        #endregion

        #region Properties ////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public string FieldName {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ParameterName {
            get { return _parameterName; }
            set { _parameterName = value; }
        }
        #endregion

        #region Construct /////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        public DbFieldAttribute(string fieldName) {

            _fieldName = fieldName;
            _parameterName = "@p_" + fieldName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="parameterName"></param>
        public DbFieldAttribute(string fieldName, string parameterName) {

            _fieldName = fieldName;
            _parameterName = parameterName;
        }
        #endregion
    }
}
