using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("ī�޶�")]
    public CinemachineVirtualCamera vCamera;
    public CinemachineImpulseSource ImpulseSource;

    [Header("�մ�")]
    public LinkedList<Customer> customers = new LinkedList<Customer>();
    [SerializeField] private Transform customerPrefab;
    [SerializeField] private Transform customerFirstPosition;
    [Header("�ֹ�")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState
    {
        BETWEENORDER,
        ORDER,
        ORDERED
    }
    [SerializeField] private Text orderText = null;

    [Header("ġŲ")]
    private float FriedCurValue = 0;
    private Chicken orderedChicken = new Chicken();
    public List<Chicken> madeChickens = new List<Chicken>();
    [SerializeField] private Slider FriedprogressBar = null;

    [Header("�⸧�µ�")]
    [SerializeField] private Slider OilprogressBar;
    public float oilTemp = 0;
    private const float oilLimit = 1000;

    private const float minOilTemp = 500;
    private const float maxOilTemp = 700;

    private float downOilTemp = 100;

    [Header("�γ���")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;
    public float waitingTime = 0;
    public float waitingTimeLimit = 1000;

    [Header("�Ѿ�")]
    public int bullet = 0;
    private const int bulletLimit = 3;
    public Image[] BulletImage = null;
    [Header("���ӿ���")]
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
            //�մ��� ȭ��
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
        if (orderState != OrderState.ORDER) return;//������Ʈ�� ORDER �� �ƴϸ� ����
        orderState = OrderState.BETWEENORDER;

        orderedChicken.RandomAmount();
        waitingTime = waitingTimeLimit;
        OrderTextReset();
        orderText.DOText($"ġŲ {orderedChicken.Amount}���� �ּ���", 3f).OnComplete(() => {
            orderState = OrderState.ORDERED;
        });
    }
    public void DisposeChicken()
    {
        if (orderState != OrderState.ORDERED) return;//������Ʈ�� ORDERED �� �ƴϸ� ����
        orderState = OrderState.BETWEENORDER;

        DOTween.KillAll();
        OrderTextReset();
        if (madeChickens.Count == orderedChicken.Amount && Instance.orderedChicken.IsGood == true)
        {
            orderText.DOText("�� ��� ���ϴ�.", 2f).OnComplete(() => {
                EndDispose();
            });
        }
        else if (madeChickens.Count <= 0)
        {
            orderText.DOText("�Һ��ڸ� ����ϴ°ų�!!!!!!!!!", 2f).OnComplete(() => {
                EndDispose();
            });
        }
        else if (madeChickens.Count != orderedChicken.Amount)
        {
            orderText.DOText("������ Ʋ���ݾ�!!!!!!!!!!!!!!", 2f).OnComplete(() => {
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
                orderText.DOText("Ƣ����°� �̰� ����!!!!!!!!!!!!.", 2f).OnComplete(() => {
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
