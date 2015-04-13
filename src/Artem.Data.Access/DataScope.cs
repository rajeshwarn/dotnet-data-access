using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    public class DataScope : IDisposable {

        #region Static Properties ///////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        static public DataScope Current {
            get {
                return DataAccessContext.Current.CurrentScope;
            }
            set {
                DataAccessContext.Current.CurrentScope = value;
            }
        }
        #endregion

        #region Static Methods //////////////////////////////////////////////////////////

        static void ThrowIfNestedScope() {

            if (DataScope.Current != null)
                throw new DataAccessException(SR.ERR_500);

        }
        #endregion

        #region Fields //////////////////////////////////////////////////////////////////

        IDbConnection _connection;
        IDbTransaction _transaction;
        Exception _innerException;
        bool _isDisposed;
        bool _isComplete;

        #endregion

        #region Properties //////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        protected internal IDbConnection Connection {
            get { return _connection; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has exception.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has exception; otherwise, <c>false</c>.
        /// </value>
        public bool HasException {
            get { return _innerException != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has transaction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has transaction; otherwise, <c>false</c>.
        /// </value>
        private bool HasTransaction {
            get { return _transaction != null; }
        }

        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>The inner exception.</value>
        public Exception InnerException {
            get { return _innerException; }
            set { _innerException = value; }
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        protected internal IDbTransaction Transaction {
            get { return _transaction; }
        }
        #endregion

        #region Construct / Destruct ////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="DataScope"/> class.
        /// </summary>
        public DataScope() {

            ThrowIfNestedScope();
            _connection = DataAccess.Provider.CreateConnection();
            _connection.ConnectionString =
                DataAccess.ConnectionStringSettings[DataAccess.Provider.ConnectionName].ConnectionString;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            DataScope.Current = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataScope"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        public DataScope(IsolationLevel level) {

            ThrowIfNestedScope();
            _connection = DataAccess.Provider.CreateConnection();
            _connection.ConnectionString =
                DataAccess.ConnectionStringSettings[DataAccess.Provider.ConnectionName].ConnectionString;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            DataScope.Current = this;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        protected virtual void Dispose(bool disposing) {

            if (!_isDisposed) {
                _isDisposed = true;
                if (disposing) {
                    try {
                        if (HasException) {
                            this.Rollback(true);
                        }
                        else {
                            this.Commit(true);
                        }
                    }
                    catch (Exception ex) {
                        this._innerException = ex;
                    }
                    try {
                        this.CloseConnection();
                    }
                    catch (Exception ex1) {
                        this._innerException = ex1;
                    }
                    if (HasException) {
                        DataAccessContext.DeliverException(this.InnerException);
                    }
                    DataScope.Current = null;
                }
            }
        }
        #endregion
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Closes the used connection.
        /// </summary>
        private void CloseConnection() {

            if (_connection != null && _connection.State != ConnectionState.Closed) {
                //try {
                    _connection.Close();
                //}
                //catch { }
            }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit() {

            this.Commit(false);
        }

        /// <summary>
        /// Commits the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        protected void Commit(bool disposing) {

            if (this.HasTransaction) {
                if (!_isComplete) {
                    _transaction.Commit();
                    _isComplete = true;
                }
                if (disposing) {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Rollbacks this instance.
        /// </summary>
        public void Rollback() {

            this.Rollback(false);
        }

        /// <summary>
        /// Rollbacks this instance.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        public void Rollback(bool disposing) {

            if (this.HasTransaction) {
                if (!_isComplete) {
                    _transaction.Rollback();
                    _isComplete = true;
                }
                if (disposing) {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }
        #endregion
    }
}
