using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public int index = 0;
    public bool isChoosed = false;
    public bool isMoved = true;
    public Text[] menuText;
    private Vector3[] menuTextFirstScale = new Vector3[3];
    private Vector3 btnsFirstPosition = new Vector3();
    public Transform btns, explainTexts;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < menuText.Length; i++)
        {
            menuTextFirstScale[i] =  menuText[i].transform.localScale;
        }
        btnsFirstPosition =  btns.transform.position;
        menuText[index].transform.localScale = menuTextFirstScale[index] + new Vector3(.5f, .5f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.pausePanel.gameObject)
        {

            if (!isChoosed)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && index <= 2 && index > 0)
                {
                    --index;
                    menuText[index].transform.localScale = menuTextFirstScale[index] + new Vector3(.5f, .5f, .5f);
                    menuText[index + 1].transform.localScale = menuTextFirstScale[index + 1];
                    if (menuText[index + 1].text == "ReallyExit")
                    {
                        menuText[index + 1].text = "Exit";
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && index < 2 && index >= 0)
                {
                    ++index;
                    menuText[index].transform.localScale = menuTextFirstScale[index] + new Vector3(.5f, .5f, .5f);
                    menuText[index - 1].transform.localScale = menuTextFirstScale[index - 1];

                }
            }

            if (index == 2 && Input.GetKeyDown(KeyCode.Space) && menuText[index].text == "ReallyExit")
            {
                Application.Quit();
                Debug.Log("나가기");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (index)
                {
                    case 0://이어하기
                        GameManager.Instance.PanelActive();
                        isChoosed = false;
                        break;
                    case 1://도움말
                        isMoved = !isMoved;
                        isChoosed = !isMoved;

                        Sequence seq = DOTween.Sequence();
                        seq.Append(btns.DOMoveX(isMoved ? btnsFirstPosition.x : btnsFirstPosition.x - 1, .5f));
                        seq.Join(btns.GetComponent<CanvasGroup>().DOFade(isMoved ? 1 : 0, 1));

                        seq.Join(explainTexts.GetComponent<Text>().DOFade(isMoved ? 0 : 1, 1));
                        seq.Join(explainTexts.DOMoveX(isMoved ? btnsFirstPosition.x : btnsFirstPosition.x + 1, .5f));
                        break;
                    case 2://나가기
                        menuText[index].text = "ReallyExit";
                        isChoosed = false;
                        break;
                }
            }
        }
    }
}
