using System.Collections.Generic;
using UnityEngine;

namespace Ferrum.Player
{
    class ObjectDataset: Dataset
    {
        public override string searchCategory { get { return "objects"; } }

        // Dictionary to hold the Dataset
        public List<GameObjectType> objects = new();

        /// <summary>
        /// Where will created objects will go per default
        /// </summary>
        public Transform parentToHoldCreatedObjects = null;

        public override bool Has(string id)
        {
            // This function is required always
            return GetGameObject(id) != null;
        }

        // Name this function whatever you want
        public GameObject GetGameObject(string id)
        {
            return objects.Find(x => x.id == id)?.obj;
        }
        public static GameObject Get(string id)
        {
            return (GetDS(id, "objects") as ObjectDataset).GetGameObject(id);
        }

    }
}