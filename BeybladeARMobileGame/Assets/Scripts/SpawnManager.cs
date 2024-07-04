using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] beybladePrefabs;

    #region Photon Callback Methods
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object beybladeSelectedIndex;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARBeybladeGame.PLAYER_SELECTION_NUMBER,
                out beybladeSelectedIndex))
            {
                Debug.Log("Selected beyblade -> " + (int)beybladeSelectedIndex);
            }
        }

    }
    #endregion
}
