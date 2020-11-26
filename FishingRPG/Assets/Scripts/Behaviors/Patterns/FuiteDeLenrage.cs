﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuiteDeLenrage : MonoBehaviour
{
    static bool playOnce = false;
    static int speedModifier = 5;

    static float timer = 50f;

    public static void Play(float dotDuration, float energyCost, bool costEnergyOverTime)
    {
        Debug.Log("Fuite de L'Enrage !");

        if ((FishManager.instance.currentFishBehavior.currentStamina - energyCost) > 0)
        {
            if (!playOnce)
            {
                if (!costEnergyOverTime)
                {
                    FishManager.instance.currentFishBehavior.currentStamina -= energyCost;
                    FishManager.instance.ChangeEnduranceText();
                }

                FishManager.instance.currentFishBehavior.baseSpeed += speedModifier;
                playOnce = true;
            }

            timer += Time.fixedDeltaTime;

            if (timer > dotDuration)
            {
                if (costEnergyOverTime)
                {
                    FishManager.instance.currentFishBehavior.currentStamina -= energyCost;
                    FishManager.instance.ChangeEnduranceText();
                }

                //Direction Opposé au joueur
                timer = 0f;
            }

            FishManager.instance.currentFishBehavior.Idle();
        }
        else
        {
            FishManager.instance.currentFish.GetComponent<FishPatterns>().ResetPattern();
        }
    }
}