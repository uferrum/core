using UnityEngine;


namespace Ferrum.Cam
{
    public class ZoneOfInterest : MonoBehaviour
    {
        public string playerTag = "Player";

        private void Start()
        {
            if(playerTag == null || playerTag.Length <= 0)
            {
                Debug.LogError("Player tag isn't set.");
            }
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                Ferrum25Camera.RegisterInterest(gameObject, Vector3.zero);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                Ferrum25Camera.RemoveInterest(gameObject);
            }
        }
    }

}