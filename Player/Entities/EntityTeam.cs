using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferrum.Player
{
    public enum EntityCommonFlags {
        ALLOW_DAMAGE = 152,
    }

    [Serializable]
    public class EntityTeam
    {
        public string Name { get; set; } = "hello";

        public List<GameObject> entities = new ();

        public List<int> flags = new ();

        public bool Compare(EntityTeam other = null)
        {
            return Name == other?.Name;
        }

        //---

        public static List<EntityTeam> teams = new();
        public static EntityTeam Get(string Name)
        {
            EntityTeam et = teams.Find(x => x.Name == Name);

            if(et == null)
            {
                et = new EntityTeam() { Name = Name };
                teams.Add(et);
            }

            return et;
        }

    }
}