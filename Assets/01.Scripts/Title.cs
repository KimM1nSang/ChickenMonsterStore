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
    public string startTextInfo = "����� ����, ġŲ���� ������ 50���� ��ƾ��";
    [SerializeField] private Text pressAnyKey;
    public bool canLoadScene = false;

    public bool isChoosed = false;
    public bool isMoved = true;

    public Text[] titleText;
    private Vector3[] titleTextFirstPosition = new Vector3[3];
    public Transform btns;
    public Vector3 btnsFirstPosition = new Vector3();
    // Update is called once per frame
    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            titleTextFirstPosition[i] = titleText[i].transform.position;
        }
        index = 0;
        titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(100, 0, 0);
        btnsFirstPosition = btns.position;
    }

    void Update()
    {
        if(!isChoosed)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && index <= 2 && index > 0)
            {
                --index;
                titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(150, 0, 0);
                titleText[index + 1].transform.position = titleTextFirstPosition[index + 1];
                if (titleText[index + 1].text == "ReallyExit")
                {
                    titleText[index + 1].text = "Exit";
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && index < 2 && index >= 0)
            {
                ++index;
                titleText[index].transform.position = titleTextFirstPosition[index] - new Vector3(150, 0, 0);
                titleText[index - 1].transform.position = titleTextFirstPosition[index - 1];

            }
        }
        

        if (index == 2 &&Input.GetKeyDown(KeyCode.Space)&&titleText[index].text == "ReallyExit")
        {
            Application.Quit();
            Debug.Log("������");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isChoosed = true;
            switch (index)
            {
                case 0://��ŸƮ
                    btns.DOMoveX(btnsFirstPosition.x + 1000,1).OnComplete(()=> {
                        startText.DOText(startTextInfo, 8).OnComplete(() => {
                            canLoadScene = true;
                            pressAnyKey.DOFade(1,1).SetLoops(-1, LoopType.Yoyo);
                        });
                    });
                    break;
                case 1://����
                    float pos = isMoved ? btnsFirstPosition.x - 1100 : btnsFirstPosition.x;
                    btns.DOMoveX(pos, 1).OnComplete(()=> {
                        isMoved = !isMoved;
                        isChoosed = !isMoved;
                    });
                    break;
                case 2://������
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
        if (Input.anyKeyDown && canLoadScene)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
