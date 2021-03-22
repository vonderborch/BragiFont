using System;
using SharpFont;

namespace BragiFont.Internal
{
    /// <summary>
    /// An implementation of the Font object.
    /// </summary>
    /// <seealso cref="BragiFont.Font" />
    internal class FontImplementation : Font
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontImplementation"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="face">The font face.</param>
        /// <param name="fontSize">Size of the font.</param>
        public FontImplementation(Tuple<string, int> key, Face face, int fontSize) : base(key, face, fontSize) { }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
