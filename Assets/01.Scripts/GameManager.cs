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

    [Header("카메라")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;


    [Header("주문")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState//주문을 받기위한 상태
    {
        BETWEENORDER,//주문을 받지 못하는 상태,치킨을 제공하지 못 하는 상태
        ORDER,//주문을 받을 수 있는 상태, 치킨을 제공하지 못 하는 상태
        ORDERED//주문을 받은 상태, 치킨을 제공할 수 있는 상태
    }
    [SerializeField] private Text orderText = null;//주문을 알려주는 UI

    [Header("손님")]
    public Queue<Customer> customers = new Queue<Customer>();//손님을 저장하는 큐
    public int index = 0;//손님의 순서
    [SerializeField] private Transform customerPrefab;//손님 프리팹
    [SerializeField] private Transform customerFirstPosition;//손님의 생성 위치

    [Header("인내심")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;//손님의 인내심을 알려줄 UI
    public float waitingTime = 0;//인내심
    public float waitingTimeLimit = 1000;//인내심 초기값
    public float downWaitingTime =0;//인내심 감소치

    [Header("치킨")]
    private float FriedCurValue = 0;//얼마나 튀겨졌는가
    [SerializeField] private Slider FriedprogressBar = null;//얼마나 튀겼는가를 알려줄 UI
    private Chicken orderedChicken = new Chicken();//치킨
    public List<Chicken> madeChickens = new List<Chicken>();//튀긴 치킨을 저장할 리스트

    [Header("기름온도")]
    [SerializeField] private Slider OilprogressBar;//기름의 온도를 알려줄 UI
    public float oilTemp = 0;//기름의 온도
    private const float oilLimit = 1000;//기름온도 제한

    //치킨의 완성을 위한 기름온도의 최대,최소값
    private const float minOilTemp = 500;
    private const float maxOilTemp = 700;

    //초당 얼마나 온도가 떨어지는가
    private float downOilTemp = 100;



    [Header("총알")]
    public int bullet = 0;//총알 곗수
    private const int bulletLimit = 3;//총알갯수의 한계
    public Image[] BulletImage = null;//총알의 갯수를 알려줄 UI
    [Header("게임오버")]
    public Image gameOverPanel = null;//게임오버 판넬

    private void Awake()
    {
        Instance = this;
        GameStart();//시작하자마자 함수 실행
        //CreateCustomer();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(customers.Count);
        //Debug.Log(canCreate);

        if (orderState == OrderState.ORDERED)//인내심 감소
            ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
        if (waitingTime <= 0)
        {
            //손님이 화남
            //Debug.Log("ANGRY");
            GameOver();
        }

        //기름 온도 감소
        ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);

        //손님 생성
        SpawningCustomer();

        //게임 재시작
        if (Time.timeScale == 0)
        {
            if(Input.anyKeyDown)
            {
                GameStart();
            }
        }
    }
    public void CreateCustomer()//손님 생성 함수
    {
        if (customers!= null && customers.Count >=2 && customers.Last().index == 2)
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position);//Customer 클래스의 생성함수 호출

    }
    public void OrderTextReset()//주문 텍스트 리셋(지우기)
    {
        orderText.text = " ";
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

        orderedChicken.RandomAmount();//치킨의 양 정하기
        waitingTime = customers.Peek().WaitingTime;//손님에 따른 인내심 세팅
        downWaitingTime = customers.Peek().DownWaitingTime;//손님에 따른 인내심 감소치 세팅
        OrderTextReset();//텍스트 리셋

        string chickenOrderTxt ="";
        switch (customers.Peek().customerType)
        {
            case Customer.CustomerType.NORMAL:
            case Customer.CustomerType.LATTE:
                chickenOrderTxt = $"치킨 {orderedChicken.Amount}마리 주세요";
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
    public void DisposeChicken()
    {
        if (orderState != OrderState.ORDERED) return;//스테이트가 ORDERED 가 아니면 리턴
        orderState = OrderState.BETWEENORDER;

        DOTween.KillAll();
        OrderTextReset();
        if (madeChickens.Count == orderedChicken.Amount && Instance.orderedChicken.IsGood == true)
        {
            orderText.DOText("잘 놀다 갑니다.", 2f).OnComplete(() => {
                EndDispose();
            });
        }
        else if (madeChickens.Count <= 0)
        {
            orderText.DOText("소비자를 기망하는거냐!!!!!!!!!", 2f).OnComplete(() => {
                EndDispose();
            });
        }
        else if (madeChickens.Count != orderedChicken.Amount)
        {
            orderText.DOText("갯수가 틀렸잖아!!!!!!!!!!!!!!", 2f).OnComplete(() => {
                EndDispose();
            });
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
                    EndDispose();
                });
            }
        }
        waitingTime = waitingTimeLimit;
        madeChickens.Clear();
        orderedChicken.Amount = 0;
    }

    public void EndDispose()//손님 퇴장 및 상태 원상 복구(주문을 받을 수 있는 원래의 상태로 복구)
    {
        customers.Peek().ExitTheStore();//퇴장
        orderState = OrderState.ORDER;//상태 복구
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FriedCurValue += workingPower;//튀김 상태 증가
            SoundManager.Instance.Fried.Play();//음원 재생
        }
        if (FriedCurValue >= 100)//다 튀겼으면 리셋
        {
            orderedChicken.IsGood = IsRightOilTemp();//치킨의 상태 체크
            madeChickens.Add(orderedChicken);//만든 치킨에 추가
            FriedCurValue = 0;//얼마나 튀겼는지를 리셋
        }
        FriedprogressBar.value = FriedCurValue / 100;//얼마나 튀겼는지를 표기
    }

    private bool IsRightOilTemp()//치킨의 완성도를 측정
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;//기름의 온도가 완성을 위한 최솟값,최대값을 맞추었는지(두 수의 사이 인지)
    }

    public void OilTempUp()//기름의 온도가 올라가게해주는 함수
    {
        float workingPower = 50;

        if (Input.GetKeyDown(KeyCode.Space) && oilTemp < oilLimit)
        {
            oilTemp += workingPower;
            SoundManager.Instance.Oil.Play();
        }

    }
    private void SpawningCustomer()//손님을 생성하는 함수
    {
        if (orderState == OrderState.ORDERED) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameOver()//게임의 상태를 정리해주는 함수
    {
        foreach (var item in customers)
        {
            Destroy(item.gameObject);
        }
        customers.Clear();
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
    }
    public void GameStart()//값을 리셋 해주는 함수
    {
        waitingTime = waitingTimeLimit;
        OrderTextReset();
        bullet = bulletLimit;
        oilTemp = oilLimit;
        orderState = OrderState.ORDER;
        foreach (var item in BulletImage)
        {
            item.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
        }
        Time.timeScale = 1;
        gameOverPanel.gameObject.SetActive(false);
    }
}
