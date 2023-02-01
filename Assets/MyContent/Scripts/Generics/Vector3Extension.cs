using UnityEngine;

namespace Vector3Extension {
    public static class Vector3Extensions {
        
        /// <summary>
        /// Multiply with float
        /// Get a new value with multiplication for each axes of a vector
        /// </summary>
        /// <param name="vector">The initial value multiplicand in axes</param>
        /// <param name="value">Multiplier</param>
        /// <returns>A new vector with product for each axes</returns>
        public static Vector3 MultiplyWithFloat(this Vector3 vector, float value) {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }
        
        /// <summary>
        /// Divide with int
        /// Get a new value with division for each axes of a vector
        /// </summary>
        /// <param name="vector">The initial value dividend in axes</param>
        /// <param name="value">Divisor</param>
        /// <returns>A new vector with quotient for each axes</returns>
        public static Vector3 DivideWithInt(this Vector3 vector, int value) {
            return new Vector3(vector.x / value, vector.y / value, vector.z / value);
        }


        /// <summary>
        /// Delta vector
        /// Get distance between two vectors
        /// </summary>
        /// <param name="vector">End position</param>
        /// <param name="value">Initial position</param>
        /// <returns>A new vector delta position</returns>
        public static Vector3 DeltaVector(this Vector3 vector, Vector3 value) {
            return new Vector3(vector.x - value.x, vector.y - value.y, vector.z - value.y);
        }

        /// <summary>
        /// Any axe is max or equal
        /// </summary>
        /// <param name="vector">To evaluate condition</param>
        /// <param name="value">Reference for condition</param>
        /// <returns></returns>
        public static bool AnyAxeIsMaxOrEqual(this Vector3 vector, Vector3  value) {
            return vector.x >= value.x || vector.y >= value.y || vector.z >= value.z;
        }
    }
}