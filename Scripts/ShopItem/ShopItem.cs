using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField]private int Price;
    [SerializeField] private string NamePrice;
    [SerializeField]private int Item;
    [SerializeField]private string NameItem;

    private TextMeshProUGUI textPrice;

    private void Start() {
        textPrice= gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textPrice.text = $"{Price} {NamePrice}s";
    }


    //getter
    public int GetPrice()
    {
        return Price;
    }
    public int GetItem()
    {
        return Item;
    }
    public string GetNameItem()
    {
        return NameItem;
    }
    public string GetNamePrice()
    {
        return NamePrice;
    }
}
