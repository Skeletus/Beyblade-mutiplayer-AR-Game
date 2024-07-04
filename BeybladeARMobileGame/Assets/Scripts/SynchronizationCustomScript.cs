using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SynchronizationCustomScript : MonoBehaviour, IPunObservable
{
    
    private Rigidbody rigidBody;
    private PhotonView photonView;

    private Vector3 networkedPosition;
    private Quaternion networkedRotation;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine)
        {
            rigidBody.position = Vector3.MoveTowards(rigidBody.position,
            networkedPosition, Time.fixedDeltaTime);
            rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation,
                networkedRotation, Time.fixedDeltaTime * 100);
        }

        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // then photonview is mine and i'm the one who controls this player
            // should send position, velocity and data to the otther players
            stream.SendNext(rigidBody.position);
            stream.SendNext(rigidBody.rotation);
        }
        else
        {
            // called on my player gameobject that exits in remote player's game
            networkedPosition = (Vector3)stream.ReceiveNext();
            networkedRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
