using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;


namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public static class DalUtil {

        #region Static Properties ///////////////////////////////////////////////////////

        /// <summary>
        /// Gets the current user key.
        /// </summary>
        /// <value>The current user key.</value>
        public static object CurrentUserKey {
            get {
                object value = HttpContext.Current.Items["__CurrentUserKey"];
                if (value == null) {
                    MembershipUser user = Membership.GetUser();
                    if (user != null) {
                        value = user.ProviderUserKey;
                        HttpContext.Current.Items["__CurrentUserKey"] = value;
                    }
                }
                return value;
            }
        }
        #endregion

        #region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// Adds the operator param.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        public static void AddOperatorParam(DataAccess da) {
            da.AddParameter("@Operator", CurrentUserKey);
        }

        /// <summary>
        /// Adds the paging params.
        /// </summary>
        /// <param name="da">The da.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="pageSize">Size of the page.</param>
        public static void AddPagingParams(DataAccess da, int? rowIndex, int? pageSize) {

            if (rowIndex.HasValue)
                da.AddParameter("@RowIndex", rowIndex.Value);
            else
                da.AddParameter("@RowIndex", 0);
            if (pageSize.HasValue)
                da.AddParameter("@PageSize", pageSize.Value);
            else
                da.AddParameter("@PageSize", int.MaxValue);
        }

        /// <summary>
        /// Adds the search params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="keyword">The keyword.</param>
        public static void AddSearchParams(DataAccess da,
            int? rowIndex, int? pageSize, string criteria, string keyword) {

            if (rowIndex.HasValue)
                da.AddParameter("@RowIndex", rowIndex.Value);
            else
                da.AddParameter("@RowIndex", 0);
            if (pageSize.HasValue)
                da.AddParameter("@PageSize", pageSize.Value);
            else
                da.AddParameter("@PageSize", int.MaxValue);
            if (!string.IsNullOrEmpty(criteria))
                da.AddParameter("@Criteria", criteria);
            if (!string.IsNullOrEmpty(keyword))
                da.AddParameter("@Keyword", keyword + "%");
        }

        /// <summary>
        /// Creates the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <returns></returns>
        public static string CreateMemberName(string name, MapMemberType memberType) {

            StringBuilder memberName = new StringBuilder();
            string[] words = name.Split('_');
            switch (memberType) {
                case MapMemberType.Field:
                    memberName.Append("_").Append(LowerFirstLetter(words[0]));
                    break;
                case MapMemberType.Property:
                    memberName.Append(CapitalizeFirstLetter(words[0]));
                    break;
                case MapMemberType.Method:
                    memberName.Append(CapitalizeFirstLetter(words[0]));

                    break;
                case MapMemberType.Parameter:
                    words[0] = words[0].Substring(1);
                    memberName.Append(LowerFirstLetter(words[0]));
                    break;
            }
            for (int i = 1; i < words.Length; i++) {
                memberName.Append(CapitalizeFirstLetter(words[i]));
            }
            return memberName.ToString();
        }

        /// <summary>
        /// Capitalizes the first letter.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(string s) {

            string ret = "";
            if (char.IsLower(s[0])) {
                ret += char.ToUpper(s[0]);
                ret += s.Substring(1);
            }
            else
                ret = s;
            return ret;
        }

        /// <summary>
        /// Capitalizes the first letter.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string LowerFirstLetter(string s) {

            string ret = "";
            if (char.IsUpper(s[0])) {
                ret += char.ToLower(s[0]);
                ret += s.Substring(1);
            }
            else
                ret = s;
            return ret;
        }

        /// <summary>
        /// Maps the type of the db.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        public static Type MapDbType(string dbType) {

            switch (dbType) {
                case "bigint":
                    return typeof(long?);
                case "bit":
                    return typeof(bool?);
                case "datetime":
                case "timestamp":
                case "smalldatetime":
                    return typeof(DateTime?);
                case "decimal":
                    return typeof(decimal?);
                case "float":
                case "smallmoney":
                    return typeof(float?);
                case "image":
                    return typeof(byte[]);
                case "int":
                    return typeof(int?);
                case "money":
                case "numeric":
                case "real":
                    return typeof(double?);
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    return typeof(string);
                case "smallint":
                    return typeof(short?);
                case "tinyint":
                    return typeof(short?);
                case "uniqueidentifier":
                    return typeof(Guid?);
                default:
                    return typeof(int?);
            }
        }

        /// <summary>
        /// Adds the param in statement.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="unitParamName">Name of the unit param.</param>
        /// <param name="indent">The indent.</param>
        public static void AddParamInStatement(StringBuilder buff, 
            string dbType, string paramName, string unitParamName, string indent) {

            switch (dbType) {
                case "bigint":
                case "bit":
                case "datetime":
                case "timestamp":
                case "smalldatetime":
                case "decimal":
                case "float":
                case "smallmoney":
                case "int":
                case "money":
                case "numeric":
                case "real":
                case "smallint":
                case "tinyint":
                case "uniqueidentifier":
                    buff.Append(indent)
                        .AppendFormat("if ({0}.HasValue)", unitParamName)
                        .AppendLine()
                        .Append(indent)
                        .Append("    ")
                        .AppendFormat("db.AddParameter(\"{0}\", {1}.Value);", paramName, unitParamName);
                    break;
                case "image":
                    buff.Append(indent)
                        .AppendFormat("if ({0} != null && {0}.Length > 0)", unitParamName)
                        .AppendLine()
                        .Append(indent)
                        .Append("    ")
                        .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParamName);
                    break;
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                    buff.Append(indent)
                         .AppendFormat("if (!string.IsNullOrEmpty({0}))", unitParamName)
                         .AppendLine()
                         .Append(indent)
                         .Append("    ")
                         .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParamName);
                    break;
                default:
                    buff.Append(indent)
                        .AppendFormat("if ({0}.HasValue)", unitParamName)
                        .AppendLine()
                        .Append(indent)
                        .Append("    ")
                        .AppendFormat("db.AddParameter(\"{0}\", {1}.Value);", paramName, unitParamName);
                    break;
            }
        }
        #endregion
    }
}
