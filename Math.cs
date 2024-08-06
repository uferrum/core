using UnityEngine;

namespace Ferrum
{
    public static class Math
    {
        public static float Apply(float leftA, float rightA, float leftB)
        {
            return (leftB * rightA) / leftA;
        }

        /// <summary>
        /// Increment / Decrement the inner value.
        /// </summary>
        /// <returns> What exceeds </returns>
        public static float SetClamped(ref float val, float by, float min = float.MinValue, float max = float.MaxValue)
        {
            float newValue = val + by;
            float excess = 0f;

            if (newValue < min)
            {
                excess = min - newValue;
                val = min;
            }
            else if (newValue > max)
            {
                excess = newValue - max;
                val = max;
            }
            else
            {
                val = newValue;
            }

            return excess;
        }
        #region "Random"

        public static Vector3 Random(Bounds bounds)
        {
            return Random(bounds.min, bounds.max);
        }

        public static Vector3 Random(Vector3 min, Vector3 max)
        {
            return new (UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
        }
        
        public static Vector2 Random(Vector2 min, Vector2 max)
        {
            return new (UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
        }

        #endregion
    }
}