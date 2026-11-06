
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float maxWeight = 50f;

    public List<InventorySlot> items = new List<InventorySlot>();

    public float currentWeight;

    void Start()
    {
        UpdateWeight();
    }

    // adds as many items as possible, returns the amount that couldn't fit
    public int AddItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return quantity;
        }

        int remaining = quantity;
        int maxStack = Mathf.Max(1, item.maxStackSize);

        // first try putting items into existing stacks
        foreach (InventorySlot slot in items)
        {
            if (slot.item == item && slot.quantity < maxStack)
            {
                int availableSpace = maxStack - slot.quantity;
                int amount = Mathf.Min(remaining, availableSpace);

                amount = CheckWeight(item, amount);

                slot.quantity += amount;
                remaining -= amount;

                if (remaining <= 0)
                {
                    break;
                }
            }
        }

        // if there's still items left, find somewhere new to put them
        while (remaining > 0)
        {
            int amount = Mathf.Min(remaining, maxStack);
            amount = CheckWeight(item, amount);

            if (amount <= 0)
            {
                break; // inventory is too heavy
            }

            Vector2Int position = FindEmptyPosition(item.gridSize);

            if (position.x == -1)
            {
                break; // no more room in the grid
            }

            InventorySlot newSlot = new InventorySlot(item, amount, position);
            items.Add(newSlot);

            remaining -= amount;
        }

        UpdateWeight();
        return remaining;
    }

    int CheckWeight(ItemData item, int amount)
    {
        if (item.weight <= 0)
        {
            return amount;
        }

        // checking the weight each time so it doesn't go over the limit
        UpdateWeight();

        float availableWeight = maxWeight - currentWeight;
        int allowedAmount = Mathf.FloorToInt((availableWeight + 0.0001f) / item.weight);

        return Mathf.Clamp(amount, 0, allowedAmount);
    }

    Vector2Int FindEmptyPosition(Vector2Int size)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (CanPlaceItem(position, size))
                {
                    return position;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public bool CanPlaceItem(Vector2Int position, Vector2Int size)
    {
        if (size.x <= 0 || size.y <= 0)
        {
            return false;
        }

        if (position.x < 0 || position.y < 0 ||
            position.x + size.x > gridWidth ||
            position.y + size.y > gridHeight)
        {
            return false;
        }

        // check every item already in the inventory to see if it overlaps
        foreach (InventorySlot slot in items)
        {
            Vector2Int otherPosition = slot.position;
            Vector2Int otherSize = slot.GetSize();

            bool overlaps =
                position.x < otherPosition.x + otherSize.x &&
                position.x + size.x > otherPosition.x &&
                position.y < otherPosition.y + otherSize.y &&
                position.y + size.y > otherPosition.y;

            if (overlaps)
            {
                return false;
            }
        }

        return true;
    }

    public bool MoveItem(InventorySlot slot, Vector2Int newPosition, bool rotate = false)
    {
        if (!items.Contains(slot))
        {
            return false;
        }

        Vector2Int oldPosition = slot.position;
        bool oldRotation = slot.rotated;

        items.Remove(slot); // remove temporarily so it doesn't collide with itself

        slot.rotated = rotate;
        Vector2Int size = slot.GetSize();

        if (CanPlaceItem(newPosition, size))
        {
            slot.position = newPosition;
            items.Add(slot);
            return true;
        }

        slot.position = oldPosition;
        slot.rotated = oldRotation;
        items.Add(slot);

        return false;
    }

    public bool RemoveItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        int total = 0;

        foreach (InventorySlot slot in items)
        {
            if (slot.item == item)
            {
                total += slot.quantity;
            }
        }

        if (total < quantity)
        {
            return false;
        }

        // remove from stacks until we've removed enough
        for (int i = items.Count - 1; i >= 0 && quantity > 0; i--)
        {
            if (items[i].item == item)
            {
                int amount = Mathf.Min(quantity, items[i].quantity);

                items[i].quantity -= amount;
                quantity -= amount;

                if (items[i].quantity <= 0)
                {
                    items.RemoveAt(i);
                }
            }
        }

        UpdateWeight();
        return true;
    }

    public void UpdateWeight()
    {
        currentWeight = 0;

        foreach (InventorySlot slot in items)
        {
            if (slot.item != null)
            {
                currentWeight += slot.item.weight * slot.quantity;
            }
        }
    }

    public void PrintInventory()
    {
        Debug.Log("Inventory weight: " + currentWeight + "/" + maxWeight);

        foreach (InventorySlot slot in items)
        {
            Debug.Log(slot.item.itemName + " x" + slot.quantity +
                " at " + slot.position);
        }
    }
}
