using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TristanDemoScript : MonoBehaviour
{
    private Animator animator;
    public Rigidbody2D player_rb;
    // Start is called before the first frame update
    void Start()
    {
        player_rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.J))
        {
            PerformThrustAttack();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            PerformSlashAttack();
        }
    }

    private void PerformThrustAttack()
    {
        animator.SetTrigger("Stab");
        // Handle thrust attack logic
    }

    private void PerformSlashAttack()
    {
        animator.SetTrigger("Slash");
        // Handle slash attack logic
    }

}
