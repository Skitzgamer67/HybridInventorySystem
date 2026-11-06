
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public int quantity = 1;

    [SerializeField] private Transform visual;

    [SerializeField] private float pickupRange = 2.5f;
    [SerializeField] private float pickupSpeed = 6f;
    [SerializeField] private float shrinkSpeed = 4f;
    [SerializeField] private float collectDistance = 0.3f;

    private Transform player;
    private PlayerInventory playerInventory;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private bool collecting = false;
    private float retryTime = 0f;

    void Start()
    {
        originalScale = transform.localScale;

        playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory != null)
        {
            player = playerInventory.transform;
        }

        if (item != null && visual != null && visual.childCount == 0)
        {
            SpawnModel();
        }
    }

    void Update()
    {
        if (player == null || playerInventory == null || item == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (!collecting && Time.time >= retryTime && distance <= pickupRange)
        {
            collecting = true;
            originalPosition = transform.position; // remember where it was dropped
        }

        if (collecting)
        {
            Vector3 targetPosition = player.position + Vector3.up * 0.7f;

            // item moves towards the player while shrinking
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, pickupSpeed * Time.deltaTime);

            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= collectDistance)
            {
                int remaining = playerInventory.CollectItem(item, quantity);

                if (remaining <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    // inventory couldn't fit everything, so leave the rest on the ground
                    quantity = remaining;
                    collecting = false;

                    transform.position = originalPosition;
                    transform.localScale = originalScale;

                    retryTime = Time.time + 1f;
                }
            }
        }
    }

    public void Initialize(ItemData newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;

        SpawnModel();
    }

    void SpawnModel()
    {
        if (item == null || item.worldModel == null || visual == null)
        {
            return;
        }

        if (visual.childCount > 0)
        {
            for (int i = visual.childCount - 1; i >= 0; i--)
            {
                Destroy(visual.GetChild(i).gameObject);
            }
        }

        Instantiate(item.worldModel, visual);
    }
}
