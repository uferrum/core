using System.Collections.Generic;
using UnityEngine;

namespace Ferrum.Player
{

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Generic")]
    public class InventoryItemType : ScriptableObject
    {
        public string id = "item";
        public string displayName = "Special Item";

        public GameObject itemFab;

        public int maxStackItems = 999;

        public ItemStack Create(int stack = 1)
        {
            return new ItemStack { stack = stack, type = this };
        }

        public virtual bool Use()
        {
            return true; // Destroy item after use
        }

        public virtual bool Pickup()
        {
            return true; // Destroy DroppedItem or the previewer after pickup
        }
    }

    public class ItemStack
    {
        public InventoryItemType type;

        public int stack = 0;
    }

    public class Inventory
    {
        public int maxInventorySlots = 30;
        public List<ItemStack> contents = new();

        public int GetStackPos(string type)
        {
            foreach (ItemStack stack in contents)
            {
                if (stack.type.id == type) return contents.IndexOf(stack);
            }

            return -1;
        }

        public int GetStackPos(InventoryItemType type)
        {
            return GetStackPos(type.id);
        }


        public List<ItemStack> Give(ItemStack stack)
        {
            if (stack == null) return null;

            int invStackPos = GetStackPos(stack.type);
            List<ItemStack> rests = new();

            if (invStackPos < 0)
            {
                rests.AddRange(normalizeStack(stack));
            }
            else
            {
                List<ItemStack> result = normalizeStack(stack);
                contents[invStackPos] = result[0];
                if (result.Count > 1) rests.AddRange(result.GetRange(1, result.Count - 1));
            }

            if (rests.Count > 0)
            {
                for (int i = 0; i < maxInventorySlots; i++)
                {
                    if (rests.Count == 0) break;

                    if (i >= contents.Count)
                    {
                        contents.Add(rests[0]);
                    }
                    else if (contents[i] == null)
                    {
                        contents[i] = rests[0];
                    }

                    rests.RemoveAt(0);
                }
            }

            return rests;
        }


        public bool Consume(string type, int stack = 1)
        {
            int consumedStack = 0;

            foreach (ItemStack itemstack in contents)
            {
                if (itemstack.type.id != type) continue;

                int oii = itemstack.stack;
                itemstack.stack = oii - stack;
                if (itemstack.stack < 0) itemstack.stack = 0;

                consumedStack += oii - itemstack.stack;

                if (consumedStack >= stack) return true;
            }

            return false;
        }

        #region "Helpers"

        /// <summary>
        /// Converts a stack to multiple stacks with maximum respected
        /// </summary>
        /// <param name="stack">The stack</param>
        /// <returns>Many stacks, with maximum respected</returns>
        public static List<ItemStack> normalizeStack(ItemStack stack)
        {
            List<ItemStack> result = new();

            if (stack != null)
            {
                while (stack.stack > stack.type.maxStackItems)
                {
                    result.Add(new ItemStack { type = stack.type, stack = (Mathf.Min(stack.type.maxStackItems, stack.stack)) });
                    stack.stack -= stack.type.maxStackItems;
                }

                result.Add(stack);
            }

            return result;
        }


        #endregion

    }

}