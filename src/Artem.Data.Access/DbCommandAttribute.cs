using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;


namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DbCommandAttribute : Attribute {

        #region Fields ////////////////////////////////////////////////////////

        private string _commandName;
        private string _commandText;
        private CommandType _commandType = CommandType.StoredProcedure;
        private string[] _parameters;

        #endregion

        #region Properties ////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public string CommandName {
            get {
                return _commandName;
            }
            set {
                _commandName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText {
            get {
                return _commandText;
            }
            set {
                _commandText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType {
            get {
                return _commandType;
            }
            set {
                _commandType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] Parameters {
            get {
                return _parameters;
            }
            set {
                _parameters = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override object TypeId {
            get { 
                return this.GetHashCode(); 
            }
        }
        #endregion

        #region Construct /////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public DbCommandAttribute(string name, params string[] parameters) {

            _commandName = name;
            _commandText = name;
            _parameters = parameters;
        }
        #endregion
    }
}
