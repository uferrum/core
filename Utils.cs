using UnityEngine;
using System.Collections;

namespace Ferrum
{
    
    public class Utils
    {
        // Can be filled with other libraries which adds extensions to this
    }

    public static class MonoBehaviorExtensions
    {
        public static IEnumerator WaitUntilStarted(this MonoBehaviour behavior)
        {
            while (!behavior.didStart) yield return null;
            yield return null;
        }
    }

    public static class BoundsExtensions
    {
        /// <summary>
        /// Returns a random Vector3 position inside the bounds
        /// </summary>
        public static Vector3 Random(this Bounds bounds)
        {
            return Math.Random(bounds.min, bounds.max);
        }
    }
}