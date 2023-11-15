using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusaderSelectScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D player_rb;
    public Animator animator;
    void Start()
    {
        player_rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 

        player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        player_rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void idle_animation()
    {
        animator.Play("idle");
    }
}
