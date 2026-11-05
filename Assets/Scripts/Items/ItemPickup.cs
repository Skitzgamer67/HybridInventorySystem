
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public int quantity = 1;

    [SerializeField] private Transform visual;

    void Start()
    {
        if (item != null && visual.childCount == 0)
        {
            SpawnModel();
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
