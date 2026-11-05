
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] private int maxHealth;
    private int currentHealth;

    [SerializeField] private GameObject itemPickupPrefab;
    [SerializeField] private ItemData wood;


    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(maxHealth);
        }
    }

    public void TakeDamage(int Damage)
    {
        if (Damage >= currentHealth)
        {
            currentHealth -= currentHealth; // this check is to make sure currenthealth doesn't go below 0, and is only here incase health bars get implemented, as the only way to see trees having negative values as health, is if it were displayed as a health bar 
            TreeDeath();
        }
        else
        {

            currentHealth -= Damage;

            if (currentHealth <= 0)
            {
                TreeDeath();
            }
        }
    }

    private void TreeDeath()
    {
        SpawnItem(wood, Random.Range(5, 10));
        Destroy(gameObject);
    }

    private void SpawnItem(ItemData item, int quantity)
    {
        GameObject droppedItem = Instantiate(itemPickupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        droppedItem.GetComponent<ItemPickup>().Initialize(item, quantity);
    }

}

