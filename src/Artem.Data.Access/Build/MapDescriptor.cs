using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public class MapDescriptor {

        #region Fields  /////////////////////////////////////////////////////////////////

        string _connectionString;
        string _namespace;
        string _prefix;
        bool _genMethods = true;
        List<MapTableDescriptor> _tableDescriptors;

        #endregion

        #region Properties  /////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate methods].
        /// </summary>
        /// <value><c>true</c> if [generate methods]; otherwise, <c>false</c>.</value>
        public bool GenerateMethods {
            get { return _genMethods; }
            set { _genMethods = value; }
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace {
            get { return _namespace; }
            set { _namespace = value; }
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix {
            get { return _prefix; }
            set { _prefix = value; }
        }

        /// <summary>
        /// Gets the table descriptors.
        /// </summary>
        /// <value>The table descriptors.</value>
        public List<MapTableDescriptor> TableDescriptors {
            get { return _tableDescriptors; }
        }
        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MapDescriptor"/> class.
        /// </summary>
        public MapDescriptor() {

            _tableDescriptors = new List<MapTableDescriptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MapDescriptor"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public MapDescriptor(XmlNode node)
            : this() {

            if (node.Name != "dal")
                throw new ArgumentException(string.Format(
                    "Invalid xml node provided {0}, while expecting dal", node.Name));

            _namespace = node.Attributes["namespace"].Value;
            if (node.Attributes["prefix"] != null) {
                _prefix = node.Attributes["prefix"].Value;
            }
            if (node.Attributes["generateMethods"] != null) {
                bool.TryParse(node.Attributes["generateMethods"].Value, out _genMethods);
            }
            if (node.Attributes["connectionString"] != null) {
                _connectionString = node.Attributes["connectionString"].Value;
            }
        }
        #endregion
    }
}
