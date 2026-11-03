using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject worldModel;
    public float weight;
    public int maxStackSize = 50;
    public Vector2Int gridSize = Vector2Int.one;
}