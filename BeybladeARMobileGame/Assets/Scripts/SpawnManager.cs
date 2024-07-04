using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] beybladePrefabs;
    [SerializeField] private Transform[] spawnPositions;

    #region Photon Callback Methods
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object beybladeSelectedIndex;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARBeybladeGame.PLAYER_SELECTION_NUMBER,
                out beybladeSelectedIndex))
            {
                //Debug.Log("Selected beyblade -> " + (int)beybladeSelectedIndex);

                int randomSpawnPointIndex = Random.Range(0, spawnPositions.Length - 1);
                Vector3 instantiatePosition = spawnPositions[randomSpawnPointIndex].position;

                PhotonNetwork.Instantiate(beybladePrefabs[(int)beybladeSelectedIndex].name,
                    instantiatePosition, Quaternion.identity);

            }
        }

    }
    #endregion
}
