using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set;}
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime;
    private void Awake() {
        Instance = this;
    }

    public void ChangeHomeScene()
    {
        animator.SetTrigger("EndTransition");
        Invoke(nameof(LoadHomeScene),animationTime);
    }
    private void LoadHomeScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeGameScene()
    {
        animator.SetTrigger("EndTransition");
        Invoke(nameof(LoadGameScene),animationTime);
    }
    private void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
