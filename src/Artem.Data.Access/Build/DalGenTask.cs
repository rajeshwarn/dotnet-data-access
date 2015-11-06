using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Configuration;
using System.Reflection;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public class DalGenTask : Task {

        #region Fields  /////////////////////////////////////////////////////////////////

        string _language = "CS";
        string _fileName;
        string _outDir;

        #endregion

        #region Properties  /////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [Required]
        public string FileName {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        /// <value>The output directory.</value>
        [Output]
        public string OutputDirectory {
            get { return _outDir; }
            set { _outDir = value; }
        }
        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalGenTask"/> class.
        /// </summary>
        public DalGenTask() {
        } 
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute() {

            if (_outDir == null) {
                _outDir = Path.GetDirectoryName(_fileName);
            }
            if (!_outDir.EndsWith("\\")) {
                _outDir += "\\";
            }
            Log.LogMessage("Generating DAL for " + Language + " file:" + FileName);
            string inputFileContent;
            ///
            /// Read the file contents
            /// 
            if (File.Exists(this.FileName)) {
                using (StreamReader reader = new StreamReader(FileName)) {
                    inputFileContent = reader.ReadToEnd();
                }
                ///
                /// Build a CodeDOM tree of the file to return
                /// 
                CodeCompileUnit unit;// = CodeHelper.BuildCodeTreeFromMapFile(inputFileContent);
                using (DalGenerator dalGen = new DalGenerator(inputFileContent)) {
                    unit = dalGen.Generate();
                }
                // Code provider
                CodeDomProvider provider = CodeDomProvider.CreateProvider(_language);
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = true;
                options.BracingStyle = "Block";
                options.ElseOnClosing = true;
                options.IndentString = "    ";
                options.VerbatimOrder = false;
                //return true;
                string path;
                CodeCompileUnit code;
                CodeNamespace ns;
                foreach (CodeTypeDeclaration type in unit.Namespaces[0].Types) {
                    path = string.Format("{0}{1}.{2}", _outDir, type.Name, provider.FileExtension);
                    code = new CodeCompileUnit();
                    ns = new CodeNamespace(unit.Namespaces[0].Name);
                    foreach (CodeNamespaceImport import in unit.Namespaces[0].Imports) {
                        ns.Imports.Add(import);
                    }
                    code.Namespaces.Add(ns);
                    ns.Types.Add(type);
                    using (StreamWriter writer = new StreamWriter(path)) {
                        provider.GenerateCodeFromCompileUnit(code, writer, options);
                    }
                }
            }
            // Success
            return true;
        } 
        #endregion
    }
}
