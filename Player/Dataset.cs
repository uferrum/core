using System;
using System.Collections.Generic;
using UnityEngine;


namespace Ferrum.Player
{
    [Serializable]
    public class GameObjectType
    {
        public string id;
        public GameObject obj;
    }

    /// <summary>
    /// Used to store global common things for a better game development
    /// </summary>
    public class Dataset : MonoBehaviour
    {
        public static Dataset Instance = null;
        public static List<Func<string, string, Dataset>> handlers = new();

        public static bool TryGetDataset(out Dataset dataset, string id, string searchCategory = "objects")
        {
            foreach (Func<string, string, Dataset> handler in handlers)
            {
                Dataset ds = handler(id, searchCategory);
                if (ds != null)
                {
                    dataset = ds;
                    return true;
                }
            }

            dataset = null;
            return false;
        }

        public static Dataset GetDS(string id, string searchCategory)
        {
            if(TryGetDataset(out Dataset ds, id, searchCategory))
            {
                return ds;
            }
            else
            {
                throw new Exception("Cannot find target! Ensure you have defined it in the matching Dataset.");
            }
        }

        //---

        public virtual string searchCategory { get { return "none"; } }

        public void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                handlers.Add((string id, string searchCategory) =>
                {
                    Dataset[] datasets = FindObjectsOfType<Dataset>();

                    // Iterate through each Dataset
                    foreach (var dataset in datasets)
                    {
                        if (dataset.searchCategory == searchCategory && dataset.Has(id)) return dataset;
                    }

                    return null;
                });
            }
        }

        // ---

        public virtual bool Has(string id)
        {
            // This function is required always
            return false;
        }
    }

}
