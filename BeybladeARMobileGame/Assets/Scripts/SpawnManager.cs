using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] beybladePrefabs;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private GameObject battleArenaGameObject;

    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }

    private void SpawnPlayer()
    {
        object beybladeSelectedIndex;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARBeybladeGame.PLAYER_SELECTION_NUMBER,
            out beybladeSelectedIndex))
        {
            //Debug.Log("Selected beyblade -> " + (int)beybladeSelectedIndex);

            int randomSpawnPointIndex = Random.Range(0, spawnPositions.Length - 1);
            Vector3 instantiatePosition = spawnPositions[randomSpawnPointIndex].position;

            GameObject playerGameObject = Instantiate(beybladePrefabs[(int)beybladeSelectedIndex],
                instantiatePosition, Quaternion.identity);

            PhotonView photonView = playerGameObject.GetComponent<PhotonView>();

            if(PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data = new object[]
                {
                    playerGameObject.transform.position - battleArenaGameObject.transform.position,
                    playerGameObject.transform.rotation,
                    photonView.ViewID,
                    beybladeSelectedIndex
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache

                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                // raise events

                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                //Debug.Log("failed to allocate a view id");
                Destroy(playerGameObject);
            }
        }
    }

    #region Photon Callback Methods
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();

            /*
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
            */
        }

    }
    #endregion
}
