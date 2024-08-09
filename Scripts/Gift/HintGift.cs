using TMPro;
using UnityEngine;

public class HintGift : GiftBase
{
    [SerializeField] private int RangeRandom;
    private int buffAmount;
    protected override void SetFloatingText()
    {
        GameObject go = Instantiate(FloatingTextPrefab,transform.position,Quaternion.identity,transform);
        TextMeshPro text = go.GetComponent<TextMeshPro>();
        if(buffAmount>0)
            text.text = "+" + buffAmount + " hint";
        else
            text.text = buffAmount + " hint";
    }

    protected override void SetBuff()
    {
        do
        {
            buffAmount = RandomNumerBuff(-RangeRandom,RangeRandom);
        }while(buffAmount==0);
        GameManager.Instance.GetGift(buffAmount,2);
    }
}