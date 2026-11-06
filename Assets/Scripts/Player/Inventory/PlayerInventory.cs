
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    void Start()
    {
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            PickupNearestItem();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.PrintInventory(); // temporary way to check what was picked up
        }
    }

    void PickupNearestItem()
    {
        if (inventory == null)
        {
            return;
        }

        ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();

        ItemPickup nearestItem = null;
        float nearestDistance = pickupRange;

        foreach (ItemPickup pickup in pickups)
        {
            float distance = Vector3.Distance(transform.position, pickup.transform.position);

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = pickup;
            }
        }

        if (nearestItem == null)
        {
            return;
        }

        int originalQuantity = nearestItem.quantity;
        int remaining = inventory.AddItem(nearestItem.item, originalQuantity);
        int collected = originalQuantity - remaining;

        if (collected <= 0)
        {
            Debug.Log("Inventory is full or too heavy");
            return;
        }

        Debug.Log("Picked up " + collected + " " + nearestItem.item.itemName);

        if (remaining <= 0)
        {
            Destroy(nearestItem.gameObject);
        }
        else
        {
            nearestItem.quantity = remaining; // leave whatever couldn't fit on the ground
        }
    }
}
