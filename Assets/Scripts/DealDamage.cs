/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    //public AssassinAnimCtrl assassinAnimCtrl;
    public PlayerHealth playerHealth;
    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollidsionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PlayerCollider")
        {
            playerHealth.Takedamage(damage);
            Debug.Log("collided with character");
            Debug.Log(damage);
        }
    }
}*/