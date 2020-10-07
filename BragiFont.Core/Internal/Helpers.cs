using System;

namespace BragiFont.Internal
{
    /// <summary>
    /// Various helpers for use with the Font System.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Converts a float to an int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The int representing the float value.</returns>
        public static int ConvertFloatToInt(float value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts the 1D array to a 2D array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="baseArray">The base 1D array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The 2D array.</returns>
        public static T[,] Convert1DArrayTo2DArray<T>(T[] baseArray, int width, int height)
        {
            var array = new T[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                array[x, y] = baseArray[x + y * width];
            }

            return array;
        }

        /// <summary>
        /// Converts the 2D array to a 1D array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="baseArray">The base array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The 1D array.</returns>
        public static T[] Convert2DArrayTo1DArray<T>(T[,] baseArray, int width, int height)
        {
            var array = new T[width * height];

            var i = 0;
            var xMax = baseArray.GetUpperBound(0);
            var yMax = baseArray.GetUpperBound(1);
            for (var x = 0; x < xMax; x++)
            for (var y = 0; y < yMax; y++)
            {
                array[i++] = baseArray[x, y];
            }

            return array;
        }
    }
}
