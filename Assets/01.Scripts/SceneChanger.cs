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
        text.DOText("����� ����, ġŲ���� ������ 50���� ��ƾ��",10).OnComplete(()=> {
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
