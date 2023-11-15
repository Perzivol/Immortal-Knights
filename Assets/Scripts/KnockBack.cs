using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Photon.Pun;

public interface ICharacterController
{
    void OnKnockbackStart();
    void OnKnockbackEnd();
}
public class KnockBack : MonoBehaviourPun
{
    private ICharacterController characterController;

    [SerializeField]
    private Rigidbody2D rb2d;
    public PhotonView view;
    private float strength = 50; 
    private float delay = 0.4f;
    public UnityEvent OnBegin, OnDone;

    PhotonView localview;
    void Start()
    {
        view = GetComponent<PhotonView>();
        rb2d = GetComponent<Rigidbody2D>();
        characterController = GetComponent<ICharacterController>();

        if (!rb2d)
        {
            Debug.LogError("No Rigidbody2D found on the character!");
        }

        if (characterController == null)
        {
            Debug.LogError("The character doesn't implement the ICharacterController interface!");
        }
        
    }

    void Update() 
    {
        /* if(view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb2d.AddForce(new Vector2(-1, 0) * 16, ForceMode2D.Impulse);
            }
        } */
    }
    [PunRPC]
    private void ApplyKnockback(Vector2 direction)
    {
       // Debug.Log("ApplyKnockback");
        //Debug.Log("direction: " + direction);
        //Debug.Log("strength: " + strength);
        rb2d.AddForce(direction * strength, ForceMode2D.Impulse);

        // Apply damping effect
        StartCoroutine(DampKnockback());
    }

    public void PlayFeedback(GameObject sender)
    {
       // Debug.Log("Knockback");
        StopAllCoroutines();
        OnBegin?.Invoke();

        Vector2 direction = (transform.position - sender.transform.position).normalized;

        PhotonView senderView = sender.GetComponent<PhotonView>();
        if (senderView != null && senderView.IsMine)
        {
           // Debug.Log("Knockback in my view");
            view.RPC("ApplyKnockback", RpcTarget.Others, direction);
            view.RPC("ResetKnockback", RpcTarget.All);
        }

        // Notify character controller
        if (characterController != null)
        {
            characterController.OnKnockbackStart();
        }
    }

    [PunRPC]
    private void ResetKnockback()
    {
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("ResetKnockback");
        rb2d.velocity = Vector2.zero;
        OnDone?.Invoke();

        // Notify character controller
        if (characterController != null)
        {
            characterController.OnKnockbackEnd();
        }
    }

    private IEnumerator DampKnockback()
    {
        yield return new WaitForSeconds(0.1f);

        // Reduce knockback force gradually over time
        while (rb2d.velocity.magnitude > 0.1f)
        {
            rb2d.velocity = Vector2.Lerp(rb2d.velocity, Vector2.zero, Time.deltaTime * 5f);
            yield return null;
        }
    }

}
