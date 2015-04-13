using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Artem.Data.Access {

    internal enum DataAccessError {
        Connection_NullReference = 0,
        Connection_WrongType = 1,
        Connection_StringNotFound = 2,
        Transaction_WrongType = 100,
        Transaction_CanNotBeInScope = 101,
        Command_TypeNotCorrect = 200,
        Scope_CanNotBeNested = 500,
        DatabaseAccessConfig_SectionNotFound = 1000
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataAccessException : Exception {

        #region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        static string GetMessage(DataAccessError error) {
            return SR.ResourceManager.GetString("ERR_" + ((int)error).ToString());
        }
        #endregion

        #region Construct ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        public DataAccessException()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        internal DataAccessException(DataAccessError error)
            : base(GetMessage(error)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="values">The values.</param>
        internal DataAccessException(DataAccessError error, params object[] values)
            : base(string.Format(GetMessage(error), values)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="innerException">The inner exception.</param>
        internal DataAccessException(DataAccessError error, Exception innerException)
            : base(GetMessage(error), innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="values">The values.</param>
        internal DataAccessException(DataAccessError error, Exception innerException, params object[] values)
            : base(string.Format(GetMessage(error), values), innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataAccessException(string message) : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        public DataAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
        #endregion
    }
}
