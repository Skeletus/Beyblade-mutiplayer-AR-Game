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

    private bool synchronizeVelocity = true;
    private bool synchronizeAngularVelocity = true;
    private bool isTeleportEnabled = true;
    private float teleportIfDistanceIsGreaterThan = 1.0f;

    private float distance;
    private float angle;

    private GameObject battleArenaGameObject;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();

        battleArenaGameObject = GameObject.Find("BattleArena");
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine)
        {
            rigidBody.position = Vector3.MoveTowards(rigidBody.position,
            networkedPosition, distance*(1.0f/PhotonNetwork.SerializationRate));
            rigidBody.rotation = Quaternion.RotateTowards(rigidBody.rotation,
                networkedRotation, angle*(1.0f/PhotonNetwork.SerializationRate));
        }

        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // then photonview is mine and i'm the one who controls this player
            // should send position, velocity and data to the otther players
            stream.SendNext(rigidBody.position - battleArenaGameObject.transform.position);
            stream.SendNext(rigidBody.rotation);

            if (synchronizeVelocity)
            {
                stream.SendNext(rigidBody.velocity);
            }
            if(synchronizeAngularVelocity)
            {
                stream.SendNext(rigidBody.angularVelocity);
            }
        }
        else
        {
            // called on my player gameobject that exits in remote player's game
            networkedPosition = (Vector3)stream.ReceiveNext() + battleArenaGameObject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if(isTeleportEnabled)
            {
                if(Vector3.Distance(rigidBody.position, networkedPosition) > teleportIfDistanceIsGreaterThan)
                {
                    rigidBody.position = networkedPosition;
                }
            }

            if (synchronizeVelocity || synchronizeAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if(synchronizeVelocity)
                {
                    rigidBody.velocity = (Vector3)stream.ReceiveNext();

                    networkedPosition += rigidBody.velocity * lag;

                    distance = Vector3.Distance(rigidBody.position, networkedPosition);
                }

                if(synchronizeAngularVelocity)
                {
                    rigidBody.angularVelocity = (Vector3)stream.ReceiveNext();

                    networkedRotation = Quaternion.Euler(rigidBody.angularVelocity*lag) * networkedRotation;

                    angle = Quaternion.Angle(rigidBody.rotation, networkedRotation);
                }
            }
        }
    }
}
