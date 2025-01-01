using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameUI : UIFollowTarget
{
    [SerializeField] TextMeshProUGUI name_tm, shadow_tm;
    [SerializeField] GameObject nameContainer;

    [SerializeField] GameObject playerIndicator;

    public void SetName(string name)
    {
        if (name_tm)
        {
            name_tm.text = name;
            shadow_tm.text = name;
        }            
        
    }

    public void SetPlayerIndicator(bool t)
    {
        if (playerIndicator)
            playerIndicator.SetActive(t);
    }

    public void Show(bool t)
    {
        if (nameContainer)
            nameContainer.SetActive(t);
    }

    
}
