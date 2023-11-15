using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Photon.Pun;
using Photon.Realtime;

public class ObservableSkeletonAnimation : MonoBehaviourPunCallbacks, IPunObservable
{
    public SkeletonAnimation skeletonAnimation;
    private bool isPlaying;
    private float time;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Write the current state of the skeleton animation to the network
            stream.SendNext(isPlaying);
            stream.SendNext(time);
        }
        else
        {
            // Read the state of the skeleton animation from the network and update the local state
            isPlaying = (bool)stream.ReceiveNext();
            time = (float)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        // Update the local state of the skeleton animation based on the network state
        if (isPlaying)
        {
            skeletonAnimation.timeScale = 1f;
            skeletonAnimation.state.Update(time);
            skeletonAnimation.state.Apply(skeletonAnimation.skeleton);
        }
        else
        {
            skeletonAnimation.timeScale = 0f;
        }
    }
}
