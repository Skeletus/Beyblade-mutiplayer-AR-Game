using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    private void Start()
    {
        if (photonView.IsMine)
        {
            // the player is local player
            transform.GetComponent<MovementController>().enabled = true;
            transform.GetComponent<MovementController>().EnableJoystick();
        }
        else
        {
            // the player is remote player
            transform.GetComponent<MovementController>().enabled = false;
            transform.GetComponent<MovementController>().DisableJoystick();
        }
    }
}
