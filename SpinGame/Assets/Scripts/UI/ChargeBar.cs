using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : UIFollowTarget
{
    [SerializeField] Image barImage;
    [SerializeField] GameObject container;

    ControllableCharacter playerRef;

    private void Awake()
    {
        container.SetActive(false);
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        playerRef = target.GetComponent<ControllableCharacter>();
    }

    void Update()
    {
        if (playerRef)
        {
            float chargeValue = playerRef.GetChargeValue()/playerRef.GetMaxCharge();
            if (chargeValue > 0)
            {
                if (!container.activeSelf)
                {
                    container.SetActive(true);
                }
                barImage.fillAmount = chargeValue;
            }else{
                if (container.activeSelf)
                {
                    container.SetActive(false);
                    barImage.fillAmount = 0;
                }
            }
        }
    }
}
