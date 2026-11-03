using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tree : MonoBehaviour
{

    [SerializeField] public int maxHealth;
    public int currentHealth;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Destroy(gameObject);
        // SpawnItem(wood, Random.Range(5, 10));
    }
    
}
