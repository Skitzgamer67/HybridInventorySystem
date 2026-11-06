using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private RectTransform inventoryGrid;

    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float cellSize = 55f;
    [SerializeField] private float cellSpacing = 5f;
    private bool inventoryOpen = false;
    private List<GameObject> itemObjects = new List<GameObject>();

    void Start()
    {
        CreateGrid(); // generates the 48 inventory cells
        inventoryPanel.SetActive(false); // inventory starts closed
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryOpen = !inventoryOpen;
            inventoryPanel.SetActive(inventoryOpen);

            if (inventoryOpen)
            {
                RefreshInventory();
            }
        }

        // refresh while open so new pickups show up
        if (inventoryOpen)
        {
            RefreshInventory();
        }
    }

    void RefreshInventory()
    {
        if (inventory == null || inventoryGrid == null)
        {
            return;
        }

        // remove the old item displays before updating them
        foreach (GameObject obj in itemObjects)
        {
            Destroy(obj);
        }

        itemObjects.Clear();

        foreach (InventorySlot slot in inventory.items)
        {
            CreateItemDisplay(slot);
        }
    }

    void CreateItemDisplay(InventorySlot slot)
    {
        Vector2Int size = slot.GetSize();

        GameObject itemObject = new GameObject("Item_" + slot.item.itemName, typeof(RectTransform), typeof(Image));
        itemObject.transform.SetParent(inventoryGrid, false);

        RectTransform rect = itemObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        rect.sizeDelta = new Vector2(
            size.x * cellSize + (size.x - 1) * cellSpacing,
            size.y * cellSize + (size.y - 1) * cellSpacing
        );

        rect.anchoredPosition = new Vector2(
            slot.position.x * (cellSize + cellSpacing),
            -slot.position.y * (cellSize + cellSpacing)
        );

        Image image = itemObject.GetComponent<Image>();
        image.color = new Color(0.35f, 0.5f, 0.4f, 1f);
        image.raycastTarget = false;

        GameObject textObject = new GameObject("ItemText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(itemObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = slot.item.itemName + " x" + slot.quantity;
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        itemObjects.Add(itemObject);
    }

    void CreateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject cell = new GameObject("Cell_" + x + "_" + y, typeof(RectTransform), typeof(Image));
                cell.transform.SetParent(inventoryGrid, false);

                RectTransform rect = cell.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                rect.anchoredPosition = new Vector2(
                    x * (cellSize + cellSpacing),
                    -y * (cellSize + cellSpacing)
                );

                Image image = cell.GetComponent<Image>();
                image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                image.raycastTarget = false;
            }
        }
    }
}
