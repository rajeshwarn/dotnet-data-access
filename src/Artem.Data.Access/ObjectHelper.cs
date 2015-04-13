using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    internal static class ObjectHelper {

        #region Static Fields ///////////////////////////////////////////////////////////

        //private static readonly Type _TypeOf_DatabaseCommandAttribute = typeof(DatabaseCommandAttribute);
        private static readonly Type _TypeOf_Field = typeof(DbFieldAttribute);

        #endregion

        #region Static Methods ///////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DbCommandAttribute FindCommand(Type type, string command) {

            DbCommandAttribute commandAttribute = null;
            foreach (Attribute attribute in TypeDescriptor.GetAttributes(type)) {
                commandAttribute = attribute as DbCommandAttribute;
                if (commandAttribute != null && commandAttribute.CommandName == command) 
                    return commandAttribute;
            }
            throw CommandNotExists(command, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyDescriptor FindPrimaryProperty(Type type) {

            if (IsDataObjectSource(type)) {
                DataObjectFieldAttribute attribute = null;
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
                    attribute = (DataObjectFieldAttribute)
                        property.Attributes[typeof(DataObjectFieldAttribute)];                    
                    if (attribute != null && attribute.PrimaryKey) return property;
                }
                throw PrimaryNotExists(type);
            }
            throw IsNotDataObject(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static PropertyDescriptor FindPropertyByField(Type type, string fieldName) {

            DbFieldAttribute attribute = null;
            fieldName = fieldName.ToLower();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
                attribute = (DbFieldAttribute)property.Attributes[_TypeOf_Field];
                if (attribute != null && attribute.FieldName.ToLower() == fieldName) return property;
            }
            throw FieldNotExists(fieldName, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static PropertyDescriptor FindPropertyByParameter(Type type, string parameterName) {

            DbFieldAttribute attribute = null;
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
                attribute = (DbFieldAttribute)property.Attributes[_TypeOf_Field];
                if(attribute != null && attribute.ParameterName == parameterName) return property;
            }
            throw ParameterNotExists(parameterName, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<string> GetAllFields(Type type) {

            DbFieldAttribute attribute = null;
            List<string> list = new List<string>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
                attribute = property.Attributes[_TypeOf_Field] as DbFieldAttribute;
                if (attribute != null) list.Add(attribute.FieldName);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<string> GetAllParameters(Type type) {

            DbFieldAttribute attribute = null;
            List<string> list = new List<string>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type)) {
                attribute = property.Attributes[_TypeOf_Field] as DbFieldAttribute;
                if (attribute != null) list.Add(attribute.ParameterName);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetFieldName(Type type, string propertyName) {

            PropertyDescriptor property = TypeDescriptor.GetProperties(type)[propertyName];
            if (property != null) {
                return GetFieldName(property);
            }
            throw PropertyNotExists(propertyName, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetFieldName(PropertyDescriptor property) {

            DbFieldAttribute attribute =
                property.Attributes[_TypeOf_Field] as DbFieldAttribute;
            if (attribute != null) {
                return attribute.FieldName;
            }
            throw FieldNotExists(string.Format("for property: {0}", property.Name), property.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Type GetFieldType(Type type, string fieldName) {
            
            PropertyDescriptor property = 
                ObjectHelper.FindPropertyByField(type, fieldName);
            if(property != null) {
                return property.PropertyType;
            }
            throw FieldNotExists(fieldName, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetFieldValue(object dataObject, string fieldName) {

            PropertyDescriptor property =
                ObjectHelper.FindPropertyByField(dataObject.GetType(), fieldName);
            if (property != null) {
                return property.GetValue(dataObject);
            }
            throw FieldNotExists(fieldName, dataObject.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetFieldValue(object dataObject, string fieldName, object value) {

            PropertyDescriptor property =
                ObjectHelper.FindPropertyByField(dataObject.GetType(), fieldName);
            if (property != null) {
                property.SetValue(dataObject, DbDataConvert.ToAny(value, property.PropertyType));
            }
            else {
                throw FieldNotExists(fieldName, dataObject.GetType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="value"></param>
        public static void SetPrimaryFieldValue(object dataObject, object value) {

            PropertyDescriptor property =
                ObjectHelper.FindPrimaryProperty(dataObject.GetType());
            if (property != null) {
                property.SetValue(dataObject, DbDataConvert.ToAny(value, property.PropertyType));
            }
            else {
                throw PrimaryNotExists(dataObject.GetType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetParameterName(Type type, string propertyName) {

            PropertyDescriptor property = TypeDescriptor.GetProperties(type)[propertyName];
            if (property != null) {
                return GetParameterName(property);
            }
            throw PropertyNotExists(propertyName, type);               
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetParameterName(PropertyDescriptor property) {

            DbFieldAttribute attribute =
                property.Attributes[_TypeOf_Field] as DbFieldAttribute;
            if (attribute != null) {
                return attribute.ParameterName;
            }
            throw ParameterNotExists(string.Format("for property: {0}", property.Name), property.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Type GetParameterType(Type type, string parameterName) {

            PropertyDescriptor property = 
                ObjectHelper.FindPropertyByParameter(type, parameterName);
            if(property != null) {
                return property.PropertyType;
            }
            throw ParameterNotExists(parameterName, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static object GetParameterValue(object dataObject, string parameterName) {

            PropertyDescriptor property = 
                ObjectHelper.FindPropertyByParameter(dataObject.GetType(), parameterName);
            if (property != null) {
                return property.GetValue(dataObject);
            }
            throw ParameterNotExists(parameterName, dataObject.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void SetParameterValue(object dataObject, string parameterName, object value) {

            PropertyDescriptor property =
                ObjectHelper.FindPropertyByParameter(dataObject.GetType(), parameterName);
            if (property != null) {
                property.SetValue(dataObject, DbDataConvert.ToAny(value, property.PropertyType));
            }
            else {
                throw ParameterNotExists(parameterName, dataObject.GetType());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDataObjectSource(Type type) {

            return TypeDescriptor.GetAttributes(type)[typeof(DataObjectAttribute)] != null;
        }

        #region Exception helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Exception IsNotDataObject(Type type) {

            return new Exception(string.Format(
                @"Type {0} is not a DataObject type! Ensure {0} is marked with 
                System.ComponentModel.DataObjectAttribute and has a primary property 
                marked with System.ComponentModel.DataObjectFieldAttribute", type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Exception PrimaryNotExists(Type type) {

            return new Exception(string.Format(
                @"Type {0} does not have a primary field! Ensure {0} is marked with 
                System.ComponentModel.DataObjectAttribute and has a primary property 
                marked with System.ComponentModel.DataObjectFieldAttribute", type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private static Exception CommandNotExists(string name, Type type) {

            return new Exception(
                string.Format(@"Command '{0}' does not exists in Type '{1}'! 
                Ensure you are using a correct command name or you have marked 
                such a command with DatabaseCommandAttribute.", name, type));
        }

        /// <summary>
        /// 
        /// </summary>
        private static Exception PropertyNotExists(string name, Type type) {

            return new Exception(
                string.Format(@"Property '{0}' does not exists in Type '{1}'! 
                Ensure you are using a correct property name.", name, type));
        }

        /// <summary>
        /// 
        /// </summary>
        private static Exception FieldNotExists(string name, Type type) {

            return new Exception(
                string.Format(@"Field '{0}' does not exists in Type '{1}'! 
                Ensure you are using a correct field name or you have marked 
                such a field with DatabaseFieldAttribute.", name, type));
        }

        /// <summary>
        /// 
        /// </summary>
        private static Exception ParameterNotExists(string name, Type type) {

            return new Exception(
                string.Format(@"Parameter '{0}' does not exists in Type '{1}'! 
                Ensure you are using a correct parameter name or you have marked 
                such a parameter with DatabaseFieldAttribute.", name, type));
        }
        #endregion
        #endregion
    }
}
