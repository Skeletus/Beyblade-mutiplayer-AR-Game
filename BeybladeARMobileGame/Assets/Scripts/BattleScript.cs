using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviour
{
    [SerializeField] private Image spinSpeedBarImage;
    [SerializeField] private TextMeshProUGUI spinSpeedRatioText;

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

            //Debug.Log("My speed: " + mySpeed + " --- other player speed: " + otherPlayerSpeed);

            if(mySpeed > otherPlayerSpeed)
            {
                //Debug.Log(" YOU DAMAGED other player");

                if(collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // applying damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, 400f);
                }
                
            }
        }
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        beybladeScript.SlowSpinSpeed(damageAmount);
        currentSpinSpeed = beybladeScript.GetSpinSpeed();

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed +  "/" + startSpinSpeed;
    }

}
