
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] public int maxHealth;
    public int currentHealth;

    [SerializeField] private GameObject itemPickupPrefab;
    [SerializeField] private ItemData wood;

    private ChunkManager chunkManager;
    private Vector2Int chunkCoordinates;
    private int treeID;
    private bool hasChunkInfo = false;
    private bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // pressing T destroys all rendered trees - for testing purposes 
        {
            TakeDamage(maxHealth);
        }
    }

    public void SetTreeInfo(ChunkManager manager, Vector2Int coordinates, int id)
    {
        chunkManager = manager;
        chunkCoordinates = coordinates;
        treeID = id;
        hasChunkInfo = true;
    }

    public void TakeDamage(int Damage)
    {
        if (isDead)
        {
            return;
        }

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
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (hasChunkInfo && chunkManager != null)
        {
            chunkManager.RegisterDestroyedTree(chunkCoordinates, treeID); // stops this tree from respawning later
        }

        SpawnItem(wood, Random.Range(5, 10));
        Destroy(gameObject);
    }

    private void SpawnItem(ItemData item, int quantity)
    {
        if (itemPickupPrefab == null || item == null)
        {
            return;
        }

        GameObject droppedItem = Instantiate(itemPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        droppedItem.GetComponent<ItemPickup>().Initialize(item, quantity);
    }

}
