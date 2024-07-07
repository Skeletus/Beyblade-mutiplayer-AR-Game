using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleScript : MonoBehaviour
{
    [SerializeField] private Image spinSpeedBarImage;

    private Beyblade beybladeScript;
    private float startSpinSpeed;
    private float currentSpinSpeed;

    private void Awake()
    {
        beybladeScript = GetComponent<Beyblade>();

        startSpinSpeed = beybladeScript.GetSpinSpeed();
        currentSpinSpeed = beybladeScript.GetSpinSpeed();

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // comparing the speed of the beyblades
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed: " + mySpeed + " --- other player speed: " + otherPlayerSpeed);

            if(mySpeed > otherPlayerSpeed)
            {
                Debug.Log(" YOU DAMAGED other player");

                // applying damage to the slower player
                collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDaamge", RpcTarget.AllBuffered, 400f);
            }
            else
            {
                Debug.Log(" GET DAMAGED !!! ");
            }
        }
    }

    [PunRPC]
    public void OnDamage(float damageAmount)
    {
        
    }

}
