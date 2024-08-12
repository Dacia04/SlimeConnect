using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ContactFilter2D contactFilter2D;
    public Transform pos1;
    public Transform pos2;
    public float distance;
    public LayerMask nodeLayerMask;

    private void Start() {
        //TestRaycast();
        TestRayCastAll();
        //TestRaycastNonAlloc();
        //TestLinecast();
    }

    public void TestRaycast() // lấy hết trigger, collider    // lấy cái chạm đầu tiên
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position,Vector2.right);
        if(raycastHit2D)
        {
            Debug.Log("right hit " + raycastHit2D.collider.gameObject.name);
        }
    }

    public void TestRayCastAll()   // lấy hết trigger, collider
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(transform.position,Vector2.right,distance,nodeLayerMask);
        //RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(transform.position,Vector2.right,distance);
        foreach(var ob in raycastHit2Ds)
        {
            Debug.Log("right hit " + ob.collider.gameObject.name);
        }
    }

    public void TestRaycastNonAlloc() // chỉ lấy số trigger chỉ định còn lại null
    {
        RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[5];
        int countHits = Physics2D.RaycastNonAlloc(transform.position,Vector2.right,raycastHit2Ds);
        Debug.Log(countHits);
        foreach(var ob in raycastHit2Ds)
        {
            if(ob)
                Debug.Log("right hit " + ob.collider.gameObject.name);
        }
    }

    public void TestLinecast()
    {
        RaycastHit2D raycastHit2D = Physics2D.Linecast(pos1.position,pos2.position);
        if(raycastHit2D)
        {
            Debug.Log("line hit " + raycastHit2D.collider.gameObject.name);
        }
    }
}
