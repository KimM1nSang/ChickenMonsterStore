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

  

    [Header("카메라")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("시간")]
    public TimeManager timeManager = null;
    private const int endDay = 50;
    public Text dayText = null;

    [Header("주문")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState//주문을 받기위한 상태
    {
        BETWEENORDER,//주문을 받지 못하는 상태,치킨을 제공하지 못 하는 상태
        ORDER,//주문을 받을 수 있는 상태, 치킨을 제공하지 못 하는 상태
        ORDERED//주문을 받은 상태, 치킨을 제공할 수 있는 상태
    }
    public Text orderText = null;//주문을 알려주는 UI
    public int orderAmount = 0;
    [Header("손님")]
    public Queue<Customer> customers = new Queue<Customer>();//손님을 저장하는 큐
    public Image attckPanel = null;
    //private int index = 0;//손님의 순서
    [SerializeField] private Transform customerPrefab;//손님 프리팹
    [SerializeField] private Transform customerFirstPosition;//손님의 생성 위치
    public Sprite[] customerSprites;
    [Header("인내심")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;//손님의 인내심을 알려줄 UI
    public float waitingTime = 0;//인내심
    public float waitingTimeLimit = 1000;//인내심 초기값
    public float downWaitingTime =0;//인내심 감소치

    [Header("치킨")]
    private float FriedCurValue = 0;//얼마나 튀겨졌는가
    [SerializeField] private Slider FriedprogressBar = null;//얼마나 튀겼는가를 알려줄 UI
    public List<Chicken> madeChickens = new List<Chicken>();//튀긴 치킨을 저장할 리스트
    public Text rawChicken = null;
    public Text friedChicken = null;

    [Header("기름온도")]
    [SerializeField] private Slider OilprogressBar;//기름의 온도를 알려줄 UI
    public float oilTemp = 0;//기름의 온도
    private const float oilLimit = 1000;//기름온도 제한

    //치킨의 완성을 위한 기름온도의 최대,최소값
    private float minOilTemp = 500;
    private float maxOilTemp = 700;

    //초당 얼마나 온도가 떨어지는가
    private float downOilTemp = 100;

    public Text friedPowder = null;

    public Image niceTempImage = null;

    [Header("총알")]
    public int bullet = 0;//총알 곗수
    private const int bulletLimit = 3;//총알갯수의 한계
    public Image[] BulletImage = null;//총알의 갯수를 알려줄 UI
    [Header("게임오버")]
    public Image gameOverPanel = null;//게임오버 판넬
    [Header("상점")]
    public Transform shopPanel = null;
    public Text[] moneyText = null;
    public bool isPanelOn = false;
    public Vector3 panelDownPos = new Vector3(0, -10, 0);
    [Header("평판")]
    public Text reputationText = null;
    [Header("엔딩")]
    public Transform endingPopup = null;
    public string endingStartStr;
    public Transform afterCome = null;
    public Text endingText = null;
    private int endingIndex = 0;
    public Image endingSprite;
    private bool isEnding = false;
    [Header("이벤트")]
    public Sprite[] eventCustomerImage = null;
    public int eventNum = 0;
    private bool isEventSpawn = false;
    private bool isEvent = false;
    public TalkManager talkManager = null;
    [Header("일시정지")]
    public Transform pausePanel;

    public SpriteManager spriteManager;
    private void Awake()
    {
        Instance = this;
        GameReset();//시작하자마자 함수 실행
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
                if (timeManager.IsDayTime)//낮이면 밤 밤이면 낮으로
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
                        //1.마왕성 경비병 시바개
                        //2.마왕성 교도관 불만두
                        //3.길잃은 모험가 쥬쥬(마왕성 교도서에서 도망침)
                        //4.마왕 곰돌이
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
                    //손님 생성
                    SpawningCustomer();

                    if (orderState == OrderState.ORDERED)//인내심 감소
                        ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
                    if (waitingTime <= 10)
                    {
                        //손님이 화남
                        //Debug.Log("ANGRY");

                        orderText.DOText("언제 주는거야!!!!!", 2f).SetId("Talk").OnComplete(() => {
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


            //기름 온도 감소
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
    public void CreateCustomer()//손님 생성 함수
    {
        if (customers != null && customers.Count >= 2 && customers.Last().index == 2) 
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position, isEvent);//Customer 클래스의 생성함수 호출

    }

    public void TextReset(Text text)//주문 텍스트 리셋(지우기)
    {
        text.text = "";
    }
    public void CameraShaking(float force)//주어진 값으로 카메라 흔드는 함수
    {
        ImpulseSource.GenerateImpulse(force);
    }
    private void ProgressBarDown(ref float mainNum, float limitNum, float downNum, ref Slider progressBar)//프로그래스 바 감소 함수
    {
        if (mainNum >= 0)
        {
            mainNum -= downNum * Time.deltaTime;
        }
        progressBar.value = mainNum / limitNum;
    }
    public void MenuOrder()
    {
        if (orderState != OrderState.ORDER) return;//스테이트가 ORDER 가 아니면 리턴
        orderState = OrderState.BETWEENORDER;

        customers.Peek().isAngry = false;
        waitingTime = customers.Peek().WaitingTime;//손님에 따른 인내심 세팅
        downWaitingTime = customers.Peek().DownWaitingTime;//손님에 따른 인내심 감소치 세팅
        TextReset(orderText);//텍스트 리셋

        if(customers.Peek().customerType == Customer.CustomerType.NORMAL)
        {
            orderAmount = RandomAmount(3);//주문 갯수 랜덤
            orderText.DOText($"치킨 {orderAmount}마리 주세요", 3f).SetId("Talk").OnComplete(() => {
                orderState = OrderState.ORDERED;//상태 변환(치킨을 제공 할 수 있게)
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
        if (orderState != OrderState.ORDERED) return;//스테이트가 ORDERED 가 아니면 리턴
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
            orderText.DOText("잘 놀다 갑니다 킥킥", 2f).SetId("Talk").OnComplete(() => {
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
            orderText.DOText("잊을 수 없는 이 치킨의 맛!", 2f).SetId("Talk").OnComplete(() => {
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
    public void DecreaseReputation(int decreaseNum)//평판 감소
    {
        SaveGame.Instance.data.reputation -= decreaseNum;
    }
    public void IncreaseReputation(int increaseNum)//평판 증가
    {
        SaveGame.Instance.data.reputation += increaseNum;
    }
    public void ReadyToAttack(int id,int talkIndex)//공격 대사
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
    public void EndDispose()//손님 퇴장 및 상태 원상 복구(주문을 받을 수 있는 원래의 상태로 복구)
    {
        TextReset(orderText);
        customers.Peek().ExitTheStore(new Vector3(1, 0, 0));//퇴장
        orderState = OrderState.ORDER;//상태 복구
    }

    public IEnumerator CustomerAttack()
    {
        customers.Peek().isAngry = true;
        orderState = OrderState.ORDER;//상태 복구
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

    public void PullCustomer()//전체 위치 이동
    {
        foreach (var item in customers)
        {

            item.index--;//인덱스 감소
            item.MovePosition();//이동
            if (item.index <0)
            {
                Destroy(item.gameObject);
            }
        }
        customers.Dequeue();//손님 제거
    }

    public void ResetWorkHistory()//일의 진척도 리셋
    {
        FriedCurValue = 0;
        FriedprogressBar.value = 0;
    }

    private bool isFriedNow = false;

    public void FriedChicken()
    {
        float workingPower = 25;//얼마나 증가하는지

        //FriedCurValue += Time.deltaTime;
        if(SaveGame.Instance.data.chickens.Count >0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FriedCurValue += workingPower;//튀김 상태 증가
                SoundManager.Instance.Fried.Play();//음원 재생
                if (FriedCurValue >= 100 && !isFriedNow)//다 튀겼으면 리셋
                {
                    CameraShaking(1f);
                    SoundManager.Instance.FriedComplete.Play(); 
                    isFriedNow = true;
                    //Debug.Log(SaveGame.Instance.data.chickens.Peek().currentPrice);
                    SaveGame.Instance.data.chickens[0].IsGood = IsRightOilTemp();//치킨의 상태 체크
                    madeChickens.Add(SaveGame.Instance.data.chickens[0]);//만든 치킨에 추가
                    SaveGame.Instance.data.chickens.RemoveAt(0);
                    //Debug.Log(madeChickens[madeChickens.Count -1].currentPrice);
                    FriedprogressBar.value = 1;
                    FriedprogressBar.DOValue(0, 0.3f).OnComplete(() => {
                        FriedCurValue = 0;//얼마나 튀겼는지를 리셋
                        isFriedNow = false;
                    });
                    RefreshText();
                }
            }
        }
        else
        {
            //실패 음원 재생
            Debug.Log("생닭 부족");
        }
        if(!isFriedNow)
        {
            FriedprogressBar.value = FriedCurValue / 100;//얼마나 튀겼는지를 표기
        }
    }

    private bool IsRightOilTemp()//치킨의 완성도를 측정
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;//기름의 온도가 완성을 위한 최솟값,최대값을 맞추었는지(두 수의 사이 인지)
    }

    public void OilTempUp()//기름의 온도가 올라가게해주는 함수
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
    public void RefreshText() //정보 재표기
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
    private void SpawningCustomer()//손님을 생성하는 함수
    {
        if (orderState == OrderState.ORDERED || timeManager.IsDayTime == false || isEnding) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameReset()//값을 리셋 해주는 함수
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

    // 1 평판이 높고 돈이 낮고 발사 수가 많으면 지원군이 않오고 마왕성 감옥에 투옥 된다.
    // 2 평판이 높고 돈이 낮고 발사 수가 적으면 지원군에게 반동으로 몰려 도망친다.
    //
    // 3 평판이 높고 돈이 높고 발사 수가 많으면 새로운 마왕이 된다.
    // 4 평판이 높고 돈이 높고 발사 수가 적으면 지원군과 치킨집 프랜차이즈 장사를 한다.
    //
    // 5 평판이 낮고 돈이 높고 발사 수가 많으면 지원군을 따라 마왕성을 토벌 한다.
    // 6 평판이 낮고 돈이 높고 발사 수가 적으면 지원군을 돈으로 지원하여 마왕성을 무너뜨린다.
    //
    // 7 평판이 낮고 돈이 낮고 발사 수가 많으면 신념에 따라 마왕을 토벌하고 작렬히 전사.
    // 8 평판이 낮고 돈이 낮고 발사 수가 적으면 굶고 병들어 지원군에게 구조되어 왕국으로 귀환
    //
    // 9 평판이 높고 돈이 높고 발사 수가 0이면 지원군을 쏘고 치킨집을 계속한다.(내 일상을 빼앗지마아!!!!!!!)
    public void Ending()
    {
        isEnding = true;
        timeManager.dayTimeScale = 0;
        endingPopup.gameObject.SetActive(true);
        endingPopup.GetChild(0).GetComponent<Text>().DOText(endingStartStr, endingStartStr.Length/4).OnComplete(()=>{
            //엔딩 인덱스 정해주기
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
            //엔딩 인덱스에 맞춰서 엔딩 사진,텍스트보여주기
            //endingSprite.sprite = endingSprites[];
            afterCome.gameObject.SetActive(true);
            Talk(endingText, endingIndex +10, 0);
        });
        

    }
    
}
