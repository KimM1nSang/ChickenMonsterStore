using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Net.NetworkInformation;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

  

    [Header("ī�޶�")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("�ð�")]
    public TimeManager timeManager = null;
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
    public Text orderText = null;//�ֹ��� �˷��ִ� UI
    public int orderAmount = 0;
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
    public Image[] BulletImage = null;//�Ѿ��� ������ �˷��� UI
    [Header("���ӿ���")]
    public Image gameOverPanel = null;//���ӿ��� �ǳ�
    [Header("����")]
    public Transform shopPanel = null;
    public Text[] moneyText = null;
    public bool isPanelOn = false;
    public Vector3 panelDownPos = new Vector3(0, -10, 0);
    [Header("����")]
    public Text reputationText = null;
    [Header("����")]
    public Transform endingPopup = null;
    public string endingStartStr;
    public Transform afterCome = null;
    public Text endingText = null;
    private int endingIndex = 0;
    public Image endingSprite;
    private bool isEnding = false;
    [Header("�̺�Ʈ")]
    public Sprite[] eventCustomerImage = null;
    public int eventNum = 0;
    private bool isEventSpawn = false;
    private bool isEvent = false;
    public TalkManager talkManager = null;
    [Header("�Ͻ�����")]
    public Transform pausePanel;

    public SpriteManager spriteManager;
    private void Awake()
    {
        Instance = this;
        GameReset();//�������ڸ��� �Լ� ����
        SaveGame.Instance.data.IsHaveData = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEnding && timeManager.dayTimeScale != 0)
        {
            timeManager.SubtractTime(Time.deltaTime);

            if (timeManager.time <= 0)
            {
                if (timeManager.IsDayTime)//���̸� �� ���̸� ������
                {
                    if (timeManager.day == endDay)
                    {
                        Ending();
                        foreach (var item in customers)
                        {
                            item.ExitTheStore(new Vector3(1,0,0));
                        }
                    }
                    else
                    {
                        timeManager.SetDayTime(false);
                        foreach (var item in customers)
                        {
                            //Destroy(item.gameObject);
                            if (item.index != 0)
                            {
                                item.ExitTheStore(new Vector3(0, -1, 0));
                            }
                        }
                    }
                }
                else
                {
                    TextReset(orderText);
                    shopPanel.gameObject.transform.DOMove(new Vector3(0, -10, 0), .5f).OnComplete(() =>
                    {
                        isPanelOn = false;
                    });
                    timeManager.SetDayTime(true);
                }
                timeManager.ResetTime();
                GameReset();
                ++timeManager.dayCheck;
                if (timeManager.dayCheck >= 2)
                {
                    timeManager.dayCheck = 0;
                    timeManager.DayPlus();
                    RefreshText();
                }

            }


            if (timeManager.IsDayTime)
            {
                if (timeManager.day % 10 == 0)
                {
                    if (timeManager.day != 50)
                    {
                        isEvent = true;
                        eventNum = (int)timeManager.day / 10;
                        //1.���ռ� ��� �ùٰ�
                        //2.���ռ� ������ �Ҹ���
                        //3.������ ���谡 ����(���ռ� ���������� ����ħ)
                        //4.���� ������
                        if (!isEventSpawn)
                        {
                            isEventSpawn = true;
                            CreateCustomer();
                        }
                    }
                }
                else
                {
                    isEvent = false;
                    //�մ� ����
                    SpawningCustomer();

                    if (orderState == OrderState.ORDERED)//�γ��� ����
                        ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
                    if (waitingTime <= 10)
                    {
                        //�մ��� ȭ��
                        //Debug.Log("ANGRY");

                        orderText.DOText("���� �ִ°ž�!!!!!", 2f).SetId("Talk").OnComplete(() => {
                            StartCoroutine(CustomerAttack());
                        });
                    }


                }

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    Vector3 panelPos = isPanelOn ? panelDownPos : Vector3.zero;
                    shopPanel.gameObject.transform.DOMove(panelPos, .5f).OnComplete(() =>
                    {
                        isPanelOn = !isPanelOn;
                    });
                }
            }


            //�⸧ �µ� ����
            ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                PanelActive();
            }
        }
        else
        {
            timeManager.dayTimeScale = 0;
        }
    }
    public void PanelActive()
    {
        pausePanel.gameObject.SetActive(!pausePanel.gameObject.activeSelf);
        timeManager.dayTimeScale = pausePanel.gameObject.activeSelf ? 0 : 1;
        Time.timeScale = pausePanel.gameObject.activeSelf ? 0 : 1;
    }
    public void CreateCustomer()//�մ� ���� �Լ�
    {
        if (customers != null && customers.Count >= 2 && customers.Last().index == 2) 
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position, isEvent);//Customer Ŭ������ �����Լ� ȣ��

    }

    public void TextReset(Text text)//�ֹ� �ؽ�Ʈ ����(�����)
    {
        text.text = "";
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

        customers.Peek().isAngry = false;
        waitingTime = customers.Peek().WaitingTime;//�մԿ� ���� �γ��� ����
        downWaitingTime = customers.Peek().DownWaitingTime;//�մԿ� ���� �γ��� ����ġ ����
        TextReset(orderText);//�ؽ�Ʈ ����

        if(customers.Peek().customerType == Customer.CustomerType.NORMAL)
        {
            orderAmount = RandomAmount(3);//�ֹ� ���� ����
            orderText.DOText($"ġŲ {orderAmount}���� �ּ���", 3f).SetId("Talk").OnComplete(() => {
                orderState = OrderState.ORDERED;//���� ��ȯ(ġŲ�� ���� �� �� �ְ�)
            });
        }
        else
        {
            orderAmount = 1;
            Talk(orderText,(int)customers.Peek().customerType, 0);
        }
    }

    private void Talk(Text text, int id,int talkIndex)
    {
        int customerType = (int)customers.Peek().customerType;

        string talkData = talkManager.GetTalk(id, talkIndex);
        if(isEnding)
        {
            EndingImage(endingSprite, id -10, talkIndex);
        }
        
        if (talkData != null)
        {
            TextReset(text);
            int leng;
            text.DOText(talkData, leng = talkData.Length >3 ? talkData.Length/5 : talkData.Length).SetId("Talk").OnComplete(() =>
            {
                Talk(text,id, ++talkIndex);
            });
        }
        else
        {
            if (customerType >= 3 && customerType <= 6 && orderState == OrderState.ORDERED)
            {
                EndDispose();
            }
        }
        if(!isEnding)
        {
            if (id > 2 && id < 7)
            {
                if (talkData == null)
                {
                    orderState = OrderState.ORDERED;
                }
            }
            else
            {
                orderState = OrderState.ORDERED;
            }
        }
    }

    private void EndingImage(Image image,int id,int imageIndex)
    {
        Sprite imageData = spriteManager.GetImage(id, imageIndex);
        if (imageData != null)
        {
            endingSprite.sprite = imageData;
        }
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

        int isBad = 0;
        for (int i = 0; i < madeChickens.Count; i++)
        {
            if (madeChickens[i].IsGood == false)
            {
                isBad++;
            }
        }

        DOTween.Kill("Talk");
        TextReset(orderText);
        int customerType = (int)customers.Peek().customerType;
        if (customerType == 2)
        {
            SaveGame.Instance.data.chickens.Clear();
            SaveGame.Instance.data.money -= 10;
            orderText.DOText("�� ��� ���ϴ� űű", 2f).SetId("Talk").OnComplete(() => {
                EndDispose();
                ClearCustomerQueue();
            });
        }
        else if(customerType >= 3 && customerType <= 6)
        {
            Talk(orderText, (int)customers.Peek().customerType + 100, 0);
        }
        else if (madeChickens.Count == orderAmount && IsGoodChickenInHere())
        {
            orderText.DOText("���� �� ���� �� ġŲ�� ��!", 2f).SetId("Talk").OnComplete(() => {
                EndDispose();
                ClearCustomerQueue();
            });
            AddMoney(3f);
            IncreaseReputation(100);
        }
        else if (madeChickens.Count <= 0)
        {
            ReadyToAttack(7, 0);
            
            DecreaseReputation(70);
        }
        else if (isBad > 0)
        {
            
            ReadyToAttack(9, 0);
            AddMoney(0.5f);
            DecreaseReputation(30);
        }
        else if (madeChickens.Count != orderAmount)
        {
            string text = talkManager.GetTalk(8, 0);
            orderText.DOText(text, 2f).SetId("Talk").OnComplete(() => {
                EndDispose();
                ClearCustomerQueue();
            });
            DecreaseReputation(20);
        }

        RefreshText();
        madeChickens.Clear();
        waitingTime = waitingTimeLimit;
        orderAmount = 0;
    }
    public void ClearCustomerQueue()
    {
        if (!timeManager.IsDayTime)
            customers.Clear();
    }
    public void AddMoney(float multiply)
    {
        for (int i = 0; i < orderAmount; i++)
        {
            SaveGame.Instance.data.money += (int)(madeChickens[i].currentPrice * multiply);
        }
    }
    public void DecreaseReputation(int decreaseNum)//���� ����
    {
        SaveGame.Instance.data.reputation -= decreaseNum;
    }
    public void IncreaseReputation(int increaseNum)//���� ����
    {
        SaveGame.Instance.data.reputation += increaseNum;
    }
    public void ReadyToAttack(int id,int talkIndex)//���� ���
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        orderText.DOText(talkData, 2f).SetId("Talk").OnComplete(() => {
            TextReset(orderText);
            StartCoroutine(CustomerAttack());
        });
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
        TextReset(orderText);
        customers.Peek().ExitTheStore(new Vector3(1, 0, 0));//����
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
        else
        {
            ClearCustomerQueue();
        }
        attckPanel.gameObject.SetActive(false);

    }

    public void PullCustomer()//��ü ��ġ �̵�
    {
        foreach (var item in customers)
        {

            item.index--;//�ε��� ����
            item.MovePosition();//�̵�
            if (item.index <0)
            {
                Destroy(item.gameObject);
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
                    CameraShaking(1f);
                    SoundManager.Instance.FriedComplete.Play(); 
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
                CameraShaking(0.5f);
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
            TextReset(orderText);
        }
        bullet = bulletLimit;
        foreach (var item in BulletImage)
        {
            item.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
        }
        shopPanel.transform.position = panelDownPos;
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

    // 1 ������ ���� ���� ���� �߻� ���� ������ �������� �ʿ��� ���ռ� ������ ���� �ȴ�.
    // 2 ������ ���� ���� ���� �߻� ���� ������ ���������� �ݵ����� ���� ����ģ��.
    //
    // 3 ������ ���� ���� ���� �߻� ���� ������ ���ο� ������ �ȴ�.
    // 4 ������ ���� ���� ���� �߻� ���� ������ �������� ġŲ�� ���������� ��縦 �Ѵ�.
    //
    // 5 ������ ���� ���� ���� �߻� ���� ������ �������� ���� ���ռ��� ��� �Ѵ�.
    // 6 ������ ���� ���� ���� �߻� ���� ������ �������� ������ �����Ͽ� ���ռ��� ���ʶ߸���.
    //
    // 7 ������ ���� ���� ���� �߻� ���� ������ �ų信 ���� ������ ����ϰ� �۷��� ����.
    // 8 ������ ���� ���� ���� �߻� ���� ������ ���� ����� ���������� �����Ǿ� �ձ����� ��ȯ
    //
    // 9 ������ ���� ���� ���� �߻� ���� 0�̸� �������� ��� ġŲ���� ����Ѵ�.(�� �ϻ��� ����������!!!!!!!)
    public void Ending()
    {
        isEnding = true;
        timeManager.dayTimeScale = 0;
        endingPopup.gameObject.SetActive(true);
        endingPopup.GetChild(0).GetComponent<Text>().DOText(endingStartStr, endingStartStr.Length/4).OnComplete(()=>{
            //���� �ε��� �����ֱ�
            endingIndex = 1;
            var data = SaveGame.Instance.data;

            bool isRich = data.money >= 300;
            bool isPoor = data.money <= 100;

            bool isFamous = data.reputation < 300;
            bool isUnknown = data.reputation > 1000;

            bool isPeacefull = data.shootNum < 1;
            bool isWild = data.shootNum >=1 && data.shootNum < 30;
            bool isVeryWild = data.shootNum >= 30;
            bool isVeryVeryWild = data.shootNum >= 140;

            bool firstEnding = isFamous && isPoor  && isVeryWild;
            bool secondEnding = isFamous && isPoor && isWild;

            bool thirdEnding = isFamous && isRich && isVeryWild;
            bool fourthEnding = isFamous && isRich && isWild;

            bool fifthEnding = isUnknown && isRich && isVeryWild;
            bool sixthEnding = isUnknown && isRich && isWild;

            bool seventhEnding = isUnknown && isPoor && isVeryWild;
            bool eightEnding = isUnknown && isPoor && isWild;

            bool ninthEnding = isFamous && isRich && isPeacefull;

            if (firstEnding)
            {
                endingIndex = 0;
            }
            else if (secondEnding)
            {
                endingIndex = 1;
            }
            else if (thirdEnding)
            {
                endingIndex = 2;

            }
            else if (fourthEnding)
            {
                endingIndex = 3;

            }
            else if (fifthEnding)
            {
                endingIndex = 4;

            }
            else if (sixthEnding)
            {
                endingIndex = 5;

            }
            else if (seventhEnding)
            {
                endingIndex = 6;

            }
            else if (eightEnding)
            {
                endingIndex = 7;

            }
            else if (ninthEnding)
            {
                endingIndex = 8;
            }
            //���� �ε����� ���缭 ���� ����,�ؽ�Ʈ�����ֱ�
            //endingSprite.sprite = endingSprites[];
            afterCome.gameObject.SetActive(true);
            Talk(endingText, endingIndex +10, 0);
        });
        

    }
    
}
