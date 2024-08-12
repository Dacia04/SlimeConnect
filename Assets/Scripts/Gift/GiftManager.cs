using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftManager : MonoBehaviour
{
    public float TimeCreateGift;
    public LevelSO LevelSO;
    public float OffsetY;

    private void Start() {
        InvokeRepeating(nameof(CreateGift),TimeCreateGift,TimeCreateGift);
    }

    private void CreateGift()
    {
        int randomGift = Random.Range(0, LevelSO.GiftList.Count-1);
        float orthographicSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;
        float viewportWidth = orthographicSize * 2 * aspectRatio;
        Vector2 posCreat = new Vector3(Camera.main.transform.position.x + Random.Range(-viewportWidth/2,viewportWidth/2),
                    Camera.main.transform.position.y+ OffsetY,Camera.main.transform.position.z);
        Instantiate(LevelSO.GiftList[randomGift],posCreat,Quaternion.identity,transform);
    }
}
