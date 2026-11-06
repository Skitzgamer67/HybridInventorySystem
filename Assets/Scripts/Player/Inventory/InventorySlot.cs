using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
    public Vector2Int position;
    public bool rotated;

    public InventorySlot(ItemData newItem, int newQuantity, Vector2Int newPosition, bool isRotated = false)
    {
        item = newItem;
        quantity = newQuantity;
        position = newPosition;
        rotated = isRotated;
    }

    public Vector2Int GetSize()
    {
        if (rotated)
        {
            return new Vector2Int(item.gridSize.y, item.gridSize.x);
        }

        return item.gridSize;
    }
}
