using UnityEngine;

namespace ObjectExporter.Exporting.Formatting
{
    /// <summary>
    /// Helper functions to convert types to .obj formatted strings.
    /// </summary>
    public class ObjString
    {
        #region Public Functions
        /// <summary>
        /// Converts a Single(float) to an obj formatted string.
        /// </summary>
        public static string Stringify(float value)
        {
            return value.ToString("0.00000");
        }

        /// <summary>
        /// Converts a Vector2 to an obj formatted string.
        /// </summary>
        public static string Stringify(Vector2 value)
        {
            return $"{value.x:0.00000} {value.y:0.00000}";
        }

        /// <summary>
        /// Converts a Vector3 to an obj formatted string.
        /// </summary>
        public static string Stringify(Vector3 value)
        {
            /* The 'x' value is inverted because .obj uses right-hand coordinates,
             * Unity uses left-hand coordinates so the x-axis is the wrong way.*/
            return $"{-value.x:0.00000} {value.y:0.00000} {value.z:0.00000}";
        }

        /// <summary>
        /// Converts a Color to an obj formatted string.
        /// </summary>
        public static string Stringify(Color value)
        {
            return $"{value.r:0.000} {value.g:0.000} {value.b:0.000}";
        }
        #endregion
    }
}
