using System;
using System.Collections.Generic;
using UnityEngine;

namespace MedievalChinaARPG.Items
{
    [Serializable]
    public class InventorySlot
    {
        public ItemDefinition item;
        public int count;
        public int upgradeTier;

        public bool IsEmpty => item == null || count <= 0;
    }

    public class Inventory : MonoBehaviour
    {
        [SerializeField] int capacity = 24;
        readonly List<InventorySlot> _slots = new();

        public IReadOnlyList<InventorySlot> Slots => _slots;
        public event Action Changed;

        void Awake()
        {
            EnsureCapacity();
        }

        void EnsureCapacity()
        {
            while (_slots.Count < capacity)
                _slots.Add(new InventorySlot());
        }

        public bool Add(ItemDefinition item, int count = 1, int tier = 0)
        {
            if (item == null || count <= 0) return false;
            EnsureCapacity();

            if (item.maxStack > 1)
            {
                foreach (var slot in _slots)
                {
                    if (slot.item == item && slot.count < item.maxStack)
                    {
                        int space = item.maxStack - slot.count;
                        int take = Mathf.Min(space, count);
                        slot.count += take;
                        count -= take;
                        if (count <= 0)
                        {
                            Changed?.Invoke();
                            return true;
                        }
                    }
                }
            }

            while (count > 0)
            {
                var empty = _slots.Find(s => s.IsEmpty);
                if (empty == null) return false;
                int take = Mathf.Min(item.maxStack, count);
                empty.item = item;
                empty.count = take;
                empty.upgradeTier = tier;
                count -= take;
            }

            Changed?.Invoke();
            return true;
        }

        public bool Remove(ItemDefinition item, int count = 1)
        {
            if (item == null || count <= 0) return false;
            int remaining = count;
            foreach (var slot in _slots)
            {
                if (slot.item != item) continue;
                int take = Mathf.Min(slot.count, remaining);
                slot.count -= take;
                remaining -= take;
                if (slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                    slot.upgradeTier = 0;
                }
                if (remaining <= 0) break;
            }

            if (remaining > 0) return false;
            Changed?.Invoke();
            return true;
        }

        public int CountOf(ItemDefinition item)
        {
            int total = 0;
            foreach (var slot in _slots)
                if (slot.item == item) total += slot.count;
            return total;
        }

        public InventorySlot GetSlot(int index)
        {
            EnsureCapacity();
            if (index < 0 || index >= _slots.Count) return null;
            return _slots[index];
        }

        public void NotifyChanged() => Changed?.Invoke();
    }
}
