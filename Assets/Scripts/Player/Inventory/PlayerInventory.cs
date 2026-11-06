
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemData startingAxe;

    void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }

    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Player is missing an Inventory component!");
            return;
        }

        if (startingAxe == null)
        {
            Debug.LogError("Starting Axe has not been assigned!");
            return;
        }

        int remaining = inventory.AddItem(startingAxe, 1);

        if (remaining == 0)
        {
            Debug.Log("Player started with an axe!");
        }
        else
        {
            Debug.LogError("Could not add starting axe to inventory!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.PrintInventory();
        }
    }

    public int CollectItem(ItemData item, int quantity)
    {
        if (inventory == null) return quantity;

        int remaining = inventory.AddItem(item, quantity);
        Debug.Log("Collected " + (quantity - remaining) + " " + item.itemName);
        return remaining;
    }
}
