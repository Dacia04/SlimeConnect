using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private void OnEnable() {
        Invoke(nameof(SetDisappearAnimation),2f);
    }


    private void SetDisappearAnimation()
    {
        gameObject.GetComponent<Animator>().SetBool("IsDisappear",true);
    }
}