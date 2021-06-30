using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public Image fadePanel;
    public Image[] cutScenes;
    public int index = 1;
    private void Awake()
    {
        if (SaveGame.Instance.data.isCutSceneWatched)
        {
            SceneActiveFalse();
        }

        DOTween.To(() => fadePanel.color, a => fadePanel.color = a, new Color(0, 0, 0, 0), 1).OnComplete(() => {
            if (SaveGame.Instance.data.isCutSceneWatched)
            {
                StartCoroutine(CutScene(2));
            }
        });
        
    }
    private void Start()
    {
        GameManager.Instance.timeManager.dayTimeScale = 0;
    }
    public IEnumerator CutScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cutScenes[index].DOFade(0, waitTime);
        if (index < cutScenes.Length)
        {
            ++index;
            StartCoroutine(CutScene(waitTime));
        }
        else
        {
            SaveGame.Instance.data.isCutSceneWatched = true;
            SceneActiveFalse();
        }
    }
    private void SceneActiveFalse()
    {
        foreach (var item in cutScenes)
        {
            item.gameObject.SetActive(false);
        }
    }
}
