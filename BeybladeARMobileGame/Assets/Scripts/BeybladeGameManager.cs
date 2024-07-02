using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BeybladeGameManager : MonoBehaviourPunCallbacks
{
    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region Photon Callback Methods

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    #endregion
}
