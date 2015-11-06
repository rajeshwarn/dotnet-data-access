using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Web.Hosting;

namespace Artem.Data.Access.Build {

    /// <summary>
    /// 
    /// </summary>
    public class DalExtractor : IDisposable {

        #region Fields  /////////////////////////////////////////////////////////////////

        XmlDocument _doc;
        string _filePath;
        bool _isVirtual;
        string _content;
        bool _disposed;

        #endregion

        #region Construct / Distruct ////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalExtractor"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="isVirtual">if set to <c>true</c> [is virtual].</param>
        public DalExtractor(string filePath, bool isVirtual)
            : this() {

            _filePath = filePath;
            _isVirtual = isVirtual;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalExtractor"/> class.
        /// </summary>
        /// <param name="dalContent">Content of the dal.</param>
        public DalExtractor(string dalContent)
            : this() {

            _content = dalContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DalExtractor"/> class.
        /// </summary>
        private DalExtractor() {
            _doc = new XmlDocument();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Artem.Data.Access.Build.DalExtractor"/> is reclaimed by garbage collection.
        /// </summary>
        ~DalExtractor() {
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
                    _filePath = null;
                    _content = null;
                    _doc = null;
                }
                _disposed = true;
            }
        }
        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        /// <summary>
        /// Extracts this instance.
        /// </summary>
        /// <returns></returns>
        public MapDescriptor Extract() {

            if (_disposed)
                throw new ObjectDisposedException("DalExtractor");

            Load();
            ///
            /// get map descriptor
            /// 
            XmlNode root = _doc.DocumentElement;
            MapDescriptor map = new MapDescriptor(root);
            foreach (XmlNode node in _doc.SelectNodes("dal/mapping")) {
                map.TableDescriptors.Add(new MapTableDescriptor(node));
            }
            // return
            return map;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        void Load() {

            if (_content == null) {
                if (_isVirtual) {
                    using (Stream inFile = VirtualPathProvider.OpenFile(_filePath)) {
                        _doc.Load(inFile);
                    }
                }
                else {
                    using (Stream inFile = File.Open(_filePath, FileMode.Open)) {
                        _doc.Load(inFile);
                    }
                }
            }
            else {
                _doc.LoadXml(_content);
            }
        }
        #endregion
    }
}
