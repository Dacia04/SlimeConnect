using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBase : MonoBehaviour, IClickable
{
    public GameObject FloatingTextPrefab;
    private float TimeDisappear = 5f;
    public float Speed;


    private void Update() {
        transform.Translate(Speed * Time.deltaTime * Vector2.down);
        CheckOutScene();
    }






    public virtual void OnMouseDown()
    {
        Speed =0f;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        SetBuff();
        SetFloatingText();
        Destroy(gameObject,TimeDisappear);
    }

    public virtual void OnMouseEnter()
    {
        
    }
    public virtual void OnMouseExit()
    {
        
    }

    protected virtual void SetFloatingText(){}
    protected virtual void SetBuff(){}

    /// <summary>
    /// value random is in [min,max]
    /// </summary>
    public int RandomNumerBuff(int min,int max)
    {
        System.Random rd= new();
        return rd.Next(min,max+1);
    }

    public void CheckOutScene()
    {
        if(Camera.main.transform.position.y - Camera.main.orthographicSize  >  transform.position.y)
        {
            Destroy(gameObject,2f);
        }
    }

}
