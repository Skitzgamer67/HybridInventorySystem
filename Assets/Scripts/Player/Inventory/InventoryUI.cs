
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Text;

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
    private Dictionary<GameObject, InventorySlot> displayedSlots = new Dictionary<GameObject, InventorySlot>();
    private string lastInventoryState = "";

    private Canvas canvas;
    private GameObject tooltip;
    private TextMeshProUGUI tooltipText;
    private TextMeshProUGUI weightText;

    private InventorySlot draggedSlot;
    private GameObject draggedObject;
    private bool draggedRotation;
    private Vector2 dragOffset;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        CreateGrid();
        CreateTooltip();
        CreateWeightText();

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
            else
            {
                draggedSlot = null;
                draggedObject = null;
                tooltip.SetActive(false);
            }
        }

        if (!inventoryOpen) return;

        // refresh while open so new pickups show up
        if (draggedSlot == null && GetInventoryState() != lastInventoryState)
        {
            RefreshInventory();
        }

        if (draggedSlot != null)
        {
            UpdateDragging();
        }
        else
        {
            UpdateHover();

            if (Input.GetMouseButtonDown(0))
            {
                StartDragging();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SplitStack();
            }
        }
    }

    Camera GetUICamera()
    {
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
        return canvas.worldCamera;
    }

    string GetInventoryState()
    {
        if (inventory == null) return "";

        StringBuilder state = new StringBuilder();

        foreach (InventorySlot slot in inventory.items)
        {
            if (slot.item == null) continue;

            state.Append(slot.item.GetInstanceID()).Append(":");
            state.Append(slot.quantity).Append(":");
            state.Append(slot.position.x).Append(":");
            state.Append(slot.position.y).Append(":");
            state.Append(slot.rotated).Append(";");
        }

        return state.ToString();
    }

    void RefreshInventory()
    {
        if (inventory == null || inventoryGrid == null) return;

        tooltip.SetActive(false);

        // remove the old item displays before updating them
        foreach (GameObject obj in itemObjects)
        {
            Destroy(obj);
        }

        itemObjects.Clear();
        displayedSlots.Clear();

        foreach (InventorySlot slot in inventory.items)
        {
            if (slot.item != null)
            {
                CreateItemDisplay(slot);
            }
        }

        UpdateWeightText();
        lastInventoryState = GetInventoryState();
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
        rect.sizeDelta = GetItemPixelSize(size);
        rect.anchoredPosition = GetCellPosition(slot.position);

        Image background = itemObject.GetComponent<Image>();
        background.color = new Color(0.35f, 0.5f, 0.4f, 0.25f);
        background.raycastTarget = true;

        GameObject previewObject = new GameObject("ItemPreview", typeof(RectTransform), typeof(RawImage));
        previewObject.transform.SetParent(itemObject.transform, false);

        RectTransform previewRect = previewObject.GetComponent<RectTransform>();
        previewRect.anchorMin = Vector2.zero;
        previewRect.anchorMax = Vector2.one;
        previewRect.offsetMin = new Vector2(2, 2);
        previewRect.offsetMax = new Vector2(-2, -2);

        RawImage previewImage = previewObject.GetComponent<RawImage>();
        previewImage.texture = slot.item.previewTexture;
        previewImage.color = Color.white;
        previewImage.raycastTarget = false;

        AspectRatioFitter fitter = previewObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = 1f;

        GameObject quantityObject = new GameObject("Quantity", typeof(RectTransform), typeof(TextMeshProUGUI));
        quantityObject.transform.SetParent(itemObject.transform, false);

        RectTransform quantityRect = quantityObject.GetComponent<RectTransform>();
        quantityRect.anchorMin = new Vector2(1, 0);
        quantityRect.anchorMax = new Vector2(1, 0);
        quantityRect.pivot = new Vector2(1, 0);
        quantityRect.anchoredPosition = new Vector2(-3, 3);
        quantityRect.sizeDelta = new Vector2(50, 20);

        TextMeshProUGUI quantityText = quantityObject.GetComponent<TextMeshProUGUI>();
        quantityText.text = slot.quantity > 1 ? "x" + slot.quantity : "";
        quantityText.fontSize = 15;
        quantityText.alignment = TextAlignmentOptions.BottomRight;
        quantityText.color = Color.white;
        quantityText.fontStyle = FontStyles.Bold;
        quantityText.raycastTarget = false;

        itemObjects.Add(itemObject);
        displayedSlots.Add(itemObject, slot);
    }

    Vector2 GetItemPixelSize(Vector2Int size)
    {
        return new Vector2(
            size.x * cellSize + (size.x - 1) * cellSpacing,
            size.y * cellSize + (size.y - 1) * cellSpacing
        );
    }

    Vector2 GetCellPosition(Vector2Int position)
    {
        return new Vector2(
            position.x * (cellSize + cellSpacing),
            -position.y * (cellSize + cellSpacing)
        );
    }

    InventorySlot GetHoveredSlot()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> entry in displayedSlots)
        {
            if (entry.Key != null && RectTransformUtility.RectangleContainsScreenPoint(entry.Key.GetComponent<RectTransform>(), Input.mousePosition, GetUICamera()))
            {
                return entry.Value;
            }
        }

        return null;
    }

    GameObject GetSlotObject(InventorySlot slot)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> entry in displayedSlots)
        {
            if (entry.Value == slot) return entry.Key;
        }

        return null;
    }

    void StartDragging()
    {
        draggedSlot = GetHoveredSlot();
        if (draggedSlot == null) return;

        draggedObject = GetSlotObject(draggedSlot);
        draggedRotation = draggedSlot.rotated;

        RectTransform rect = draggedObject.GetComponent<RectTransform>();
        Vector2 mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryGrid, Input.mousePosition, GetUICamera(), out mousePosition);

        dragOffset = rect.anchoredPosition - mousePosition;

        draggedObject.transform.SetAsLastSibling();
        tooltip.SetActive(false);
    }

    void UpdateDragging()
    {
        if (draggedObject == null)
        {
            draggedSlot = null;
            return;
        }

        RectTransform rect = draggedObject.GetComponent<RectTransform>();

        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryGrid, Input.mousePosition, GetUICamera(), out mousePosition);

        rect.anchoredPosition = mousePosition + dragOffset;

        if (Input.GetKeyDown(KeyCode.R))
        {
            draggedRotation = !draggedRotation;

            Vector2Int size = draggedSlot.item.gridSize;

            if (draggedRotation)
            {
                size = new Vector2Int(size.y, size.x);
            }

            rect.sizeDelta = GetItemPixelSize(size);
        }

        if (Input.GetMouseButtonUp(0))
        {
            FinishDragging();
        }
    }

    void FinishDragging()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryGrid, Input.mousePosition, GetUICamera(), out mousePosition);

        Vector2 topLeft = mousePosition + dragOffset;

        int x = Mathf.RoundToInt(topLeft.x / (cellSize + cellSpacing));
        int y = Mathf.RoundToInt(-topLeft.y / (cellSize + cellSpacing));

        Vector2Int newPosition = new Vector2Int(x, y);

        InventorySlot target = null;

        foreach (InventorySlot slot in inventory.items)
        {
            if (slot == draggedSlot) continue;

            Vector2Int size = slot.GetSize();

            if (x >= slot.position.x && x < slot.position.x + size.x && y >= slot.position.y && y < slot.position.y + size.y)
            {
                target = slot;
                break;
            }
        }

        bool merged = false;

        if (target != null && target.item == draggedSlot.item && target != draggedSlot)
        {
            int maxStack = Mathf.Max(1, target.item.maxStackSize);
            int available = maxStack - target.quantity;
            int amount = Mathf.Min(available, draggedSlot.quantity);

            if (amount > 0)
            {
                target.quantity += amount;
                draggedSlot.quantity -= amount;

                if (draggedSlot.quantity <= 0)
                {
                    inventory.items.Remove(draggedSlot);
                }

                merged = true;
            }
        }

        if (!merged)
        {
            inventory.MoveItem(draggedSlot, newPosition, draggedRotation);
        }

        draggedSlot = null;
        draggedObject = null;

        RefreshInventory();
    }

    void SplitStack()
    {
        InventorySlot slot = GetHoveredSlot();

        if (slot == null || slot.quantity <= 1) return;

        int splitAmount = slot.quantity / 2;
        Vector2Int size = slot.GetSize();

        for (int y = 0; y <= gridHeight - size.y; y++)
        {
            for (int x = 0; x <= gridWidth - size.x; x++)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (inventory.CanPlaceItem(position, size))
                {
                    slot.quantity -= splitAmount;

                    InventorySlot newSlot = new InventorySlot(slot.item, splitAmount, position, slot.rotated);
                    inventory.items.Add(newSlot);

                    inventory.UpdateWeight();
                    RefreshInventory();
                    return;
                }
            }
        }
    }

    void UpdateHover()
    {
        InventorySlot slot = GetHoveredSlot();

        if (slot == null)
        {
            tooltip.SetActive(false);
            return;
        }

        string content = slot.item.itemName + " x" + slot.quantity;

        if (!string.IsNullOrEmpty(slot.item.description))
        {
            content += "\n" + slot.item.description;
        }

        tooltipText.text = content;
        tooltip.SetActive(true);
        tooltip.transform.SetAsLastSibling();

        RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();
        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();

        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, Input.mousePosition, GetUICamera(), out mousePosition);

        tooltipRect.anchoredPosition = mousePosition + new Vector2(15, -15);
    }

    void CreateTooltip()
    {
        tooltip = new GameObject("InventoryTooltip", typeof(RectTransform), typeof(Image));
        tooltip.transform.SetParent(inventoryPanel.transform, false);

        RectTransform rect = tooltip.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0, 1);
        rect.sizeDelta = new Vector2(220, 90);

        Image background = tooltip.GetComponent<Image>();
        background.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("TooltipText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(tooltip.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8, 8);
        textRect.offsetMax = new Vector2(-8, -8);

        tooltipText = textObject.GetComponent<TextMeshProUGUI>();
        tooltipText.fontSize = 14;
        tooltipText.color = Color.white;
        tooltipText.enableWordWrapping = true;
        tooltipText.raycastTarget = false;

        tooltip.SetActive(false);
    }

    void CreateWeightText()
    {
        GameObject weightObject = new GameObject("InventoryWeight", typeof(RectTransform), typeof(TextMeshProUGUI));
        weightObject.transform.SetParent(inventoryPanel.transform, false);

        RectTransform rect = weightObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, -210);
        rect.sizeDelta = new Vector2(250, 30);

        weightText = weightObject.GetComponent<TextMeshProUGUI>();
        weightText.fontSize = 18;
        weightText.alignment = TextAlignmentOptions.Center;
        weightText.color = Color.white;
        weightText.raycastTarget = false;
    }

    void UpdateWeightText()
    {
        if (inventory == null || weightText == null) return;

        inventory.UpdateWeight();
        weightText.text = "Weight: " + inventory.currentWeight.ToString("0.0") + " / 50 kg";
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
