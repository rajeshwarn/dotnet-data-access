using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public class MapTableDescriptor {

        #region Fields  /////////////////////////////////////////////////////////////////

        string _className;
        string _commandText;
        CommandType _commandType = CommandType.Text;
        bool _isPartial = true;

        #endregion

        #region Properties  /////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName {
            get { return _className; }
            set { _className = value; }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText {
            get { return _commandText; }
            set { _commandText = value; }
        }

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType {
            get { return _commandType; }
            set { _commandType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is partial.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is partial; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartial {
            get { return _isPartial; }
            set { _isPartial = value; }
        }
        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MapTableDescriptor"/> class.
        /// </summary>
        public MapTableDescriptor() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MapTableDescriptor"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public MapTableDescriptor(XmlNode node) {

            _className = node.Attributes["className"].Value;
            _commandText = node.Attributes["commandText"].Value;
            if (node.Attributes["commandType"] != null) {
                switch (node.Attributes["commandType"].Value.Trim().ToLower()) {
                    case "storedprocedure":
                        _commandType = CommandType.StoredProcedure;
                        break;
                    case "tabledirect":
                        _commandType = CommandType.TableDirect;
                        break;
                    default:
                        _commandType = CommandType.Text;
                        break;
                }
            }
            if (node.Attributes["partial"] != null) {
                bool.TryParse(node.Attributes["partial"].Value, out _isPartial);
            }
        }
        #endregion
    }
}
