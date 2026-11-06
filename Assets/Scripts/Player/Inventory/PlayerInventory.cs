
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    void Start()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
            {
                inventory.PrintInventory(); // temporary way to check the inventory
            }
        }
    }

    public int CollectItem(ItemData item, int quantity)
    {
        if (inventory == null || item == null)
        {
            return quantity;
        }

        int remaining = inventory.AddItem(item, quantity);
        int collected = quantity - remaining;

        if (collected > 0)
        {
            Debug.Log("Picked up " + collected + " " + item.itemName);
        }

        return remaining;
    }
}
