﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Rotate : MonoBehaviour
{
    Quaternion FishingRodRotaBase;
    [HideInInspector] public bool isMax = false;

    public bool result = false;

    private float holdingButtonTimer;
    public float holdingButtonMaxTimer;
    public Image holdButtonImage;

    private bool isCancelFishing = false;

    private bool isReleaseButton = true;

    private Vector3 imageTarget;

    private bool axisRelease = false;

    //New Bobber launch
    public GameObject bobberAura;
    public Camera mainCamera;
    RaycastHit hit;
    Vector3 raycastOrigin;
    public LayerMask layer;

    public Color yellowArrow;
    public Color redArrow;

    bool goodZone = true;                   //La flêche est dans une zone correcte ?

    private void Start()
    {
        FishingRodRotaBase = transform.localRotation;
        imageTarget = new Vector3(transform.position.x, transform.position.y + 20f, transform.position.z);
    }

    void FixedUpdate()
    {
        if (isReleaseButton && FishingManager.instance.isInFishingRod)
        {
            if (!PlayerManager.instance.isPressingRT)
            {
                if (PlayerManager.instance.playerInventory.inventory.currentFishOnMe < 3)
                {
                    if (Input.GetAxis("Right Trigger") == 0 && axisRelease && !FishingRodManager.instance.bobberThrowed)
                    {
                        if (goodZone)
                        {
                            StartCoroutine("Throw");
                            FishingRodManager.instance.bobber.GetComponent<Bobber>().SetBezierPoint(bobberAura.transform.position);
                        }
                        else
                        {
                            StartCoroutine("FailedThrow");
                        }

                        isMax = false;
                        axisRelease = false;
                        bobberAura.SetActive(false);
                    }

                    if ((Input.GetAxis("Right Trigger") > 0.1f) && !FishingRodManager.instance.bobberThrowed)                               //First Press RT
                    {
                        axisRelease = true;
                        if ((transform.localRotation.eulerAngles.x > 270f || (transform.localRotation.eulerAngles.x >= 0 && transform.localRotation.eulerAngles.x < 1)) && !isMax)
                        {
                            transform.Rotate(new Vector3(-2f, 0f, 0f));
                            PlayerManager.instance.playerView.GetComponent<PlayerView>().bezierBobber += 0.6f;
                        }
                        else
                        {
                            isMax = true;
                        }

                        //Aura bobber
                        raycastOrigin = mainCamera.transform.position;

                        if(Physics.Raycast(raycastOrigin, mainCamera.transform.forward, out hit, 80 , layer))
                        {
                            bobberAura.transform.position = new Vector3(hit.point.x, hit.point.y + 0.7f, hit.point.z);
                            bobberAura.SetActive(true);

                            if (hit.distance > FishingRodManager.instance.fishingLine.fMax || hit.collider.gameObject.layer == 8 )
                            {
                                bobberAura.gameObject.GetComponent<SpriteRenderer>().color = redArrow;
                                goodZone = false;
                            }
                            else
                            {
                                bobberAura.gameObject.GetComponent<SpriteRenderer>().color = yellowArrow;
                                goodZone = true;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Trop de poisson");
                }
            }
            else
            {
                if(Input.GetAxis("Right Trigger") == 0)
                {
                    PlayerManager.instance.isPressingRT = false;
                }
            }
        }
        else
        {
            if (Input.GetButtonUp("B Button"))
            {
                isReleaseButton = true;
            }
        }

        if (Input.GetButton("B Button") && FishingRodManager.instance.bobberThrowed)
        {
            holdButtonImage.gameObject.SetActive(true);

            isCancelFishing = true;
            isReleaseButton = false;
            holdingButtonTimer += Time.fixedDeltaTime;

            if (holdingButtonTimer < holdingButtonMaxTimer)
            {
                holdButtonImage.fillAmount = holdingButtonTimer / holdingButtonMaxTimer;
            }

            if (holdingButtonTimer >= holdingButtonMaxTimer)
            {
                FishingManager.instance.CancelFishing();
                FishingRodManager.instance.bobberThrowed = false;

                holdingButtonTimer = 0;
                holdButtonImage.fillAmount = 0f;
                isCancelFishing = false;
                holdButtonImage.gameObject.SetActive(false);
            }
        }

        if(Input.GetButtonUp("B Button") && isCancelFishing)
        {
            holdButtonImage.gameObject.SetActive(false);
            holdingButtonTimer = 0;
            holdButtonImage.fillAmount = 0f;
            isCancelFishing = false;
            isReleaseButton = true;
        }
    }

    IEnumerator Throw()
    {
        for (float t = 0f; t < 1f; t += 2f * Time.fixedDeltaTime)
        {
            if(t > 0.25f)
            {
                result = true;
            }
            transform.localRotation = Quaternion.Lerp(transform.localRotation, FishingRodRotaBase, t);
            yield return null;
        }
        transform.localRotation = FishingRodRotaBase;
    }

    IEnumerator FailedThrow()
    {
        for (float t = 0f; t < 1f; t += 2f * Time.fixedDeltaTime)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, FishingRodRotaBase, t);
            yield return null;
        }
        transform.localRotation = FishingRodRotaBase;
    }

    public void ResetRotation()
    {
        transform.localRotation = Quaternion.Euler(360, 0, 0);
        transform.parent.transform.localPosition = new Vector3(0f, 0.5f, 0.5f);
        transform.parent.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}

