using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Artem.Data.Access {
    public partial class DataAccess {

        /// <summary>
        /// 
        /// </summary>
        public static class Object {

            #region Static Methods //////////////////////////////////////////////////////

            /// <summary>
            /// Converts the specified data row.
            /// </summary>
            /// <param name="dataRow">The data row.</param>
            /// <returns></returns>
            public static T Convert<T>(DataRow dataRow) {

                Type type = typeof(T);
                System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null) {
                    T dataObject = (T)constructor.Invoke(null);
                    foreach (string fieldName in ObjectHelper.GetAllFields(type)) {
                        ObjectHelper.SetFieldValue(dataObject, fieldName, dataRow[fieldName]);
                    }
                    return dataObject;
                }
                else {
                    throw new Exception(string.Format(
                        @"Type '{0}' does not have a default(parameterless) public constructor! ", type));
                }
            }

            /// <summary>
            /// Fetches the array.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns></returns>
            internal static T[] FetchArray<T>(IDataReader reader) {

                DataAccessView<T> list = FetchCollection<T>(reader);
                T[] array = new T[list.Count];
                list.CopyTo(array, 0);
                return array;
            }

            /// <summary>
            /// Fetches the collection.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns></returns>
            internal static DataAccessView<T> FetchCollection<T>(IDataReader reader) {

                DataAccessView<T> list = new DataAccessView<T>();
                Type type = typeof(T);
                T current = default(T);
                System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null) {
                    // get all db fields
                    List<string> allFields = new List<string>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++) {
                        allFields.Add(reader.GetName(i).ToLower());
                    }
                    // get all object db fields and intersect
                    string __fieldName;
                    List<string> fields = new List<string>(reader.FieldCount);
                    foreach (string fieldName in ObjectHelper.GetAllFields(type)) {
                        __fieldName = fieldName.ToLower();
                        if (allFields.Contains(__fieldName)) fields.Add(__fieldName);
                    }
                    // now fetch reader
                    while (reader.Read()) {
                        current = (T)constructor.Invoke(null);
                        foreach (string fieldName in fields) {
                            ObjectHelper.SetFieldValue(current, fieldName, reader[fieldName]);
                        }
                        list.Add(current);
                    }
                    return list;//.ToArray(type);
                }
                else {
                    throw new Exception(string.Format(
                        @"Type '{0}' does not have a default(parameterless) public constructor! ", type));
                }
            }

            /// <summary>
            /// Fetches the object.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <returns></returns>
            internal static T FetchObject<T>(IDataReader reader) {

                Type type = typeof(T);
                System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null) {
                    // get all db fields
                    List<string> allFields = new List<string>(reader.FieldCount);
                    for (int i = 0; i < reader.FieldCount; i++) {
                        allFields.Add(reader.GetName(i).ToLower());
                    }
                    // get all object db fields and intersect
                    string __fieldName;
                    List<string> fields = new List<string>(reader.FieldCount);
                    foreach (string fieldName in ObjectHelper.GetAllFields(type)) {
                        __fieldName = fieldName.ToLower();
                        if (allFields.Contains(__fieldName)) fields.Add(__fieldName);
                    }
                    // now fetch reader
                    T current = default(T);
                    if (reader.Read()) {
                        current = (T)constructor.Invoke(null);
                        foreach (string fieldName in fields) {
                            ObjectHelper.SetFieldValue(current, fieldName, reader[fieldName]);
                        }
                    }
                    return current;
                }
                else {
                    throw new Exception(string.Format(
                        @"Type '{0}' does not have a default(parameteless) public constructor! ", type));
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="reader"></param>
            internal static void FetchObject(object obj, IDataReader reader) {

                Type type = obj.GetType();
                // get all db fields
                List<string> allFields = new List<string>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; i++) {
                    allFields.Add(reader.GetName(i).ToLower());
                }
                // get all object db fields and intersect
                string __fieldName;
                List<string> fields = new List<string>(reader.FieldCount);
                foreach (string fieldName in ObjectHelper.GetAllFields(type)) {
                    __fieldName = fieldName.ToLower();
                    if (allFields.Contains(__fieldName)) fields.Add(__fieldName);
                }
                // now fetch reader
                if (reader.Read()) {
                    foreach (string fieldName in fields) {
                        ObjectHelper.SetFieldValue(obj, fieldName, reader[fieldName]);
                    }
                }
            }

            /// <summary>
            /// Gets the specified command.
            /// </summary>
            /// <param name="command">The command.</param>
            /// <returns></returns>
            public static T Get<T>(string command) {

                Type type = typeof(T);
                DbCommandAttribute attribute =
                    ObjectHelper.FindCommand(type, command);
                using (DataAccess db = new DataAccess(attribute.CommandText)) {
                    db.CommandType = attribute.CommandType;
                    using (IDataReader reader = db.ExecuteReader()) {
                        return FetchObject<T>(reader);
                    }
                }
            }

            /// <summary>
            /// Gets this instance.
            /// </summary>
            /// <returns></returns>
            public static T Get<T>() {

                return Get<T>("get");
            }

            /// <summary>
            /// Inserts the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <param name="command">The command.</param>
            /// <returns></returns>
            public static T Insert<T>(T dataObject, string command) {

                Type type = dataObject.GetType();
                DbCommandAttribute attribute = ObjectHelper.FindCommand(type, command);
                using (DataAccess db = new DataAccess(attribute.CommandText)) {
                    db.CommandType = attribute.CommandType;
                    foreach (string parameterName in attribute.Parameters) {
                        db.AddParameter(parameterName,
                            ObjectHelper.GetParameterValue(dataObject, parameterName));
                    }
                    dataObject = db.FetchObject<T>();
                }
                return dataObject;
            }

            /// <summary>
            /// Inserts the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <returns></returns>
            public static T Insert<T>(T dataObject) {

                return Insert<T>(dataObject, "insert");
            }

            /// <summary>
            /// Selects the specified command.
            /// </summary>
            /// <param name="command">The command.</param>
            /// <returns></returns>
            public static DataAccessView<T> Select<T>(string command) {

                Type type = typeof(T);
                DbCommandAttribute attribute = ObjectHelper.FindCommand(type, command);
                using (DataAccess db = new DataAccess(attribute.CommandText)) {
                    db.CommandType = attribute.CommandType;
                    using (IDataReader reader = db.ExecuteReader()) {
                        return FetchCollection<T>(reader);
                    }
                }
            }

            /// <summary>
            /// Selects this instance.
            /// </summary>
            /// <returns></returns>
            public static DataAccessView<T> Select<T>() {

                return Select<T>("select");
            }

            /// <summary>
            /// Updates the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <param name="command">The command.</param>
            /// <returns></returns>
            public static T Update<T>(T dataObject, string command) {

                Type type = dataObject.GetType();
                DbCommandAttribute attribute =
                    ObjectHelper.FindCommand(type, command);
                using (DataAccess db = new DataAccess(attribute.CommandText)) {
                    db.CommandType = attribute.CommandType;
                    foreach (string parameterName in attribute.Parameters) {
                        db.AddParameter(parameterName,
                            ObjectHelper.GetParameterValue(dataObject, parameterName));
                    }
                    dataObject = db.FetchObject<T>();
                }
                return dataObject;
            }

            /// <summary>
            /// Updates the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <returns></returns>
            public static T Update<T>(T dataObject) {

                return Update<T>(dataObject, "update");
            }

            /// <summary>
            /// Deletes the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <param name="command">The command.</param>
            /// <returns></returns>
            public static int Delete<T>(T dataObject, string command) {

                Type type = dataObject.GetType();
                DbCommandAttribute attribute =
                    ObjectHelper.FindCommand(type, command);

                using (DataAccess db = new DataAccess(attribute.CommandText)) {
                    db.CommandType = attribute.CommandType;
                    foreach (string parameterName in attribute.Parameters) {
                        db.AddParameter(parameterName,
                            ObjectHelper.GetParameterValue(dataObject, parameterName));
                    }
                    return db.ExecuteNonQuery();
                }
            }

            /// <summary>
            /// Deletes the specified data object.
            /// </summary>
            /// <param name="dataObject">The data object.</param>
            /// <returns></returns>
            public static int Delete<T>(T dataObject) {

                return Delete<T>(dataObject, "delete");
            }
            #endregion
        }
    }
}