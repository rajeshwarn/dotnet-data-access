using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace Artem.Data.Access {
    public partial class DataAccess {

        #region Methods /////////////////////////////////////////////////////////////////

        #region Execute Methods

        /// <summary>
        /// Executes as non query command at specified index
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery() {

            TraceStart("ExecuteNonQuery");
            try {
                this.Prepare();
                return Command.ExecuteNonQuery();
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw;
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("ExecuteNonQuery");
            }
        }

        /// <summary>
        /// Executes command as IDataAdapter at specified index.
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader() {

            TraceStart("ExecuteReader");
            try {
                this.Prepare();
                IDataReader reader = Command.ExecuteReader(
                    this.HasTransaction ? CommandBehavior.Default : CommandBehavior.CloseConnection);
                this.Readers.Add(reader); // add a ref for disposing
                return reader;
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw;
            }
            finally {
                TraceEnd("ExecuteReader");
            }
        }

        /// <summary>
        /// Executes command as scalar at specified index.
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar() {

            TraceStart("ExecuteScalar");
            try {
                this.Prepare();
                return Command.ExecuteScalar();
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw;
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("ExecuteScalar");
            }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <returns></returns>
        public T ExecuteScalar<T>() {

            return DbDataConvert.To<T>(ExecuteScalar());
        }
        #endregion

        #region Fetch Methods

        /// <summary>
        /// Fetches the array.
        /// </summary>
        /// <returns></returns>
        public T[] FetchArray<T>() {

            TraceStart("FetchArray<T>");
            try {
                this.Prepare();
                using (IDataReader reader = this.ExecuteReader()) {
                    return DataAccess.Object.FetchArray<T>(reader);
                }
            }
            finally {
                TraceEnd("FetchArray<T>");
            }
        }

        /// <summary>
        /// Fetches the collection.
        /// </summary>
        /// <returns></returns>
        public DataAccessView<T> FetchCollection<T>() {

            TraceStart("FetchCollection<T>");
            try {
                this.Prepare();
                using (IDataReader reader = this.ExecuteReader()) {
                    return DataAccess.Object.FetchCollection<T>(reader);
                }
            }
            finally {
                TraceEnd("FetchCollection<T>");
            }
        }

        /// <summary>
        /// Fetches the object.
        /// </summary>
        /// <returns></returns>
        public T FetchObject<T>() {

            TraceStart("FetchObject<T>");
            try {
                this.Prepare();
                using (IDataReader reader = this.ExecuteReader()) {
                    return DataAccess.Object.FetchObject<T>(reader);
                }
            }
            finally {
                TraceEnd("FetchObject<T>");
            }
        }

        /// <summary>
        /// Fetches the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected internal void FetchObject(object obj) {

            TraceStart("FetchObject");
            try {
                this.Prepare();
                using (IDataReader reader = this.ExecuteReader()) {
                    DataAccess.Object.FetchObject(obj, reader);
                }
            }
            finally {
                TraceEnd("FetchObject");
            }
        }
        #endregion

        #region Fill Methods

        /// <summary>
        /// Fills the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        public void Fill(DataSet dataSet) {

            TraceStart("Fill");
            this.Prepare();
            try {
                IDbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter();
                adapter.SelectCommand = Command;
                adapter.Fill(dataSet);
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Fill");
            }
        }

        /// <summary>
        /// Fills the specified data table.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public void Fill(DataTable dataTable) {

            TraceStart("Fill");
            this.Prepare();
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    adapter.SelectCommand = Command as DbCommand;
                    adapter.Fill(dataTable);
                }
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Fill");
            }
        }

        /// <summary>
        /// Fills the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="sourceTable">The source table.</param>
        public void Fill(DataSet dataSet, string sourceTable) {

            TraceStart("Fill");
            this.Prepare();
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    adapter.SelectCommand = Command as DbCommand;
                    adapter.Fill(dataSet, sourceTable);
                }
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Fill");
            }
        }

        /// <summary>
        /// Fills the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="startRecord">The start record.</param>
        /// <param name="maxRecords">The max records.</param>
        /// <param name="sourceTable">The source table.</param>
        public void Fill(DataSet dataSet,
            int startRecord, int maxRecords, string sourceTable) {

            TraceStart("Fill");
            this.Prepare();
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    adapter.SelectCommand = Command as DbCommand;
                    adapter.Fill(dataSet, startRecord, maxRecords, sourceTable);
                }
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Fill");
            }
        }
        #endregion

        #region Fill Methods

        /// <summary>
        /// Fills the schema.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        public void FillSchema(DataSet dataSet) {

            TraceStart("FillSchema");
            this.Prepare();
            try {
                IDbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter();
                adapter.SelectCommand = Command;
                adapter.FillSchema(dataSet, SchemaType.Mapped);
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("FillSchema");
            }
        }

        /// <summary>
        /// Fills the schema.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public void FillSchema(DataTable dataTable) {

            TraceStart("FillSchema");
            DataSet dataSet = new DataSet();
            this.Prepare();
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    adapter.SelectCommand = Command as DbCommand;
                    adapter.FillSchema(dataTable, SchemaType.Mapped);
                }
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("FillSchema");
            }
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Updates a dataset using insert, update and delete comands, specified by indices
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        public void Update(DataSet dataSet) {

            TraceStart("Update");
            try {
                this.Prepare();
                IDbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter();
                if (dataSet.HasChanges(DataRowState.Added)) {
                    adapter.InsertCommand = Command;
                }
                if (dataSet.HasChanges(DataRowState.Modified)) {
                    adapter.UpdateCommand = Command;
                }
                if (dataSet.HasChanges(DataRowState.Deleted)) {
                    adapter.DeleteCommand = Command;
                }
                adapter.Update(dataSet);
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw; // not handled here
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Update");
            }
        }

        /// <summary>
        /// Updates a dataset using insert, update and delete comands,
        /// specified by indices
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public void Update(DataTable dataTable) {

            TraceStart("Update");
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    adapter.InsertCommand = Command as DbCommand;
                    adapter.UpdateCommand = Command as DbCommand;
                    adapter.DeleteCommand = Command as DbCommand;
                    this.Prepare();
                    adapter.Update(dataTable);
                }
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw; // not handled here
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Update");
            }
        }

        /// <summary>
        /// Updates the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="sourceTable">The source table.</param>
        public void Update(DataSet dataSet, string sourceTable) {

            TraceStart("Update");
            try {
                using (DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter) {
                    if (dataSet.HasChanges(DataRowState.Added))
                        adapter.InsertCommand = Command as DbCommand;
                    if (dataSet.HasChanges(DataRowState.Modified))
                        adapter.UpdateCommand = Command as DbCommand;
                    if (dataSet.HasChanges(DataRowState.Deleted))
                        adapter.DeleteCommand = Command as DbCommand;
                    this.Prepare();
                    adapter.Update(dataSet, sourceTable);
                }
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw; // not handled here
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Update");
            }
        }

        /// <summary>
        /// Updates the specified data rows.
        /// </summary>
        /// <param name="dataRows">The data rows.</param>
        public void Update(DataRow[] dataRows) {

            TraceStart("Update");
            try {
                DbDataAdapter adapter = DataAccess.Provider.CreateDataAdapter() as DbDataAdapter;
                adapter.InsertCommand = Command as DbCommand;
                adapter.UpdateCommand = Command as DbCommand;
                adapter.DeleteCommand = Command as DbCommand;
                this.Prepare();
                adapter.Update(dataRows);
            }
            catch (Exception ex) {
                this.HandleException(ex);
                //this.Rollback();
                throw; // not handled here
            }
            finally {
                this.CloseConnection(false);
                TraceEnd("Update");
            }
        }
        #endregion
        #endregion
    }
}
