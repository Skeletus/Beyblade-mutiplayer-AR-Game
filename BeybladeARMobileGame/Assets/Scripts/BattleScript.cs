using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class BattleScript : MonoBehaviourPun
{
    [SerializeField] private Image spinSpeedBarImage;
    [SerializeField] private TextMeshProUGUI spinSpeedRatioText;
    [SerializeField] private GameObject ui_3D_Gameobject;
    [SerializeField] private GameObject deathPanelUIPrefab;

    [Header("Default damage coefficient")]
    [SerializeField] private float commonDamageCoefficient = 0.05f;

    [Header("Player type damage coefficient")]
    [Header("Attacker")]
    [SerializeField] private float doDamageCoefficientAttacker = 10f;
    [SerializeField] private float getDamageCoefficientAttacker = 1.2f;
    [Header("Defender")]
    [SerializeField] private float doDamageCoefficientDefender = 0.75f;
    [SerializeField] private float getDamageCoefficientDefender = 1.2f;

    private GameObject deathPanelUIGameObject;
    private Beyblade beybladeScript;
    private Rigidbody rigidbody;
    private float startSpinSpeed;
    private float currentSpinSpeed;
    private bool isAttacker;
    private bool isDefender;
    private bool isDead = false;

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
        rigidbody = GetComponent<Rigidbody>();
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
        if(!isDead)
        {
            if (isAttacker)
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
            spinSpeedRatioText.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                // die
                Die();
            }
        }
        
    }

    [PunRPC]
    public void Die()
    {
        isDead = true;

        GetComponent<MovementController>().enabled = false;

        rigidbody.freezeRotation = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        beybladeScript.SetSpinSpeed(0f);
        ui_3D_Gameobject.SetActive(false);

        if (photonView.IsMine)
        {
            // count down for respawn
            StartCoroutine(Respawn());
        }
    }

    [PunRPC]
    public void Reborn()
    {
        beybladeScript.SetSpinSpeed(startSpinSpeed);
        currentSpinSpeed = beybladeScript.GetSpinSpeed();

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;

        rigidbody.freezeRotation = false;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        ui_3D_Gameobject.SetActive(true);

        isDead = false;
    }

    IEnumerator Respawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");

        if (canvasGameObject == null)
        {
            deathPanelUIGameObject = Instantiate(deathPanelUIPrefab, canvasGameObject.transform);
            yield return null;
        }
        else
        {
            deathPanelUIGameObject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;
        respawnTimeText.text = respawnTime.ToString(".00");

        while(respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }

        deathPanelUIGameObject.SetActive(false);
        GetComponent<MovementController>().enabled = true;

        photonView.RPC("Reborn", RpcTarget.AllBuffered);
    }
}
