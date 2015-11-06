using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    enum MethodStatementType {
        Common,
        Get,
        Select,
        SelectCount,
        Delete,
        Insert,
        Update
    }

    /// <summary>
    /// 
    /// </summary>
    public class MethodBuilder {

        #region Static Fields ///////////////////////////////////////////////////////////

        static string _CommandText = @"
            select	s.id , s.name, t.name as [type], t.length
            from	syscolumns s inner join systypes t on s.xtype = t.xtype 
            where	id = (select id from sysobjects where name = '{0}') and t.name <> 'sysname'
            order by s.colorder";

        #endregion

        #region Static Methods //////////////////////////////////////////////////////////

        /// <summary>
        /// Builds the specified unit class.
        /// </summary>
        /// <param name="unitClass">The unit class.</param>
        /// <param name="desc">The desc.</param>
        /// <param name="sqlName">Name of the SQL.</param>
        /// <returns></returns>
        public static CodeMemberMethod Build(CodeTypeDeclaration unitClass, MapDescriptor desc, string sqlName) {

            MethodBuilder builder = new MethodBuilder(unitClass, desc, sqlName);
            return builder.Build();
        }
        #endregion

        #region Fields  /////////////////////////////////////////////////////////////////

        CodeTypeDeclaration _unitClass;
        CodeMemberMethod _unitMethod;
        MapDescriptor _mapDescriptor;
        string _sqlName;
        string _memberName;

        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MethodBuilder"/> class.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <param name="sqlName">Name of the SQL.</param>
        public MethodBuilder(CodeTypeDeclaration unitClass, MapDescriptor desc, string sqlName) {

            _unitClass = unitClass;
            _unitMethod = new CodeMemberMethod();
            _mapDescriptor = desc;
            _sqlName = sqlName;
            int index = _sqlName.IndexOf('_') + 1;
            if (!string.IsNullOrEmpty(_mapDescriptor.Prefix)) {
                index = _sqlName.IndexOf('_', index) + 1;
            }
            _memberName = _sqlName.Substring(index);
            _memberName = DalUtil.CreateMemberName(_memberName, MapMemberType.Method);
        }
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public CodeMemberMethod Build() {

            CodeAttributeDeclaration attr = null;
            _unitMethod.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            MethodStatementType statementType = MethodStatementType.Common;
            if (_memberName.StartsWith("Get")) {
                _unitMethod.ReturnType = new CodeTypeReference(_unitClass.Name);
                statementType = MethodStatementType.Get;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Select")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            else if (_memberName.StartsWith("SelCount")) {
                _memberName = "SelectCount";
                _unitMethod.ReturnType = new CodeTypeReference(typeof(int));
                statementType = MethodStatementType.SelectCount;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Select")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            else if (_memberName.StartsWith("Sel")) {
                if (!_memberName.StartsWith("Select")) {
                    _memberName = _memberName.Replace("Sel", "Select");
                }
                _unitMethod.ReturnType = new CodeTypeReference(
                    string.Format("Artem.Data.Access.DataAccessView<{0}>", _unitClass.Name));
                statementType = MethodStatementType.Select;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Select")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            else if (_memberName.StartsWith("Del")) {
                if (!_memberName.StartsWith("Delete")) {
                    _memberName = _memberName.Replace("Del", "Delete");
                }
                statementType = MethodStatementType.Delete;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Delete")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            else if (_memberName.StartsWith("Ins")) {
                if (!_memberName.StartsWith("Insert")) {
                    _memberName = _memberName.Replace("Ins", "Insert");
                }
                statementType = MethodStatementType.Insert;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Insert")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            else if (_memberName.StartsWith("Upd")) {
                if (!_memberName.StartsWith("Update")) {
                    _memberName = _memberName.Replace("Upd", "Update");
                }
                statementType = MethodStatementType.Update;
                // data attribute
                attr = new CodeAttributeDeclaration("System.ComponentModel.DataObjectMethod",
                    new CodeAttributeArgument(new CodeSnippetExpression("System.ComponentModel.DataObjectMethodType.Update")));
                _unitMethod.CustomAttributes.Add(attr);
            }
            _unitMethod.Name = _memberName;
            BuildStatement(statementType);
            return _unitMethod;
        }

        /// <summary>
        /// Builds the params.
        /// </summary>
        void BuildStatement(MethodStatementType statementType) {

            StringBuilder buff = new StringBuilder();
            CodeParameterDeclarationExpression unitParam;
            string paramName;
            string dbType;
            string indentSpace = "            ";//12 chars
            string indentSpaceSub = "                ";
            PagingFlags pagingFlags = PagingFlags.None;
            buff.Append(indentSpace);
            buff.AppendFormat("using(DataAccess db = new DataAccess(\"{0}\"))", _sqlName);
            buff.AppendLine("{");
            foreach (DataRow dr in FetchParamList().Rows) {
                paramName = DbDataConvert.ToString(dr[1]);
                dbType = DbDataConvert.ToString(dr[2]);
                unitParam = new CodeParameterDeclarationExpression(
                    DalUtil.MapDbType(dbType),
                    DalUtil.CreateMemberName(paramName, MapMemberType.Parameter));
                switch (unitParam.Name.ToLower()) {
                    case "rowindex":
                        if (statementType == MethodStatementType.Select || 
                            statementType == MethodStatementType.SelectCount) {
                            ///
                            pagingFlags |= PagingFlags.RowIndex;
                            unitParam.Type = new CodeTypeReference(typeof(int?));
                            buff.Append(indentSpaceSub)
                                .AppendFormat("if ({0}.HasValue)", unitParam.Name)
                                .AppendLine()
                                .Append(indentSpaceSub).Append("    ")
                                .AppendFormat("db.AddParameter(\"{0}\", {1}.Value);", paramName, unitParam.Name);
                        }
                        else {
                            buff.Append(indentSpaceSub)
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        _unitMethod.Parameters.Add(unitParam);
                        break;
                    case "pagesize":
                        if (statementType == MethodStatementType.Select ||
                            statementType == MethodStatementType.SelectCount) {
                            ///
                            pagingFlags |= PagingFlags.PageSize;
                            unitParam.Type = new CodeTypeReference(typeof(int?));
                            buff.Append(indentSpaceSub)
                                .AppendFormat("if ({0}.HasValue)", unitParam.Name)
                                .AppendLine()
                                .Append(indentSpaceSub).Append("    ")
                                .AppendFormat("db.AddParameter(\"{0}\", {1}.Value);", paramName, unitParam.Name);
                        }
                        else {
                            buff.Append(indentSpaceSub)
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        _unitMethod.Parameters.Add(unitParam);
                        break;
                    case "keyword":
                        if (statementType == MethodStatementType.Select ||
                            statementType == MethodStatementType.SelectCount) {
                            ///
                            buff.Append(indentSpaceSub)
                                .AppendLine("if (!string.IsNullOrEmpty(keyword))")
                                .Append(indentSpaceSub).Append("    ")
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        else {
                            buff.Append(indentSpaceSub)
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        _unitMethod.Parameters.Add(unitParam);
                        break;
                    case "criteria":
                        if (statementType == MethodStatementType.Select ||
                            statementType == MethodStatementType.SelectCount) {
                            ///
                            buff.Append(indentSpaceSub)
                                .AppendLine("if (!string.IsNullOrEmpty(criteria))")
                                .Append(indentSpaceSub).Append("    ")
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        else {
                            buff.Append(indentSpaceSub)
                                .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        }
                        _unitMethod.Parameters.Add(unitParam);
                        break;
                    case "operator":
                        buff.Append(indentSpaceSub)
                            .AppendFormat("Artem.Data.Access.Build.DalUtil.AddOperatorParam(db);");
                        break;
                    default:
                        DalUtil.AddParamInStatement(buff, dbType, paramName, unitParam.Name, indentSpaceSub);
                        //buff.Append(indentSpaceSub)
                        //    .AppendFormat("db.AddParameter(\"{0}\", {1});", paramName, unitParam.Name);
                        _unitMethod.Parameters.Add(unitParam);
                        break;
                }
                buff.AppendLine();
            }
            switch (statementType) {
                case MethodStatementType.Common:
                case MethodStatementType.Delete:
                case MethodStatementType.Insert:
                case MethodStatementType.Update:
                    buff.Append(indentSpaceSub).Append("db.ExecuteNonQuery();");
                    break;
                case MethodStatementType.Get:
                    buff.Append(indentSpaceSub).AppendFormat("return db.FetchObject<{0}>();", _unitClass.Name);
                    break;
                case MethodStatementType.Select:
                    buff.Append(indentSpaceSub).AppendFormat("return db.FetchCollection<{0}>();", _unitClass.Name);
                    break;
                case MethodStatementType.SelectCount:
                    buff.Append(indentSpaceSub).Append("return db.ExecuteScalar<int>();");
                    break;
            }
            buff.AppendLine();
            buff.Append(indentSpace).Append("}");
            _unitMethod.Statements.Add(new CodeSnippetStatement(buff.ToString()));
            // if paging enabled add count method before go back
            //if (pagingFlags == PagingFlags.Enabled) {
            //    GenSelectCount();
            //}
        }

        /// <summary>
        /// Fetches the param list.
        /// </summary>
        /// <returns></returns>
        DataTable FetchParamList() {

            string commandText = string.Format(_CommandText, _sqlName);
            DataTable dt = new DataTable();
            using (DataAccess db = new DataAccess(commandText)) {
                if (_mapDescriptor.ConnectionString != null) {
                    db.ConnectionString = _mapDescriptor.ConnectionString;
                }
                db.CommandType = CommandType.Text;
                db.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Gens the select count.
        /// </summary>
        void GenSelectCount() {

            CodeMemberMethod unitMethod = new CodeMemberMethod();
            unitMethod.Name = _memberName + "Count";
            _unitClass.Members.Add(unitMethod);
        }
        #endregion
    }
}
