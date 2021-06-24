using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public string nextScene = "";
    [SerializeField] private Text text = null;
    [SerializeField] private Transform pressAnyKey = null;
    public bool canLoadScene = false;
    void Start()
    {
        text.DOText("용사인 나는, 치킨집을 차려서 50일을 버틴다",10).OnComplete(()=> {
            canLoadScene = true;
            pressAnyKey.gameObject.SetActive(true);
        });
    }
    void Update()
    {
        if(Input.anyKeyDown && canLoadScene)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
