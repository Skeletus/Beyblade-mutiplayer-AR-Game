using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

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
    
    [Header("SFX and VFX")]
    [SerializeField] private List<GameObject> pooledObjects;
    [SerializeField] private int amountToPool = 8;
    [SerializeField] private GameObject collisionEffectPrefab;

    private GameObject deathPanelUIGameObject;
    private Beyblade beybladeScript;
    private Rigidbody rigidBody;
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
        rigidBody = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            pooledObjects= new List<GameObject>();
            for(int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(collisionEffectPrefab,
                    Vector3.zero, Quaternion.identity);

                obj.SetActive(true);
                pooledObjects.Add(obj);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 +
                    new Vector3(0, 0.5f, 0);

                // instantiate collision effect particle system
                GameObject collisionEffectGameObject = GetPooledObject();
                if(collisionEffectGameObject != null)
                {
                    collisionEffectGameObject.transform.position = effectPosition;
                    collisionEffectGameObject.SetActive(true);
                    collisionEffectGameObject.GetComponentInChildren<ParticleSystem>().Play();

                    // deactivate collision effect particle system after x seconds
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameObject, 0.5f));
                }
            }

            
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

    private IEnumerator DeactivateAfterSeconds(GameObject collisionEffectGameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        collisionEffectGameObject.SetActive(true);
    }

    private GameObject GetPooledObject()
    {
        for(int i= 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
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

                if (damageAmount > 1000)
                {
                    damageAmount = 400f;
                }
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

        rigidBody.freezeRotation = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

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

        rigidBody.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        ui_3D_Gameobject.SetActive(true);

        isDead = false;
    }

    IEnumerator Respawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");

        if (deathPanelUIGameObject == null)
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
