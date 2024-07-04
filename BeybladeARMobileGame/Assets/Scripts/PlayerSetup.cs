using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI playerNameText;

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
        SetPlayerName();
    }

    private void SetPlayerName()
    {
        if (playerNameText != null)
        {
            if (photonView.IsMine)
            {
                playerNameText.text = "YOU";
                playerNameText.color = Color.green;
            }
            else
            {
                playerNameText.text = photonView.Owner.NickName;
            }

            
        }
    }
}
