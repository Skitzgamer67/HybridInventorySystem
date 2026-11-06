using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Texture previewTexture;
    public GameObject worldModel;
    public float weight;
    public int maxStackSize = 50;
    public Vector2Int gridSize = Vector2Int.one;
    [TextArea(2, 4)] public string description;
}