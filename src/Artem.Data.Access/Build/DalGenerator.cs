using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public class DalGenerator : IDisposable {

        #region Static Fields ///////////////////////////////////////////////////////////

        static string _CommandText = @"
            select	[name]
            from	dbo.sysobjects 
            where	[type]='P' and charindex('{0}_{1}_', [name]) > 0 
            order by name asc"; 

        #endregion

        #region Fields  /////////////////////////////////////////////////////////////////

        DalExtractor _extractor;
        MapDescriptor _mapDescriptor;
        bool _disposed;

        #endregion

        #region Construct / Destruct ////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalGenerator"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="isVirtual">if set to <c>true</c> [is virtual].</param>
        public DalGenerator(string filePath, bool isVirtual) {

            _extractor = new DalExtractor(filePath, isVirtual);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalGenerator"/> class.
        /// </summary>
        /// <param name="dalContent">Content of the dal.</param>
        public DalGenerator(string dalContent) {

            _extractor = new DalExtractor(dalContent);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Artem.Data.Access.Build.DalGenerator"/> is reclaimed by garbage collection.
        /// </summary>
        ~DalGenerator() {
            Dispose(false);
        }

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

            if (!_disposed) {
                if (disposing) {
                    _extractor.Dispose();
                }
                _disposed = true;
            }
        }
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <returns></returns>
        public CodeCompileUnit Generate() {
            ///
            /// init
            /// 
            _mapDescriptor = _extractor.Extract();
            CodeCompileUnit code = new CodeCompileUnit();            
            ///
            /// Create the namespace
            /// 
            CodeNamespace unitNamespace = new CodeNamespace(_mapDescriptor.Namespace);
            unitNamespace.Imports.AddRange(new CodeNamespaceImport[] {
                new CodeNamespaceImport("System.Collections.Generic"),
                new CodeNamespaceImport("Artem.Data.Access")
            });
            code.Namespaces.Add(unitNamespace);
            ///
            /// Loop through the mappings and add classes
            /// 
            for (int i = 0; i < _mapDescriptor.TableDescriptors.Count; i++) {
                // Get the descriptor
                MapTableDescriptor tblDescriptor = _mapDescriptor.TableDescriptors[i];
                ///
                /// Create the class to represent the table
                ///
                CodeTypeDeclaration unitClass = new CodeTypeDeclaration(tblDescriptor.ClassName);
                unitClass.IsPartial = tblDescriptor.IsPartial;
                unitNamespace.Types.Add(unitClass);
                ///
                /// Custom Attributes
                ///
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(
                    "System.ComponentModel.DataObjectAttribute",
                    new CodeAttributeArgument(new CodePrimitiveExpression(true)));
                unitClass.CustomAttributes.Add(attr);
                attr = new CodeAttributeDeclaration("System.SerializableAttribute");
                unitClass.CustomAttributes.Add(attr);
                ///
                /// Loop through the selected table columns and add members
                /// 
                DataTable dt = new DataTable();
                using (DataAccess db = new DataAccess(tblDescriptor.CommandText)) {
                    if (_mapDescriptor.ConnectionString != null) {
                        db.ConnectionString = _mapDescriptor.ConnectionString;
                    }
                    db.CommandType = tblDescriptor.CommandType;// CommandType.Text;
                    db.FillSchema(dt);
                }
                ///
                /// Create Fields & Properties
                /// 
                GenFields(unitClass, dt);
                ///
                /// Create Methods
                /// 
                if (_mapDescriptor.GenerateMethods) {
                    GenMethods(unitClass);
                }
            }

            return code;
        }

        /// <summary>
        /// Generates the fields.
        /// </summary>
        /// <param name="unitClass">The unit class.</param>
        /// <param name="dt">The dt.</param>
        void GenFields(CodeTypeDeclaration unitClass, DataTable dt) {

            for (int j = 0; j < dt.Columns.Count; j++) {
                DataColumn column = dt.Columns[j];
                string colName = column.ColumnName;
                Type colType = column.DataType;
                bool colIsPrimatyKey = false;
                foreach (DataColumn col in column.Table.PrimaryKey) {
                    if (col.ColumnName == colName) {
                        colIsPrimatyKey = true;
                        break;
                    }
                }
                string fieldName = DalUtil.CreateMemberName(colName, MapMemberType.Field);
                //
                // Add the private field to store the data
                //
                CodeMemberField unitField = new CodeMemberField(colType, fieldName);
                unitClass.Members.Add(unitField);
                //
                // Add property declaration and get/set accessors
                //
                CodeMemberProperty unitProperty = new CodeMemberProperty();
                unitProperty.Name = DalUtil.CreateMemberName(colName, MapMemberType.Property);
                unitProperty.Type = new CodeTypeReference(colType);
                unitProperty.Attributes = MemberAttributes.Public;
                //
                // Custom Attributes
                //
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(
                    "Artem.Data.Access.DbFieldAttribute",
                    new CodeAttributeArgument(new CodePrimitiveExpression(colName)));
                unitProperty.CustomAttributes.Add(attr);
                attr = new CodeAttributeDeclaration(
                    "System.ComponentModel.DataObjectField",
                    new CodeAttributeArgument(new CodePrimitiveExpression(colIsPrimatyKey)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(column.AutoIncrement)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(column.AllowDBNull)),
                    new CodeAttributeArgument(new CodePrimitiveExpression(column.MaxLength)));
                unitProperty.CustomAttributes.Add(attr);
                //
                // Define the codeDOM reference for the property's private field 
                //
                CodeFieldReferenceExpression unitFieldRef = new CodeFieldReferenceExpression();
                unitFieldRef.TargetObject = new CodeThisReferenceExpression();
                unitFieldRef.FieldName = fieldName;
                //
                // Get
                //
                CodeMethodReturnStatement unitReturn = new CodeMethodReturnStatement(unitFieldRef);
                unitProperty.GetStatements.Add(unitReturn);
                //
                // Set
                //
                CodeAssignStatement unitAssign = new CodeAssignStatement();
                unitAssign.Left = unitFieldRef;
                unitAssign.Right = new CodePropertySetValueReferenceExpression();
                unitProperty.SetStatements.Add(unitAssign);

                unitClass.Members.Add(unitProperty);
            }
        }

        /// <summary>
        /// Gens the methods.
        /// </summary>
        /// <param name="unit">The unit.</param>
        void GenMethods(CodeTypeDeclaration unitClass) {

            string commandText = string.Format(_CommandText, _mapDescriptor.Prefix, unitClass.Name);
            DataTable dt = new DataTable();
            using (DataAccess db = new DataAccess(commandText)) {
                if (_mapDescriptor.ConnectionString != null) {
                    db.ConnectionString = _mapDescriptor.ConnectionString;
                }
                db.CommandType = CommandType.Text;
                db.Fill(dt);
            }
            foreach (DataRow dr in dt.Rows) {
                unitClass.Members.Add(
                    MethodBuilder.Build(unitClass, _mapDescriptor, Convert.ToString(dr[0])));
            }
        }
        #endregion
    }
}
