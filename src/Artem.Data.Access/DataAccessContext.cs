using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Artem.Data.Access {


    /// <summary>
    /// This is made for more proper additional use.
    /// </summary>
    internal class DataAccessContext : IDisposable {

        #region Delegates ///////////////////////////////////////////////////////////////

        private delegate void ExceptionDelivery(Exception ex);

        #endregion

        #region Static Fields ///////////////////////////////////////////////////////////

        private static DataAccessContext _Current;

        #endregion

        #region Static Properties ///////////////////////////////////////////////////////

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static DataAccessContext Current {
            get {
                if (_Current == null) {
                    _Current = new DataAccessContext();
                }
                return DataAccessContext._Current;
            }
        }
        #endregion

        #region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// Delivers the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected static internal void DeliverException(Exception ex) {

            ExceptionDelivery ed = new ExceptionDelivery(OnException);
            ed.BeginInvoke(ex, null, null);
        }

        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="ex">The ex.</param>
        private static void OnException(Exception ex) {

            throw ex;
        }
        #endregion

        #region Fields //////////////////////////////////////////////////////////////////

        private Dictionary<int, DataScope> _scopesTable = new Dictionary<int, DataScope>();
        private bool _isDisposed = false;

        #endregion

        #region Properties //////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the current scope.
        /// </summary>
        /// <value>The current scope.</value>
        public DataScope CurrentScope {
            get {
                if (_scopesTable.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                    return _scopesTable[Thread.CurrentThread.ManagedThreadId];
                return null;
            }
            set {
                if (value != null) {
                    _scopesTable[Thread.CurrentThread.ManagedThreadId] = value;
                }
                else {
                    _scopesTable.Remove(Thread.CurrentThread.ManagedThreadId);
                }
            }
        }
        #endregion

        #region Construct ///////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessContext"/> class.
        /// </summary>
        private DataAccessContext() {
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
                // dispose managed resources
                if (disposing) {
                    foreach (DataScope scope in _scopesTable.Values) {
                        scope.Dispose();
                    }
                    _scopesTable.Clear();
                    _scopesTable = null;
                }
                // dispose unmanaged resources
            }
            _isDisposed = true;
        }
        #endregion
        #endregion
    }
}
