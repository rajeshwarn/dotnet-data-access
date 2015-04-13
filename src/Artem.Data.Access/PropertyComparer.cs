using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Artem.Data.Access {

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyComparer<T> : System.Collections.Generic.IComparer<T> {

        #region Fields  /////////////////////////////////////////////////////////////////

        private PropertyDescriptor _property;
        private ListSortDirection _direction; 

        #endregion

        #region Construct  //////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction) {

            _property = property;
            _direction = direction;
        } 

        #endregion

        #region Methods /////////////////////////////////////////////////////////////////

        #region IComparer<T>

        /// <summary>
        /// Compares the specified x word.
        /// </summary>
        /// <param name="xWord">The x word.</param>
        /// <param name="yWord">The y word.</param>
        /// <returns></returns>
        public int Compare(T xWord, T yWord) {
            // Get property values
            object xValue = GetPropertyValue(xWord, _property.Name);
            object yValue = GetPropertyValue(yWord, _property.Name);

            // Determine sort order
            if (_direction == ListSortDirection.Ascending) {
                return CompareAscending(xValue, yValue);
            }
            else {
                return CompareDescending(xValue, yValue);
            }
        }

        /// <summary>
        /// Equalses the specified x word.
        /// </summary>
        /// <param name="xWord">The x word.</param>
        /// <param name="yWord">The y word.</param>
        /// <returns></returns>
        public bool Equals(T xWord, T yWord) {
            return xWord.Equals(yWord);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public int GetHashCode(T obj) {
            return obj.GetHashCode();
        }

        #endregion

        // Compare two property values of any type
        /// <summary>
        /// Compares the ascending.
        /// </summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <returns></returns>
        private int CompareAscending(object xValue, object yValue) {
            int result;

            // If values implement IComparer
            if (xValue is IComparable) {
                result = ((IComparable)xValue).CompareTo(yValue);
            }
            // If values don't implement IComparer but are equivalent
            else if (xValue.Equals(yValue)) {
                result = 0;
            }
            // Values don't implement IComparer and are not equivalent, so compare as string values
            else result = xValue.ToString().CompareTo(yValue.ToString());

            // Return result
            return result;
        }

        /// <summary>
        /// Compares the descending.
        /// </summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <returns></returns>
        private int CompareDescending(object xValue, object yValue) {
            // Return result adjusted for ascending or descending sort order ie
            // multiplied by 1 for ascending or -1 for descending
            return CompareAscending(xValue, yValue) * -1;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private object GetPropertyValue(T value, string property) {
            // Get property
            PropertyInfo propertyInfo = value.GetType().GetProperty(property);

            // Return value
            return propertyInfo.GetValue(value, null);
        } 

        #endregion
    }
}
