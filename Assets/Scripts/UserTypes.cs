using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTypes : MonoBehaviour
{
    public AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        // When spacebar is hit
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0))
        {
            source.Play();
        }
    }

}