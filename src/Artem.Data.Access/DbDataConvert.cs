using System;


namespace Artem.Data.Access {


	/// <summary>
	/// Utility class for converting with respect of DBNull.
	/// </summary>
	public static class DbDataConvert {

		#region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T To<T>(object value) {

            return (T)ToAny(value, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToAny(object value, Type type) {

            switch (type.Name) {
                case "Boolean":
                    return DbDataConvert.ToBoolean(value);
                case "Byte":
                    return DbDataConvert.ToByte(value);
                case "Byte[]":
                    return DbDataConvert.ToByteArray(value);
                case "Char":
                    return DbDataConvert.ToChar(value);
                case "DateTime":
                    return DbDataConvert.ToDateTime(value);
                case "Decimal":
                    return DbDataConvert.ToDecimal(value);
                case "Double":
                    return DbDataConvert.ToDouble(value);
                case "Single":
                    return DbDataConvert.ToFloat(value);
                case "Int32":
                    return DbDataConvert.ToInt(value);
                case "Int64":
                    return DbDataConvert.ToLong(value);
                case "Int16":
                    return DbDataConvert.ToShort(value);
                case "String":
                    return DbDataConvert.ToString(value);
                case "UInt32":
                    return DbDataConvert.ToUInt(value);
                case "UInt64":
                    return DbDataConvert.ToULong(value);
                case "UInt16":
                    return DbDataConvert.ToUShort(value);
                case "Guid":
                    return DbDataConvert.ToGuid(value);
            }
            return value;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool ToBoolean(object value) {

			return Convert.ToBoolean(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte ToByte(object value) {

			return Convert.ToByte(Convert.IsDBNull(value) ? null : value);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(object value) {

            return Convert.IsDBNull(value) ? null : (byte[])value;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static char ToChar(object value) {

			return Convert.ToChar(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(object value) {

			return Convert.ToDateTime(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static decimal ToDecimal(object value) {

			return Convert.ToDecimal(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double ToDouble(object value) {

			return Convert.ToDouble(Convert.IsDBNull(value) ? null : value);
		}

        /// <summary>
        /// Toes the GUID.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Guid ToGuid(object value) {

            if (Convert.IsDBNull(value)) {
                return Guid.Empty;
            }
            else if (value is Guid) {
                return (Guid)value;
            }
            else {
                string strValue = Convert.ToString(value);
                if (!string.IsNullOrEmpty(strValue)) {
                    return new Guid(strValue);
                }
                return Guid.Empty;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static short ToShort(object value) {

			return Convert.ToInt16(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ToInt(object value) {

			return Convert.ToInt32(Convert.IsDBNull(value) ? null : value);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLong(object value) {

            return Convert.ToInt64(Convert.IsDBNull(value) ? null : value);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float ToFloat(object value) {

			return Convert.ToSingle(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToString(object value) {

			return Convert.ToString(Convert.IsDBNull(value) ? null : value);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringOrNull(object value) {

            if (Convert.IsDBNull(value)) return null;
            return Convert.ToString(value);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ushort ToUShort(object value) {

			return Convert.ToUInt16(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static uint ToUInt(object value) {

			return Convert.ToUInt32(Convert.IsDBNull(value) ? null : value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ulong ToULong(object value) {

			return Convert.ToUInt64(Convert.IsDBNull(value) ? null : value);
		}
		#endregion
	}
}
