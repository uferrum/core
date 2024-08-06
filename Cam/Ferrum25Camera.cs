using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 Ferrum Camera
 */

namespace Ferrum.Cam
{
    public enum InterestType
    {
        Idle, // Foucses when there are no objects needs interest
        Entity, // Like the player and important entity like a boss
        Zone, // Needed for ZOIs ( Zones of interest )
        Scene, // When the game wants to show something special to the player ( like when a door is opened )
    }

    public class CameraInterest
    {
        public InterestType type = InterestType.Idle;

        // One of the two will be used, so fixedTarget will be ignored when target is present;
        public Vector3 fixedTarget = Vector3.zero;
        public GameObject target = null;

        public Vector3 offset = Vector3.zero;

        public Bounds CaluclateBounds()
        {
            if (target != null)
            {
                if (target.TryGetComponent<Collider>(out var collider))
                {
                    return collider.bounds;
                }
                else
                {
                    return new(target.transform.position + offset, Vector3.zero);
                }
            }
            else
            {
                return new(fixedTarget + offset, Vector3.zero);
            }
        }
    }


    public class Ferrum25Camera : MonoBehaviour
    {
        public static Ferrum25Camera Instance = null;

        [Header("Smoothing & Offsets")]

        public float smoothSpeed = 0.1f;
        public float importantSwitchSpeed = 1f;

        public float defaultZOffset = 7f;

        [Header("Bounds")]
        public bool limitVertically = false;
        public bool limitHorizontally = true;

        //---

        public static List<CameraInterest> interests = new();

        public static Bounds maximumBoundary;

        private Vector3 cameraVelocity = Vector3.zero;

        private void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 desiredPosition = CalculateFitClamped(GetCurrentInterests(), maximumBoundary, defaultZOffset, limitVertically, limitHorizontally);

            // Smoothly interpolate between the camera's current position and the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, smoothSpeed);

            // Calculate the direction to the interest point
            Vector3 direction = (new Vector3(desiredPosition.x, desiredPosition.y, 0f) - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, smoothSpeed);
        }


        public static void RegisterInterest(GameObject target, Vector3 offset, InterestType type = InterestType.Entity)
        {
            RemoveInterest(target);
            interests.Add(new() { target = target, type = type, offset = offset });
        }

        public static void RegisterInterest(Vector3 target, Vector3 offset, InterestType type = InterestType.Entity)
        {
            RemoveInterest(target);
            interests.Add(new() { fixedTarget = target, type = type, offset = offset });
        }

        public static void RemoveInterest(GameObject target)
        {
            interests.Remove(interests.Find(e => e.target == target));
        }

        public static void RemoveInterest(Vector3 target)
        {
            interests.Remove(interests.Find(e => e.fixedTarget == target));
        }

        public static void RemoveAllTypedInterests(InterestType type = InterestType.Zone)
        {
            foreach (CameraInterest i in interests.FindAll(e => e.type == type))
            {
                interests.Remove(i);
            }
        }

        #region "Misc Utils"

        public static List<CameraInterest> GetCurrentInterests()
        {
            // Prioritize by type
            var groupedInterests = interests
                .GroupBy(interest => interest.type)
                .OrderByDescending(group => group.Key)
                .FirstOrDefault();

            return groupedInterests?.ToList() ?? new List<CameraInterest>();
        }

        public static Vector3 CalculateFitClamped(List<CameraInterest> interests, Bounds maximumBoundary, float minimumDistance = 7f, bool limitVertically = false, bool limitHorizontally = false)
        {
            if (interests == null || interests.Count == 0)
            {
                return Vector3.zero;
            }

            Camera camera = Camera.main;

            // Start with the first
            Bounds bounds = interests[0].CaluclateBounds();

            // Encapsulate all GameObjects into the bounds
            foreach (CameraInterest interest in interests)
            {
                bounds.Encapsulate(interest.CaluclateBounds());
            }

            float zDistance = Mathf.Max(DistanceToFit(bounds.size), minimumDistance);

            float vertExtent = 2.0f * zDistance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float horzExtent = vertExtent * camera.aspect;

            float minX = maximumBoundary.min.x + horzExtent / 2;
            float maxX = maximumBoundary.max.x - horzExtent / 2;
            float minY = maximumBoundary.min.y + vertExtent / 2;
            float maxY = maximumBoundary.max.y - vertExtent / 2;

            float x = limitHorizontally ? Mathf.Clamp(bounds.center.x, minX, maxX) : bounds.center.x;
            float y = limitVertically ? Mathf.Clamp(bounds.center.y, minY, maxY) : bounds.center.y;

            // Return the distance and center position
            return new(x, y, zDistance);
        }

        public static float DistanceToFit(Vector3 boxSize)
        {
            Camera camera = Camera.main;
            float fov = camera.fieldOfView;
            float aspectRatio = camera.aspect;

            float halfHeight = boxSize.y / 2.0f;
            float halfWidth = boxSize.x / 2.0f;
            float distanceHeight = halfHeight / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            float distanceWidth = halfWidth / (aspectRatio * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad));

            return Mathf.Max(distanceHeight, distanceWidth);
        }

        #endregion
    }

}