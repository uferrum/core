using Ferrum.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Ferrum.Player
{

    public class EntityTarget
    {
        public GameObject target;

        public bool isRadarAssigned = false;
    }

    public class Entity : MonoBehaviour
    {
        [SerializeField]
        public EntityTeam team = null;

        [SerializeField]
        public AssignedData data = new();

        [SerializeField]
        public List<int> flags = new();

        [SerializeField]
        public List<int> negativeFlags = new();

        public bool HasFlag(int flag) {
            if (negativeFlags.Contains(flag)) return false;
            if (flags.Contains(flag)) return true;

            return team.flags.Contains(flag);
        }
    }

    public class LivingEntity: Entity
    {
        public void Start()
        {
            float maxHealth = data.GetFloat("maxHealth");

            if (maxHealth == 0f)
            {
                maxHealth = 10f;
                data.SetFloat("maxHealth", maxHealth);
            }

            float health = data.GetFloat("health");

            if (health == 0f || health > maxHealth)
            {
                health = maxHealth;
                data.SetFloat("health", health);
            }
        }

        // Update is called once per frame
        public void Update()
        {
            if (data.GetFloat("health") <= 0f)
            {
                Die();
            }

            TargetingFunctionality();

            if (!data.GetBoolean("NoAI", false)) DoAITick();
        }

        public virtual void Die()
        {
            // Do something
            Destroy(gameObject);
        }

        #region "AIAndMovement"

        public virtual void DoAITick()
        {

        }

        #endregion

        #region "Targeting"

        public EntityTarget target = null;
        public float maxRadarDistance = 5f;


        void TargetingFunctionality()
        {
            if (target != null && target.target != null)
            {
                if (target.isRadarAssigned && Vector3.Distance(transform.position, target.target.transform.position) > maxRadarDistance)
                {
                    target = null;
                }
            }
            else
            {
                target = DoRadarScan();
            }
        }

        public virtual EntityTarget DoRadarScan()
        {
            return null;
        }

        #endregion
    }

}