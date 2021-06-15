using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("카메라")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("손님")]
    public LinkedList<Customer> customers = new LinkedList<Customer>();
    [SerializeField] private Transform customerPrefab;
    [SerializeField] private Transform customerFirstPosition;
    [Header("주문")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState
    {
        BETWEENORDER,
        ORDER,
        ORDERED
    }
    [SerializeField] private Text orderText = null;

    [Header("치킨")]
    private float FriedCurValue = 0;
    private Chicken orderedChicken = new Chicken();
    public List<Chicken> madeChickens = new List<Chicken>();
    [SerializeField] private Slider FriedprogressBar = null;

    [Header("기름온도")]
    [SerializeField] private Slider OilprogressBar;
    public float oilTemp = 0;
    private const float oilLimit = 1000;

    private const float minOilTemp = 500;
    private const float maxOilTemp = 700;

    private float downOilTemp = 100;

    [Header("인내심")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;
    public float waitingTime = 0;
    public float waitingTimeLimit = 1000;

    [Header("총알")]
    public int bullet = 0;
    private const int bulletLimit = 3;
    public Image[] BulletImage = null;
    [Header("게임오버")]
    public Image gameOverPanel = null;

    private void Awake()
    {
        Instance = this;

        GameStart();
        //CreateCustomer();
    }

    // Update is called once per frame
    void Update()
    {
            //Debug.Log(canCreate);
        if (orderState == OrderState.ORDERED)
            ProgressBarDown(ref waitingTime, waitingTimeLimit, 200, ref WaitingTimeprogressBar);
        if (waitingTime <= 0)
        {
            //손님이 화남
            Debug.Log("ANGRY");
            GameOver();
        }
        ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);
        SpawningCustomer();
        if (Time.timeScale == 0)
        {
            if(Input.anyKeyDown)
            {
                GameStart();
            }
        }
    }
    public void CreateCustomer()
    {
        if (customers.Count >= 3 || !CanCreate())
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position);
    }
    public void OrderTextReset()
    {
        orderText.text = " ";
    }
    public void CameraShaking(float force)
    {
        ImpulseSource.GenerateImpulse(force);
    }
    private void ProgressBarDown(ref float mainNum, float limitNum, float downNum, ref Slider progressBar)
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

        orderedChicken.RandomAmount();
        waitingTime = waitingTimeLimit;
        OrderTextReset();
        orderText.DOText($"치킨 {orderedChicken.Amount}마리 주세요", 3f).OnComplete(() => {
            orderState = OrderState.ORDERED;
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
    public void EndDispose()
    {
        customers.First.Value.ExitTheStore();
        customers.RemoveFirst();
        orderState = OrderState.ORDER;
    }
    public void PullCustomer()
    {
        foreach (var item in customers)
        {
            if(item._index >=1)
            {
                item._index--;
            }
            item.MovePosition();
        }
    }
    public bool CanCreate()
    {
        return true ;
    }

    public void ResetWorkHistory()
    {
        FriedCurValue = 0;
        FriedprogressBar.value = 0;
    }
    public void FriedChicken()
    {
        float workingPower = 25;

        //FriedCurValue += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FriedCurValue += workingPower;
            SoundManager.Instance.Fried.Play();
        }
        if (FriedCurValue >= 100)
        {
            orderedChicken.IsGood = IsRightOilTemp();
            madeChickens.Add(orderedChicken);
            FriedCurValue = 0;
        }
        FriedprogressBar.value = FriedCurValue / 100;
    }

    private bool IsRightOilTemp()
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;
    }

    public void OilTempUp()
    {
        float workingPower = 50;

        if (Input.GetKeyDown(KeyCode.Space) && oilTemp < oilLimit)
        {
            oilTemp += workingPower;
            SoundManager.Instance.Oil.Play();
        }

    }
    private void SpawningCustomer()
    {
        if (orderState == OrderState.ORDERED) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameOver()
    {
        foreach (var item in customers)
        {
            Destroy(item.gameObject);
        }
        customers.Clear();
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
    }
    public void GameStart()
    {
        waitingTime = waitingTimeLimit;
        OrderTextReset();
        bullet = bulletLimit;
        oilTemp = oilLimit;
        orderState = OrderState.ORDER;
        foreach (var item in BulletImage)
        {
            item.gameObject.SetActive(true);
        }
        Time.timeScale = 1;
        gameOverPanel.gameObject.SetActive(false);
    }
}
