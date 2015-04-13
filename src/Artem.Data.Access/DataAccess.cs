using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.Common;


namespace Artem.Data.Access {

    /// <summary>
    /// Database utility object. 
    /// </summary>
    public partial class DataAccess : IDisposable {

        #region Fields //////////////////////////////////////////////////////////////////

        IDbConnection _connection;
        IDbTransaction _transaction;
        //List<DataAccessCommand> _commands;
        IDbCommand _command;
        List<IDataReader> _readers;
        string _connectionName;
        string _connectionString;
        bool _isDisposed = false;
        bool _isInScope = false;

        #endregion

        #region Properties //////////////////////////////////////////////////////////////


        /// <summary>
        /// Gets an provider indepedent database connection
        /// </summary>
        protected internal IDbConnection Connection {
            get { return this._connection; }
        }

        /// <summary>
        /// Gets the name of the used connection
        /// </summary>
        /// <value>The name of the connection.</value>
        public string ConnectionName {
            get {
                if (string.IsNullOrEmpty(_connectionName)) {
                    return DataAccess.Provider.ConnectionName;
                }
                else {
                    return _connectionName;
                }
            }
            set { _connectionName = value; }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Returns <code>true</code> if database object contains at least
        /// one transaction started, otherwise returns <code>false</code>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has transaction; otherwise, <c>false</c>.
        /// </value>
        public bool HasTransaction {
            get { return this._transaction != null; }
        }

        /// <summary>
        /// Returns the list of all <code>IDataReader</code>s created by
        /// database object if any
        /// </summary>
        /// <value>The readers.</value>
        protected IList<IDataReader> Readers {
            get {
                if (_readers == null) {
                    _readers = new List<IDataReader>();
                }
                return _readers;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IDataParameter this[int index] {
            get { return (IDataParameter)_command.Parameters[index]; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommand Command {
            get { return _command; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters {
            get { return _command.Parameters; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateRowSource UpdatedRowSource {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }
        #endregion

        #region Construct / Destruct ////////////////////////////////////////////////////

        /// <summary>
        /// Creates a new <code>Database</code> object.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        public DataAccess(string commandText) {

            if (DataScope.Current != null) {
                this._isInScope = true;
                this._connection = DataScope.Current.Connection;
                this._transaction = DataScope.Current.Transaction;
            }
            else {
                this._connection = DataAccess.Provider.CreateConnection();
            }
            //this._transactionStack = new Stack<IDbTransaction>();
            this._command = this.CreateCommand(commandText);
            //    new List<DataAccessCommand>(Default.CommandsSize);
            //if (commands != null) {
            //    foreach (string commandText in commands) {

            //    }
            //}
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="transaction"></param>
        ///// <param name="commands"></param>
        //public Database(IDbTransaction transaction, params string[] commands) 
        //    : this(commands) {

        //    this._transaction = transaction;
        //    //this._transactionStack.Push(transaction);
        //}

        /// <summary>
        /// Creates a new <code>Database</code> object.
        /// </summary>
        public DataAccess()
            : this(null) {
        }

        #region Disposing

        /// <summary>
        /// Releases all resources used by <code>Database</code>
        /// </summary>
        public void Dispose() {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {

            if (!_isDisposed) {
                if (disposing) {
                    this.CloseReaders();
                    this.CloseCommands();
                    if (this.HasTransaction) this.Commit();
                    this.CloseConnection(true);
                }
            }
            _isDisposed = true;
        }
        #endregion
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public IDbDataParameter AddParameter(
            string name, int dbType, ParameterDirection direction, int size, object value) {

            IDbDataParameter __parameter =
                DataAccess.CreateParameter(name, dbType, direction, size, value);
            this.Parameters.Add(__parameter);
            return __parameter;
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public IDbDataParameter AddParameter(
            string name, int dbType, ParameterDirection direction, object value) {

            IDbDataParameter __parameter = DataAccess.CreateParameter(name, dbType, direction, value);
            this.Parameters.Add(__parameter);
            return __parameter;
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public IDbDataParameter AddParameter(string name, object value) {

            IDbDataParameter __parameter = DataAccess.CreateParameter(name, value);
            this.Parameters.Add(__parameter);
            return __parameter;
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public IDbDataParameter AddParameter(IDbDataParameter parameter) {

            this.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Starts a new database transaction. The current implementation allows a nested
        /// transactions to be used. If there are nested transaction then the
        /// <code>RollbackBehaviour</code> is used to manage rollback behaviour
        /// </summary>
        /// <param name="level">the transaction isolation level</param>
        /// <returns>returns the created transaction</returns>
        public IDbTransaction BeginTransaction(IsolationLevel level) {

            if (_isInScope) {
                throw new DataAccessException(DataAccessError.Transaction_CanNotBeInScope);
            }
            this.Prepare();
            _transaction = _connection.BeginTransaction(level);
            return _transaction;
        }

        /// <summary>
        /// Starts a new database transaction. The current implementation allows a nested
        /// transactions to be used. If there are nested transaction then the 
        /// <code>RollbackBehaviour</code> is used to manage rollback behaviour
        /// </summary>
        /// <returns>returns the created transaction</returns>
        public IDbTransaction BeginTransaction() {

            if (_isInScope) {
                throw new DataAccessException(DataAccessError.Transaction_CanNotBeInScope);
            }
            this.Prepare();
            this._transaction = _connection.BeginTransaction(IsolationLevel.RepeatableRead);
            return this._transaction;
        }

        /// <summary>
        /// Creates a database command  using the default database provider
        /// </summary>
        /// <param name="commandText">the command text</param>
        /// <param name="commandType">the command type</param>
        /// <param name="commandParams">the list of command parameters</param>
        /// <returns>returns the created command</returns>
        public IDbCommand CreateCommand(
            string commandText, CommandType commandType, params IDbDataParameter[] commandParams) {

            IDbTransaction __transaction = null;
            if (this.HasTransaction) {
                __transaction = this._transaction;
            }
            IDbCommand __command = DataAccess.Provider.CreateCommand(
                commandText, commandType, _connection, __transaction);
            foreach (DbParameter __parameter in commandParams) {
                __command.Parameters.Add(__parameter);
            }
            return __command;
        }

        /// <summary>
        /// Creates a database command  using the default database provider
        /// </summary>
        /// <param name="commandText">the command text</param>
        /// <param name="commandParams">the list of command parameters</param>
        /// <returns>returns the created command</returns>
        public IDbCommand CreateCommand(string commandText, params IDbDataParameter[] commandParams) {

            IDbTransaction __transaction = null;
            if (this.HasTransaction) {
                __transaction = this._transaction;// _transactionStack.Peek();
            }
            IDbCommand __command = DataAccess.Provider.CreateCommand(
                commandText, DataAccess.Provider.DefaultCommandType, _connection, __transaction);
            foreach (IDbDataParameter __parameter in commandParams) {
                __command.Parameters.Add(__parameter);
            }
            return __command;
        }

        /// <summary>
        /// Creates a database command  using the default database provider
        /// </summary>
        /// <param name="commandText">the command text</param>
        /// <param name="commandType">the command type</param>
        /// <returns>returns the created command</returns>
        public IDbCommand CreateCommand(string commandText, CommandType commandType) {

            IDbTransaction __transaction = null;
            if (this.HasTransaction) {
                __transaction = this._transaction;// _transactionStack.Peek();
            }
            IDbCommand __command = DataAccess.Provider.CreateCommand(
                commandText, commandType, _connection, __transaction);
            return __command;
        }

        /// <summary>
        /// Creates a database command  using the default database provider
        /// </summary>
        /// <param name="commandText">the command text</param>
        /// <returns>returns the created command</returns>
        public IDbCommand CreateCommand(string commandText) {

            IDbTransaction __transaction = null;
            if (this.HasTransaction) {
                __transaction = this._transaction;// _transactionStack.Peek();
            }
            IDbCommand __command = DataAccess.Provider.CreateCommand(
                commandText, DataAccess.Provider.DefaultCommandType, _connection, __transaction);
            return __command;
        }

        /// <summary>
        /// Prepares the database object for use.
        /// </summary>
        protected void Prepare() {

            if (_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken) {
                if (string.IsNullOrEmpty(this.ConnectionString)) {
                    _connection.ConnectionString =
                        ConnectionStringSettings[this.ConnectionName].ConnectionString;
                }
                else {
                    _connection.ConnectionString = _connectionString;
                }
                _connection.Open();
            }
            else {
                // close the forgoten readers, if any, before next command execution 
                this.CloseReaders();
            }
        }

        ///// <summary>
        ///// Prepares the command at index for use.
        ///// </summary>
        ///// <param name="index"></param>
        //protected void Prepare(int index) {

        //    //IDbCommand __dbCommand = this[index].Command;
        //    //if (this.HasTransaction && __dbCommand.Transaction == null) {
        //    //    __dbCommand.Transaction = this._transaction;// _transactionStack.Peek();
        //    //}
        //    // TODO: resolve this problem
        //    //INullableValue __nullable;
        //    //foreach (IDbDataParameter __param in __dbCommand.Parameters) {
        //    //    if (__param.Value is INullableValue) {
        //    //        __nullable = __param.Value as INullableValue;
        //    //        __param.Value = (__nullable.HasValue) ? __nullable.Value : DBNull.Value;
        //    //    }
        //    //}
        //}

        /// <summary>
        /// Commits transaction, if this object is transactional.
        /// It will be invoked at disposing of the object,
        /// that's why the explicit call of this method can be ommited
        /// if one transaction is to be commited.
        /// </summary>
        public void Commit() {

            if (this.HasTransaction && !_isInScope) {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back transaction, if this object is transactional. 
        /// It will be invoked by default, if any exception occur,
        /// that's why the explicit call of this method can be ommited.
        /// </summary>
        public void Rollback() {

            if (this.HasTransaction && !_isInScope) {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        #region Utitlity Methods

        /// <summary>
        /// Closes the used connection.
        /// </summary>
        /// <param name="disposing"></param>
        private void CloseConnection(bool disposing) {

            if ((!disposing && this.HasTransaction) || _isInScope) return;
            if (_connection != null && _connection.State != ConnectionState.Closed) {
                try {
                    _connection.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Closes all created commands
        /// </summary>
        private void CloseCommands() {

            if (_command != null) {
                _command.Dispose();
            }
        }

        /// <summary>
        /// Closes all readers opened, if any.
        /// </summary>
        private void CloseReaders() {

            if (_readers != null) {
                foreach (IDataReader __reader in _readers) {
                    if (__reader != null) {
                        if (!__reader.IsClosed) {
                            __reader.Close();
                        }
                        __reader.Dispose();
                    }
                }
                _readers.Clear();
            }
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void HandleException(Exception ex) {

            if (!_isInScope) {
                this.Rollback();
                throw ex;
            }
            else {
                DataScope.Current.InnerException = ex;
            }
        }

        /// <summary>
        /// Traces the start.
        /// </summary>
        /// <param name="method">The method.</param>
        private void TraceStart(string method) {

            if (TraceEnabled && Trace != null) {
                Trace.Write("data.access", string.Format("{0}_Start: {1}", method, this.Command.CommandText));
            }
        }

        /// <summary>
        /// Traces the end.
        /// </summary>
        /// <param name="method">The method.</param>
        private void TraceEnd(string method) {

            if (TraceEnabled && Trace != null) {
                Trace.Write("data.access", string.Format("{0}_End: {1}", method, this.Command.CommandText));
            }
        }
        #endregion
        #endregion
    }
}
