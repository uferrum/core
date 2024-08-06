using Ferrum.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ferrum
{
    /// <summary>
    /// Provides a way to quickly select Objects in Hierarchy like Elements in HTML.
    /// </summary>
    public static class Scene
    {
        public class Selector
        {
            public string selectorString = "";

            public string Name = "";
            public int Layer = -1;
            public string Tag = "";

            public List<GameObject> Filter(List<GameObject> objects)
            {
                if (selectorString == "::root") return GameObject.FindObjectsOfType<GameObject>().ToList();
                else if (selectorString == "*" || selectorString == ">") return objects;

                Populate(); // No special selectors ? now populate fields ( this lazy way makes selectors fast )

                List<GameObject> result = new();

                foreach (GameObject gobj in objects)
                {
                    if (Name.Length > 0 && gobj.name != Name) continue;
                    if (Layer >= 0 && gobj.layer != Layer) continue;
                    if (Tag.Length > 0 && gobj.tag != Tag) continue;
                    result.Add(gobj);
                }

                return result;
            }

            /// <summary>
            /// Populates Name, Layer, Tag
            /// Used internally, generally not recommended to use it externally
            /// </summary>
            private void Populate()
            {
                // We will add Layer and Tag later
                Name = selectorString;
            }

            public static List<Selector> QueryToSelectors(string query)
            {
                return query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Create(p.Trim())).ToList();
            }

            public static Selector Create(string selector)
            {
                // We will add Layer and Tag later
                return new Selector { selectorString = selector };
            }
        }

        /// <summary>
        /// Searches for a specific GameObject.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <param name="parent">The parent GameObject to search within.</param>
        /// <returns>The wanted GameObject.</returns>
        public static GameObject Find(string query, GameObject parent = null)    
        {
            return Find<GameObject>(query, parent);
        }

        /// <summary>
        /// Searches for a specific Object or Component.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <param name="parent">The parent GameObject to search within.</param>
        /// <returns>The wanted Object.</returns>
        public static T Find<T>(string query, GameObject parent = null) where T : UnityEngine.Object
        {
            var results = FindAll<T>(query, parent);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Searches for many GameObjects.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <param name="parent">The parent GameObject to search within.</param>
        /// <returns>An array of GameObjects.</returns>
        public static List<GameObject> FindAll(string query, GameObject parent = null)
        {
            return FindAll<GameObject>(query, parent);
        }

        /// <summary>
        /// Searches for many Objects or Components.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <param name="parent">The parent GameObject to search within.</param>
        /// <returns>An array of Objects or Components.</returns>
        public static List<T> FindAll<T>(string query, GameObject parent = null) where T : UnityEngine.Object
        {
            var currentObjects = parent == null
                ? GameObject.FindObjectsOfType<GameObject>().ToList()
                : parent.FindAllChildren();

            List<GameObject> results = FilterAsc(currentObjects, Selector.QueryToSelectors(query));

            if (typeof(T) == typeof(GameObject))
            {
                return results.Distinct().Cast<T>().ToList();
            }
            else
            {
                return results.Distinct().Select(go => go.GetComponent<T>()).ToList();
            }
        }

        #region "Objects Management Utils"
        /// <summary>
        /// Creates an object by it's id, That object must be defined with Ferrum.Player.Dataset
        /// </summary>
        public static GameObject CreateObject(string id, Vector3 initialPos = new(), Transform parent = null)
        {
            ObjectDataset ed = Dataset.GetDS(id, "objects") as ObjectDataset;

            GameObject nobj = UnityEngine.Object.Instantiate(ed.GetGameObject(id));

            nobj.transform.position = initialPos;

            if(parent == null)
            {
                parent = ed.parentToHoldCreatedObjects ? ed.parentToHoldCreatedObjects : ed.transform;
            }

            nobj.transform.parent = parent;

            return nobj;
        }

        #endregion

        public static List<GameObject> FilterAsc(List<GameObject> objects, List<Selector> selectorsAsc)
        {
            var filteredObjects = new List<GameObject>(objects);


            bool rootIteration = true; // Skip getting children on first iteration
            foreach (Selector selector in selectorsAsc)
            {
                if (!rootIteration)
                {
                    List<GameObject> newFiltered = new();
                    // Gets all children on new iteration
                    foreach (GameObject gobj in filteredObjects)
                    {
                        newFiltered.AddRange(gobj.transform.FindAllChildren().Select(p => p.gameObject));
                    }
                    filteredObjects = newFiltered;
                }

                rootIteration = false;

                if (selector == null) return filteredObjects;
                filteredObjects = selector.Filter(filteredObjects);
            }

            return filteredObjects;
        }


        /// <summary>
        /// Generates a selector which can be used to get an object.
        /// </summary>
        /// <param name="obj">The object to generate the selector for.</param>
        /// <param name="parent">The common parent.</param>
        /// <returns>The query string.</returns>
        public static string GetSelector(GameObject obj, GameObject parent = null)
        {
            if (parent == null)
            {
                parent = UnityEngine.Object.FindObjectOfType<Transform>().root.gameObject;
            }

            List<string> selectorParts = new List<string>();
            while (obj != null && obj != parent)
            {
                string part = obj.name;
                if (obj.transform.parent != null && obj.transform.parent != parent.transform)
                {
                    part = $"{obj.transform.parent.name} > {part}";
                }
                selectorParts.Insert(0, part);
                obj = obj.transform.parent?.gameObject;
            }

            return string.Join(" ", selectorParts);
        }
    }

    public static class GameObjectExtensions
    {
        /// <summary>
        /// Finds a component, or creates it.
        /// Useful for temporary components.
        /// </summary>
        /// <returns>The Component.</returns>
        public static T Compo<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Searches for a specific Object, Component, or GameObject inside this GameObject.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <returns>The Object or Component.</returns>
        public static T Find<T>(this GameObject gameObject, string query) where T : UnityEngine.Object
        {
            return Scene.Find<T>(query, gameObject);
        }

        /// <summary>
        /// Returns the parent of this GameObject in the hierarchy.
        /// </summary>
        /// <returns>The parent GameObject.</returns>
        public static GameObject Parent(this GameObject gameObject)
        {
            return gameObject.transform.parent?.gameObject;
        }

        /// <summary>
        /// Generates a selector which can be used to get this object.
        /// </summary>
        /// <param name="usedParent">The common parent.</param>
        /// <returns>The query string.</returns>
        public static string GetSelector(this GameObject gameObject, GameObject usedParent = null)
        {
            return Scene.GetSelector(gameObject, usedParent);
        }

        public static List<GameObject> FindAllChildren(this GameObject parent)
        {
            return parent.GetComponentsInChildren<Transform>(true).Select(c => c.gameObject).ToList();
        }
    }

    public static class TransformExtensions
    {
        /// <summary>
        /// Gets all children recursively
        /// </summary>
        /// <returns>A list of children transforms</returns>
        public static List<Transform> FindAllChildren(this Transform parent)
        {
            return parent.gameObject.GetComponentsInChildren<Transform>(true).ToList();
        }
    }
}
