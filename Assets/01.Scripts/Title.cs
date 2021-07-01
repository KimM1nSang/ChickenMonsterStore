using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public int index = 0;
    [SerializeField] private Text startText = null;
    public string nextScene = "";
    public string startTextInfo = "용사인 나는, 치킨집을 차려서 50일을 버틴다";
    [SerializeField] private Text pressAnyKey;

    public bool isLoadGame = false;

    public bool isChoosed = false;
    public bool isMoved = true;
    public bool isTextPlay = false;

    public Text[] titleText;
    private Vector3[] titleTextFirstPosition = new Vector3[4];
    public Transform btns;
    public Vector3 btnsFirstPosition = new Vector3();
    // Update is called once per frame
    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            titleTextFirstPosition[i] = titleText[i].transform.position;
        }
        index = 0;
        titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(100, 0, 0);
        btnsFirstPosition = btns.position;
        if (!SaveGame.Instance.data.IsHaveData)
        {
            titleText[0].color -= new Color(.4f, .4f, .4f, 0);
        }
    }

    void Update()
    {
        if (!isChoosed)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && index <= 3 && index > 0)
            {
                --index;
                titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(150, 0, 0);
                titleText[index + 1].transform.position = titleTextFirstPosition[index + 1];
                if (titleText[index + 1].text == "ReallyExit")
                {
                    titleText[index + 1].text = "Exit";
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && index < 3 && index >= 0)
            {
                ++index;
                titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(150, 0, 0);
                titleText[index - 1].transform.position = titleTextFirstPosition[index - 1];

            }
        }
        

        if (index == 2 &&Input.GetKeyDown(KeyCode.Space)&&titleText[index].text == "ReallyExit")
        {
            Application.Quit();
            Debug.Log("나가기");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isChoosed = true;
            switch (index)
            {
                case 0://이어하기
                    if(SaveGame.Instance.data.IsHaveData)
                    {
                        ReadyToGameStart();
                    }
                    else
                    {
                        isChoosed = false;
                    }
                    break;
                case 1://뉴게임
                    SaveGame.Instance.NewGameData();
                    SaveGame.Instance.SaveGameData();
                    ReadyToGameStart();
                    break;
                case 2://도움말
                    isChoosed = false;
                    float pos = isMoved ? btnsFirstPosition.x - 1100 : btnsFirstPosition.x;
                    btns.DOMoveX(pos, 1).OnComplete(()=> {
                        isMoved = !isMoved;
                        isChoosed = !isMoved;
                    });
                    break;
                case 3://나가기
                    titleText[index].text = "ReallyExit";
                    isChoosed = false;
                    break;
            }
        }
/*        if(Input.GetKeyDown(KeyCode.Space) && isMoved && index == 1)
        {
            btns.DOMoveX(btnsFirstPosition.x, 1).OnComplete(()=> {
                isMoved = false;
                });
        }*/
        if (Input.anyKeyDown)
        {
            if (isLoadGame)
            {
                SceneManager.LoadScene(nextScene);
            }
            if (isTextPlay)
            {
                DOTween.Kill("text");
                startText.text = startTextInfo;
                PressAnyKeyOn();
            }
        }
    }
    private void PressAnyKeyOn()
    {
        isLoadGame = true;

        pressAnyKey.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
    }
    private void ReadyToGameStart()
    {
        btns.DOMoveX(btnsFirstPosition.x + 1000, 1).OnComplete(() => {
            isTextPlay = true;
            startText.DOText(startTextInfo, 8).SetId("text").OnComplete(() => {
                PressAnyKeyOn();
            });
        });
    }
}
