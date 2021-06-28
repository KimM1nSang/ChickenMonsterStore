using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

  

    [Header("ī�޶�")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("�ð�")]
    public TimeManager timeManager = null;
    private int dayCheck = 0;
    private const int endDay = 50;
    public Text dayText = null;

    [Header("�ֹ�")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState//�ֹ��� �ޱ����� ����
    {
        BETWEENORDER,//�ֹ��� ���� ���ϴ� ����,ġŲ�� �������� �� �ϴ� ����
        ORDER,//�ֹ��� ���� �� �ִ� ����, ġŲ�� �������� �� �ϴ� ����
        ORDERED//�ֹ��� ���� ����, ġŲ�� ������ �� �ִ� ����
    }
    [SerializeField] private Text orderText = null;//�ֹ��� �˷��ִ� UI
    private int orderAmount = 0;
    [Header("�մ�")]
    public Queue<Customer> customers = new Queue<Customer>();//�մ��� �����ϴ� ť
    public Image attckPanel = null;
    //private int index = 0;//�մ��� ����
    [SerializeField] private Transform customerPrefab;//�մ� ������
    [SerializeField] private Transform customerFirstPosition;//�մ��� ���� ��ġ
    public Sprite[] customerSprites;
    [Header("�γ���")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;//�մ��� �γ����� �˷��� UI
    public float waitingTime = 0;//�γ���
    public float waitingTimeLimit = 1000;//�γ��� �ʱⰪ
    public float downWaitingTime =0;//�γ��� ����ġ

    [Header("ġŲ")]
    private float FriedCurValue = 0;//�󸶳� Ƣ�����°�
    [SerializeField] private Slider FriedprogressBar = null;//�󸶳� Ƣ��°��� �˷��� UI
    public List<Chicken> madeChickens = new List<Chicken>();//Ƣ�� ġŲ�� ������ ����Ʈ
    public Text rawChicken = null;
    public Text friedChicken = null;

    [Header("�⸧�µ�")]
    [SerializeField] private Slider OilprogressBar;//�⸧�� �µ��� �˷��� UI
    public float oilTemp = 0;//�⸧�� �µ�
    private const float oilLimit = 1000;//�⸧�µ� ����

    //ġŲ�� �ϼ��� ���� �⸧�µ��� �ִ�,�ּҰ�
    private float minOilTemp = 500;
    private float maxOilTemp = 700;

    //�ʴ� �󸶳� �µ��� �������°�
    private float downOilTemp = 100;

    public Text friedPowder = null;

    public Image niceTempImage = null;

    [Header("�Ѿ�")]
    public int bullet = 0;//�Ѿ� ���
    private const int bulletLimit = 3;//�Ѿ˰����� �Ѱ�
    public int shootNum = 0;
    public Image[] BulletImage = null;//�Ѿ��� ������ �˷��� UI
    [Header("���ӿ���")]
    public Image gameOverPanel = null;//���ӿ��� �ǳ�
    [Header("����")]
    public Transform shopPanel = null;
    public Text[] moneyText = null;
    public bool isPanelOn = false;
    [Header("����")]
    public Text reputationText = null;
    [Header("����")]
    public Transform endingPopup = null;
    private bool isEnding = false;
    private void Awake()
    {
        Instance = this;
        GameReset();//�������ڸ��� �Լ� ����
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(customers.Count);
        //Debug.Log(canCreate);
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            RefreshText();
        }*/
            //Debug.Log(timeManager.time);
  

        if(!isEnding)
        {
            timeManager.SubtractTime(Time.deltaTime);

            if (timeManager.time <= 0)
            {
                if (timeManager.IsDayTime)//���̸� �� ���̸� ������
                {
                    if (timeManager.day == endDay)
                    {
                        Ending();
                    }
                    else
                    {
                        timeManager.SetDayTime(false);
                        foreach (var item in customers)
                        {
                            //Destroy(item.gameObject);
                            if (item.index != 0)
                            {
                                item.ExitTheStore();
                            }
                        }
                    }
                }
                else
                {
                    customers.Clear();
                    OrderTextReset();
                    shopPanel.gameObject.transform.DOMove(new Vector3(0, -10, 0), .5f).OnComplete(() =>
                    {
                        isPanelOn = false;
                    });
                    timeManager.SetDayTime(true);

                }
                timeManager.ResetTime();
                ResetWorkHistory();
                GameReset();
                dayCheck++;
                if (dayCheck >= 2)
                {
                    dayCheck = 0;
                    timeManager.DayPlus();
                    RefreshText();
                }

            }


            if (timeManager.IsDayTime)
            {
                if (orderState == OrderState.ORDERED)//�γ��� ����
                    ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
                if (waitingTime <= 0)
                {
                    //�մ��� ȭ��
                    //Debug.Log("ANGRY");

                    orderText.DOText("���� �ִ°ž�!!!!!", 2f).SetId("Talk").OnComplete(() => {
                        StartCoroutine(CustomerAttack());
                    });
                }


                //�մ� ����
                SpawningCustomer();

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    if (!isPanelOn)
                    {
                        shopPanel.gameObject.transform.DOMove(Vector3.zero, .5f).OnComplete(() => {
                            isPanelOn = true;
                        });
                    }
                    else
                    {
                        shopPanel.gameObject.transform.DOMove(new Vector3(0, -10, 0), .5f).OnComplete(() =>
                        {
                            isPanelOn = false;
                        });
                    }

                }
            }


            //�⸧ �µ� ����
            ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);


            //Ƣ�� ���൵ ����
            //ProgressBarDown(ref FriedCurValue, 100, 1 , ref FriedprogressBar);
        }
        else
        {
            timeManager.dayTimeScale = 0;
        }
    }
    public void CreateCustomer()//�մ� ���� �Լ�
    {
        if (customers != null && customers.Count >= 2 && customers.Last().index == 2) 
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position);//Customer Ŭ������ �����Լ� ȣ��

    }

    public void OrderTextReset()//�ֹ� �ؽ�Ʈ ����(�����)
    {
        orderText.text = "";
    }
    public void CameraShaking(float force)//�־��� ������ ī�޶� ���� �Լ�
    {
        ImpulseSource.GenerateImpulse(force);
    }
    private void ProgressBarDown(ref float mainNum, float limitNum, float downNum, ref Slider progressBar)//���α׷��� �� ���� �Լ�
    {
        if (mainNum >= 0)
        {
            mainNum -= downNum * Time.deltaTime;
        }
        progressBar.value = mainNum / limitNum;
    }
    public void MenuOrder()
    {
        if (orderState != OrderState.ORDER) return;//������Ʈ�� ORDER �� �ƴϸ� ����
        orderState = OrderState.BETWEENORDER;

        orderAmount = RandomAmount(3);//�ֹ� ���� ����
        waitingTime = customers.Peek().WaitingTime;//�մԿ� ���� �γ��� ����
        downWaitingTime = customers.Peek().DownWaitingTime;//�մԿ� ���� �γ��� ����ġ ����
        OrderTextReset();//�ؽ�Ʈ ����

        string chickenOrderTxt ="";
        switch (customers.Peek().customerType)
        {
            case Customer.CustomerType.NORMAL:
                chickenOrderTxt = $"ġŲ {orderAmount}���� �ּ���";
                break;
            case Customer.CustomerType.LATTE:
                chickenOrderTxt = $"�� ġŲ {orderAmount}���� �� ���";
                break;
            case Customer.CustomerType.ROBBER:
                chickenOrderTxt = "���� ġŲ �������� ����� ġŲ�� ������ �Ծ��.";
                break;
        }
        orderText.DOText(chickenOrderTxt, 3f).SetId("Talk").OnComplete(() => {
            switch (customers.Peek().customerType)
            {
                case Customer.CustomerType.NORMAL:
                    break;
                case Customer.CustomerType.LATTE:
                    OrderTextReset();
                    Sequence seq = DOTween.Sequence();
                    seq.Join(
                    orderText.DOText("�׷��� ���̾�", 1f).OnComplete(() => {
                        OrderTextReset();
                        orderText.DOText("��", .1f).OnComplete(() => {
                            OrderTextReset();
                            orderText.DOText("ġŲ�̶��� �� �ű���", 1.5f).OnComplete(() => {
                                OrderTextReset();
                                orderText.DOText("������ �̷� ��... ���� ũ��Ű?", 1.5f).OnComplete(() => {
                                OrderTextReset();
                                    orderText.DOText("�׷��� �����ŵ�", 1.5f).OnComplete(() => {
                                            OrderTextReset();
                                        orderText.DOText("���� �� �ľ��ٰ� �����ϴ°ž�?", 1.5f).OnComplete(() => {
                                            OrderTextReset();
                                            orderText.DOText("��,�� ���̰� ���", 1.5f).OnComplete(() => {
                                                OrderTextReset();
                                                orderText.DOText("�ϰ� �ι߷� �ȱ� �����Ҷ� ���̾�", 1.5f).OnComplete(() => {
                                                    OrderTextReset();
                                                    orderText.DOText("��?", 1.5f).OnComplete(() => {

                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    })
                    );
                    seq.SetId("Talk");
                    break;
                case Customer.CustomerType.ROBBER:
                    break;
            }
            orderState = OrderState.ORDERED;//���� ��ȯ(ġŲ�� ���� �� �� �ְ�)
        });

    }
    public int RandomAmount(int maxAmount)
    {
        if (Random.Range(0, 99) == 0)
        {
            return Random.Range(10, 15);
        }
        else
        {
            return  Random.Range(1, maxAmount);
        }
    }
    public void DisposeChicken()
    {
        if (orderState != OrderState.ORDERED) return;//������Ʈ�� ORDERED �� �ƴϸ� ����
        orderState = OrderState.BETWEENORDER;

        DOTween.KillAll();
        OrderTextReset();
        if (madeChickens.Count == orderAmount && IsGoodChickenInHere())
        {
            orderText.DOText("���� �� ���� �� ġŲ�� ��!", 2f).SetId("Talk").OnComplete(() => {
                EndDispose();

            });
            for (int i = 0; i < orderAmount; i++)
            {
                SaveGame.Instance.data.money += madeChickens[i].currentPrice *2;
            }
            SaveGame.Instance.data.reputation += 100;
        }
        else if (madeChickens.Count <= 0)
        {
            orderText.DOText("�� ��� �ϴ°ų�!!!!!!", 2f).SetId("Talk").OnComplete(() => {
                ReadyToAttack();
            });
            SaveGame.Instance.data.reputation -= 70;
        }
        else if (madeChickens.Count != orderAmount)
        {
            orderText.DOText("������ Ʋ���ݾ�!!!!!!!!!!!!!!", 2f).SetId("Talk").OnComplete(() => {
                ReadyToAttack();
            });

            SaveGame.Instance.data.reputation -= 20;
        }
        else
        {
            int isBad = 0;
            for (int i = 0; i < madeChickens.Count; i++)
            {
                if (madeChickens[i].IsGood == false)
                {
                    isBad++;
                }
            }
            if (isBad > 0)
            {
                orderText.DOText("Ƣ����°� �̰� ����!!!!!!!!!!!!.", 2f).SetId("Talk").OnComplete(() => {
                    ReadyToAttack();
                });
                for (int i = 0; i < orderAmount; i++)
                {
                    SaveGame.Instance.data.money += madeChickens[i].currentPrice /2;
                }
                SaveGame.Instance.data.reputation -= 30;
            }
        }
        RefreshText();
        madeChickens.Clear();
        waitingTime = waitingTimeLimit;
        orderAmount = 0;
    }
    public void ReadyToAttack()
    {
        StartCoroutine(CustomerAttack());
        OrderTextReset();
    }
    public bool IsGoodChickenInHere()
    {
        int index =0;
        foreach (var item in madeChickens)
        {
            if(!item.IsGood)
            {
                index++;
            }
        }
        return index == 0;
    }
    public void EndDispose()//�մ� ���� �� ���� ���� ����(�ֹ��� ���� �� �ִ� ������ ���·� ����)
    {
        OrderTextReset();
        customers.Peek().ExitTheStore();//����
        orderState = OrderState.ORDER;//���� ����
    }
    public IEnumerator CustomerAttack()
    {
        customers.Peek().isAngry = true;
        orderState = OrderState.ORDER;//���� ����
        yield return new WaitForSeconds(0.5f);
        attckPanel.gameObject.SetActive(true);
        attckPanel.gameObject.SetActive(customers.Peek().isAngry);

        yield return new WaitForSeconds(1f);
        if(customers.Peek().isAngry)
        {
            GameOver();
        }
        attckPanel.gameObject.SetActive(false);

    }
    public void PullCustomer()//��ü ��ġ �̵�
    {
        foreach (var item in customers)
        {
            if(item.index !=0)
            {
                item.index--;//�ε��� ����
                item.MovePosition();//�̵�
            }
        }
        customers.Dequeue();//�մ� ����
    }

    public void ResetWorkHistory()//���� ��ô�� ����
    {
        FriedCurValue = 0;
        FriedprogressBar.value = 0;
    }

    private bool isFriedNow = false;

    public void FriedChicken()
    {
        float workingPower = 25;//�󸶳� �����ϴ���

        //FriedCurValue += Time.deltaTime;
        if(SaveGame.Instance.data.chickens.Count >0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FriedCurValue += workingPower;//Ƣ�� ���� ����
                SoundManager.Instance.Fried.Play();//���� ���
                if (FriedCurValue >= 100 && !isFriedNow)//�� Ƣ������ ����
                {
                    isFriedNow = true;
                    //Debug.Log(SaveGame.Instance.data.chickens.Peek().currentPrice);
                    SaveGame.Instance.data.chickens[0].IsGood = IsRightOilTemp();//ġŲ�� ���� üũ
                    madeChickens.Add(SaveGame.Instance.data.chickens[0]);//���� ġŲ�� �߰�
                    SaveGame.Instance.data.chickens.RemoveAt(0);
                    //Debug.Log(madeChickens[madeChickens.Count -1].currentPrice);
                    FriedprogressBar.value = 1;
                    FriedprogressBar.DOValue(0, 0.3f).OnComplete(() => {
                        FriedCurValue = 0;//�󸶳� Ƣ������� ����
                        isFriedNow = false;
                    });
                    RefreshText();
                }
            }
        }
        else
        {
            //���� ���� ���
            Debug.Log("���� ����");
        }
        if(!isFriedNow)
        {
            FriedprogressBar.value = FriedCurValue / 100;//�󸶳� Ƣ������� ǥ��
        }
    }

    private bool IsRightOilTemp()//ġŲ�� �ϼ����� ����
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;//�⸧�� �µ��� �ϼ��� ���� �ּڰ�,�ִ밪�� ���߾�����(�� ���� ���� ����)
    }

    public void OilTempUp()//�⸧�� �µ��� �ö󰡰����ִ� �Լ�
    {
        float workingPower = 20;

        if (Input.GetKeyDown(KeyCode.Space) && SaveGame.Instance.data.friedPowders.Count >=1)
        {
            if (oilTemp < oilLimit)
            {
                oilTemp += workingPower * (SaveGame.Instance.data.friedPowders[0].rank +1);
                SoundManager.Instance.Oil.Play();
            }
            RefreshText();
            SaveGame.Instance.data.friedPowders.RemoveAt(0);
        }

    }
    public void RefreshText() //���� ��ǥ��
    {
        friedPowder.text = $"{SaveGame.Instance.data.friedPowders.Count}";
        rawChicken.text = $"{SaveGame.Instance.data.chickens.Count}";
        friedChicken.text = $"{madeChickens.Count}";
        dayText.text = $"{timeManager.day} - Day";
        foreach (var item in moneyText)
        {
            item.text = string.Format("{0:#,###}", SaveGame.Instance.data.money.ToString());
        }
        minOilTemp = 500 - 40 * SaveGame.Instance.data.oil.rank;
        float min =  40 * SaveGame.Instance.data.oil.rank;
        niceTempImage.GetComponent<RectTransform>().offsetMin = new Vector2(niceTempImage.GetComponent<RectTransform>().offsetMin.x, -min);
        reputationText.text = SaveGame.Instance.data.reputation.ToString();
    }
    private void SpawningCustomer()//�մ��� �����ϴ� �Լ�
    {
        if (orderState == OrderState.ORDERED || timeManager.IsDayTime == false || isEnding) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameReset()//���� ���� ���ִ� �Լ�
    {
        RefreshText();
        //oilTemp = oilLimit;
        if (customers.Count >= 1)
        {

        }
        else
        {
            orderState = OrderState.ORDER;
            waitingTime = waitingTimeLimit;
            OrderTextReset();

        }
        bullet = bulletLimit;
        foreach (var item in BulletImage)
        {
            item.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
        }
        gameOverPanel.gameObject.SetActive(false);
    }
    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        attckPanel.gameObject.SetActive(false);
        gameOverPanel.transform.GetChild(0).transform.DORotate(new Vector3(0,0,90),.8f).SetEase(Ease.InQuad).OnComplete(()=> {
            gameOverPanel.transform.GetChild(1).gameObject.GetComponent<Text>().DOFade(1, 2f).SetEase(Ease.InQuad).OnComplete(()=> {
                gameOverPanel.transform.GetChild(2).gameObject.GetComponent<Text>().DOFade(1, 1).SetLoops(-1,LoopType.Yoyo);
                isEnding = true;
            });
        });
    }

    public void Ending()
    {
        isEnding = true;
        endingPopup.gameObject.SetActive(true);

    }
}
