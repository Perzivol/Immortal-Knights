/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    public GameObject healthText;

    public bool disablesSimulation = false;

    public bool canTurnInvincible = false;

    public float invincibilityTime = 0.25f;

    Animator animator;

    Rigidbody2D rb;

    Collider2D phyicsCollider;

    bool isAlive = true;

    private float invincibleTimeElapsed = 0f;

    public float Health
    {
        set
        {
            // When health is dropped (new value less than old value), play hit animation and show damage taken as text
            if(value < _health)
            {
                // might want to put this into the character controller instead
                animator.SetTrigger("Hit");

                //Spawn damage text right above the character
                RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
                textTransform.transform.poistion = Camera.main.World(gameObject.transform.position);

                Canvas canvas = Gameobject.FindObjectOfType<Canvas>();
                textTransform.SetParent(canvas.transform);
            }

            _health = value;

            if(_health <= 0){
                animator.SetBool("isAlive", false);
                Targetable = false;
            }            
        }
        get{
            return _health;
        }
    }
    // method knocks back the character but I don't think I want this. commenting out for now
    /*
    public void OnHit(float damage, Vector2 knockback)
    {
        if(!Invincible)
        {
            Health -= damage;

            //Apply force to the character
            //Impulse for instantaneour forces
            rb.AddForce(knockback, ForceMode2D.Impulse);
            //makes character invincible for a bit after getting hit
            if(canTurnInvincible) {
                //Activate invinceibility and timer
                Invincible = true;
            }
        }
    }
   

    // Take damage without knockback
    public void OnHit(float damage)
    {
        if(!Invincible)
        {
            Health -= damage;

            if(canTurnInvincible)
            {
                // Activate invincibility and timer
                Invincible = true;
            }
        }
    }
}
 */