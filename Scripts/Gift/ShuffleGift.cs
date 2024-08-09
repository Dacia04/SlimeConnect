using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class ShuffleGift :GiftBase
{
    [SerializeField] private int RangeRandom;
    private int buffAmount;
    protected override void SetFloatingText()
    {
        GameObject go = Instantiate(FloatingTextPrefab,transform.position,Quaternion.identity,transform);
        TextMeshPro text = go.GetComponent<TextMeshPro>();
        if(buffAmount>0)
            text.text = "+ " + buffAmount + " shuffle";
        else
            text.text = buffAmount + " shuffle";
    }

    protected override void SetBuff()
    {   
        do
        {
            buffAmount = RandomNumerBuff(-RangeRandom,RangeRandom);
        }while(buffAmount==0);
        GameManager.Instance.GetGift(buffAmount,0);
    }


}
