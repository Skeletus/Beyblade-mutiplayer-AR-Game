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

    [Header("Default damage coefficient")]
    [SerializeField] private float commonDamageCoefficient = 0.05f;

    [Header("Player type damage coefficient")]
    [Header("Attacker")]
    [SerializeField] private float doDamageCoefficientAttacker = 10f;
    [SerializeField] private float getDamageCoefficientAttacker = 1.2f;
    [Header("Defender")]
    [SerializeField] private float doDamageCoefficientDefender = 0.75f;
    [SerializeField] private float getDamageCoefficientDefender = 1.2f;

    private Beyblade beybladeScript;
    private float startSpinSpeed;
    private float currentSpinSpeed;
    private bool isAttacker;
    private bool isDefender;

    private void Awake()
    {
        beybladeScript = GetComponent<Beyblade>();

        startSpinSpeed = beybladeScript.GetSpinSpeed();
        currentSpinSpeed = beybladeScript.GetSpinSpeed();

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void Start()
    {
        CheckPlayerType();
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
                float defaultDamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * commonDamageCoefficient;

                if (isAttacker)
                {
                    defaultDamageAmount *= doDamageCoefficientAttacker;
                }
                else if (isDefender)
                {
                    defaultDamageAmount *= doDamageCoefficientDefender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // applying damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, 400f);
                }
                
            }
        }
    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            beybladeScript.SetSpinSpeed(4400);
            startSpinSpeed = beybladeScript.GetSpinSpeed();
            currentSpinSpeed = beybladeScript.GetSpinSpeed();

            spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        if(isAttacker)
        {
            damageAmount *= getDamageCoefficientAttacker;
        }
        else if (isDefender)
        {
            damageAmount *= getDamageCoefficientDefender;
        }

        beybladeScript.SlowSpinSpeed(damageAmount);
        currentSpinSpeed = beybladeScript.GetSpinSpeed();

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed.ToString("F0") +  "/" + startSpinSpeed;
    }

}
