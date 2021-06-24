using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.IO;
using System.Dynamic;

public class GameManager : MonoBehaviour
{
    public SaveData _data;
    public SaveData data
    {
        get
        {
            if(_data == null)
            {
                LoadGameData();
                SaveGameData();
            }
            return _data;
        }
    }
    public static GameManager Instance { get; private set; }

    private string gameDataFileName = "ChickenMonster";
    private const string gameDataSaveType = ".json";

    [Header("카메라")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("시간")]
    public TimeManager timeManager = null;
    private int dayCheck = 0;
    public int day = 0;
    public Text dayText = null;
    [Header("주문")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState//주문을 받기위한 상태
    {
        BETWEENORDER,//주문을 받지 못하는 상태,치킨을 제공하지 못 하는 상태
        ORDER,//주문을 받을 수 있는 상태, 치킨을 제공하지 못 하는 상태
        ORDERED//주문을 받은 상태, 치킨을 제공할 수 있는 상태
    }
    [SerializeField] private Text orderText = null;//주문을 알려주는 UI
    private int orderAmount = 0;
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
    [Header("평판")]
    public Text reputationText = null;
    private void Awake()
    {
        Instance = this;
        GameSet();
        GameReset();//시작하자마자 함수 실행
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(customers.Count);
        //Debug.Log(canCreate);

            //Debug.Log(timeManager.time);
        if (timeManager.time <= 0)
        {
            if (timeManager.IsDayTime)
            {
                timeManager.SetDayTime(false);
                DOTween.KillAll();
                foreach (var item in customers)
                {
                    //Destroy(item.gameObject);
                    item.ExitTheStore();
                }
            }
            else
            {
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
            if(dayCheck >= 2)
            {
                dayCheck = 0;
                day++;
                RefreshText();
            }
        }
        if (timeManager.IsDayTime)
        {
            if (orderState == OrderState.ORDERED)//인내심 감소
                ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
            if (waitingTime <= 0)
            {
                //손님이 화남
                //Debug.Log("ANGRY");
                
                orderText.DOText("언제 주는거야!!!!!", 2f).OnComplete(()=> {
                    StartCoroutine(CustomerAttack());
                });
            }

            //기름 온도 감소
            ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);

            //손님 생성
            SpawningCustomer();
          
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if(!isPanelOn)
                {
                    shopPanel.gameObject.transform.DOMove(Vector3.zero, .5f).OnComplete(()=>{
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
        timeManager.SubtractTime(Time.deltaTime);
    }
    public void CreateCustomer()//손님 생성 함수
    {
        if (customers != null && customers.Count >= 2 && customers.Last().index == 2) 
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position);//Customer 클래스의 생성함수 호출

    }

    public void OrderTextReset()//주문 텍스트 리셋(지우기)
    {
        orderText.text = "";
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

        orderAmount = RandomAmount(3);//주문 갯수 랜덤
        waitingTime = customers.Peek().WaitingTime;//손님에 따른 인내심 세팅
        downWaitingTime = customers.Peek().DownWaitingTime;//손님에 따른 인내심 감소치 세팅
        OrderTextReset();//텍스트 리셋

        string chickenOrderTxt ="";
        switch (customers.Peek().customerType)
        {
            case Customer.CustomerType.NORMAL:
                chickenOrderTxt = $"치킨 {orderAmount}마리 주세요";
                break;
            case Customer.CustomerType.LATTE:
                chickenOrderTxt = $"그 치킨 {orderAmount}개만 좀 줘봐";
                break;
            case Customer.CustomerType.ROBBER:
                chickenOrderTxt = "저는 치킨 강도에요 당신의 치킨을 가지러 왔어요.";
                break;
        }
        orderText.DOText(chickenOrderTxt, 3f).OnComplete(() => {
            switch (customers.Peek().customerType)
            {
                case Customer.CustomerType.NORMAL:
                    break;
                case Customer.CustomerType.LATTE:
                    OrderTextReset();
                    orderText.DOText("그런데 말이야", 1f).OnComplete(() => {
                        OrderTextReset();
                        orderText.DOText("이", .1f).OnComplete(() => {
                            OrderTextReset();
                            orderText.DOText("치킨이란게 참 신기해", 1.5f).OnComplete(() => {
                                OrderTextReset();
                                orderText.DOText("나때는 이런 그... 뭐냐 크런키?", 1.5f).OnComplete(() => {
                                OrderTextReset();
                                    orderText.DOText("그런게 없었거든", 1.5f).OnComplete(() => {
                                            OrderTextReset();
                                        orderText.DOText("지금 나 늙었다고 무시하는거야?", 1.5f).OnComplete(() => {
                                            OrderTextReset();
                                            orderText.DOText("허,참 어이가 없어서", 1.5f).OnComplete(() => {
                                                OrderTextReset();
                                                orderText.DOText("니가 두발로 걷기 시작할때 말이야", 1.5f).OnComplete(() => {
                                                    OrderTextReset();
                                                    orderText.DOText("어?", 1.5f).OnComplete(() => {

                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                    break;
                case Customer.CustomerType.ROBBER:
                    break;
            }
            orderState = OrderState.ORDERED;//상태 변환(치킨을 제공 할 수 있게)
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
        if (orderState != OrderState.ORDERED) return;//스테이트가 ORDERED 가 아니면 리턴
        orderState = OrderState.BETWEENORDER;

        DOTween.KillAll();
        OrderTextReset();
        if (madeChickens.Count == orderAmount && IsGoodChickenInHere())
        {
            orderText.DOText("잊을 수 없는 이 치킨의 맛!", 2f).OnComplete(() => {
                
                EndDispose();

            });
            for (int i = 0; i < orderAmount; i++)
            {
                data.money += madeChickens[i].currentPrice;
            Debug.Log(madeChickens[i].currentPrice);
            }
            data.reputation += 100;
        }
        else if (madeChickens.Count <= 0)
        {
            orderText.DOText("날 기망 하는거냐!!!!!!", 2f).OnComplete(() => {
                StartCoroutine(CustomerAttack());
            });
            data.reputation -= 70;
        }
        else if (madeChickens.Count != orderAmount)
        {
            orderText.DOText("갯수가 틀렸잖아!!!!!!!!!!!!!!", 2f).OnComplete(() => {
                StartCoroutine(CustomerAttack());
            });
            for (int i = 0; i < orderAmount; i++)
            {
                data.money += madeChickens[i].currentPrice / 4;
            }
            data.reputation -= 20;
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
                orderText.DOText("튀김상태가 이게 뭐야!!!!!!!!!!!!.", 2f).OnComplete(() => {
                    StartCoroutine(CustomerAttack());
                });
                for (int i = 0; i < orderAmount; i++)
                {
                    data.money += madeChickens[i].currentPrice /2;
                }
                data.reputation -= 30;
            }
        }
        RefreshText();
        madeChickens.Clear();
        waitingTime = waitingTimeLimit;
        orderAmount = 0;
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
        customers.Peek().ExitTheStore();//퇴장
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
        attckPanel.gameObject.SetActive(false);

    }
    public void PullCustomer()//전체 위치 이동
    {
        foreach (var item in customers)
        {
            if(item.index !=0)
            {
                item.index--;//인덱스 감소
                item.MovePosition();//이동
            }
        }
        /*if(customers.Peek().index == 1)
        {
            customers.Last().index--;
            customers.Last().MovePosition();
        }*/
        customers.Dequeue();//손님 제거
    }
    public void ResetWorkHistory()//일의 진척도 리셋
    {
        FriedCurValue = 0;
        FriedprogressBar.value = 0;
    }
    public void FriedChicken()
    {
        float workingPower = 25;//얼마나 증가하는지

        //FriedCurValue += Time.deltaTime;
        if(data.chickens.Count >0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FriedCurValue += workingPower;//튀김 상태 증가
                SoundManager.Instance.Fried.Play();//음원 재생
            }
            if (FriedCurValue >= 100)//다 튀겼으면 리셋
            {
                //Debug.Log(data.chickens.Peek().currentPrice);
                data.chickens.Peek().IsGood = IsRightOilTemp();//치킨의 상태 체크
                madeChickens.Add(data.chickens.Dequeue());//만든 치킨에 추가
                //Debug.Log(madeChickens[madeChickens.Count -1].currentPrice);
                FriedCurValue = 0;//얼마나 튀겼는지를 리셋
            }
            FriedprogressBar.value = FriedCurValue / 100;//얼마나 튀겼는지를 표기
        }
        else
        {
            //실패 음원 재생
            Debug.Log("생닭 부족");
        }
    }

    private bool IsRightOilTemp()//치킨의 완성도를 측정
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;//기름의 온도가 완성을 위한 최솟값,최대값을 맞추었는지(두 수의 사이 인지)
    }

    public void OilTempUp()//기름의 온도가 올라가게해주는 함수
    {
        float workingPower = 20;

        if (Input.GetKeyDown(KeyCode.Space) && data.friedPowders.Count >=1)
        {
            if (oilTemp < oilLimit)
            {
                oilTemp += workingPower * (data.friedPowders.Peek().rank +1);
                SoundManager.Instance.Oil.Play();
            }
            RefreshText();
            data.friedPowders.Dequeue();
        }

    }
    public void RefreshText()
    {
        friedPowder.text = $"{data.friedPowders.Count}";
        rawChicken.text = $"{data.chickens.Count}";
        dayText.text = $"{day} - Day";
        foreach (var item in moneyText)
        {
            item.text = string.Format("{0:#,###}", data.money.ToString());
        }
        minOilTemp = 500 - 40 * data.oil.rank;
        float min =  40 * data.oil.rank;
        niceTempImage.GetComponent<RectTransform>().offsetMin = new Vector2(niceTempImage.GetComponent<RectTransform>().offsetMin.x, -min);
        reputationText.text = data.reputation.ToString();
    }
    private void SpawningCustomer()//손님을 생성하는 함수
    {
        if (orderState == OrderState.ORDERED || timeManager.IsDayTime == false) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameReset()//값을 리셋 해주는 함수
    {
        RefreshText();
        waitingTime = waitingTimeLimit;
        OrderTextReset();
        bullet = bulletLimit;
        oilTemp = oilLimit;
        orderState = OrderState.ORDER;
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
    }
    public void GameSet()
    {
        if(data.money == 0)
        {
            data.money = 200;
        }
    }
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + gameDataFileName + gameDataSaveType;

        if(File.Exists(filePath))
        {
            Debug.Log("불러오기 성공!");
            string FromJsonData = File.ReadAllText(filePath);
            _data = JsonUtility.FromJson<SaveData>(FromJsonData);
        }
        else
        {
            Debug.Log("파일이 없는 관계로 새로운 파일 생성");

            _data = new SaveData();
        }
    }
    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(data);
        string filePath = Application.persistentDataPath + gameDataFileName + gameDataSaveType;
        File.WriteAllText(filePath, ToJsonData);
        Debug.Log("저장 완료");
    }
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
