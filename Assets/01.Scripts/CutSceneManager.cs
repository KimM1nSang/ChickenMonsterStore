using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public Image fadePanel;
    public Image[] cutScenes;
    public int index = 0;

    private void Start()
    {
        index = 0;
        GameManager.Instance.timeManager.dayTimeScale = 0;
        if (SaveGame.Instance.data.isCutSceneWatched)
        {
            SceneActiveFalse();
        }
        //Debug.Log(SaveGame.Instance.data.isCutSceneWatched);
        DOTween.To(() => fadePanel.color, a => fadePanel.color = a, new Color(0, 0, 0, 0), 3).OnComplete(() => {
            if (!SaveGame.Instance.data.isCutSceneWatched)
            {
                CutScene(2,2);
            }
            else
            {
                GameManager.Instance.timeManager.dayTimeScale = 1;
            }
        });
    }
    public void CutScene(float waitTime,float fadeTime)
    {
        cutScenes[index].DOFade(1, waitTime).OnComplete(() => {
            cutScenes[index].DOFade(0, fadeTime).OnComplete(() => {
                if (index == cutScenes.Length -1)
                {
                    GameManager.Instance.timeManager.dayTimeScale = 1;
                    SaveGame.Instance.data.isCutSceneWatched = true;
                    SceneActiveFalse();
                    Debug.Log("aaaaa");
                }
                else
                {
                    ++index;
                    CutScene(waitTime, fadeTime);
                }
            });
        });
            
        
    }
    private void SceneActiveFalse()
    {
        foreach (var item in cutScenes)
        {
            item.gameObject.SetActive(false);
        }
    }
}
