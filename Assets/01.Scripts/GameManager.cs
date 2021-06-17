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


    [Header("�ֹ�")]
    public OrderState orderState = OrderState.ORDER;
    public enum OrderState//�ֹ��� �ޱ����� ����
    {
        BETWEENORDER,//�ֹ��� ���� ���ϴ� ����,ġŲ�� �������� �� �ϴ� ����
        ORDER,//�ֹ��� ���� �� �ִ� ����, ġŲ�� �������� �� �ϴ� ����
        ORDERED//�ֹ��� ���� ����, ġŲ�� ������ �� �ִ� ����
    }
    [SerializeField] private Text orderText = null;//�ֹ��� �˷��ִ� UI

    [Header("�մ�")]
    public Queue<Customer> customers = new Queue<Customer>();//�մ��� �����ϴ� ť
    public int index = 0;//�մ��� ����
    [SerializeField] private Transform customerPrefab;//�մ� ������
    [SerializeField] private Transform customerFirstPosition;//�մ��� ���� ��ġ

    [Header("�γ���")]
    [SerializeField] private Slider WaitingTimeprogressBar = null;//�մ��� �γ����� �˷��� UI
    public float waitingTime = 0;//�γ���
    public float waitingTimeLimit = 1000;//�γ��� �ʱⰪ
    public float downWaitingTime =0;//�γ��� ����ġ

    [Header("ġŲ")]
    private float FriedCurValue = 0;//�󸶳� Ƣ�����°�
    [SerializeField] private Slider FriedprogressBar = null;//�󸶳� Ƣ��°��� �˷��� UI
    private Chicken orderedChicken = new Chicken();//ġŲ
    public List<Chicken> madeChickens = new List<Chicken>();//Ƣ�� ġŲ�� ������ ����Ʈ

    [Header("�⸧�µ�")]
    [SerializeField] private Slider OilprogressBar;//�⸧�� �µ��� �˷��� UI
    public float oilTemp = 0;//�⸧�� �µ�
    private const float oilLimit = 1000;//�⸧�µ� ����

    //ġŲ�� �ϼ��� ���� �⸧�µ��� �ִ�,�ּҰ�
    private const float minOilTemp = 500;
    private const float maxOilTemp = 700;

    //�ʴ� �󸶳� �µ��� �������°�
    private float downOilTemp = 100;



    [Header("�Ѿ�")]
    public int bullet = 0;//�Ѿ� ���
    private const int bulletLimit = 3;//�Ѿ˰����� �Ѱ�
    public Image[] BulletImage = null;//�Ѿ��� ������ �˷��� UI
    [Header("���ӿ���")]
    public Image gameOverPanel = null;//���ӿ��� �ǳ�

    private void Awake()
    {
        Instance = this;
        GameStart();//�������ڸ��� �Լ� ����
        //CreateCustomer();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(customers.Count);
        //Debug.Log(canCreate);

        if (orderState == OrderState.ORDERED)//�γ��� ����
            ProgressBarDown(ref waitingTime, waitingTimeLimit, downWaitingTime, ref WaitingTimeprogressBar);
        if (waitingTime <= 0)
        {
            //�մ��� ȭ��
            //Debug.Log("ANGRY");
            GameOver();
        }

        //�⸧ �µ� ����
        ProgressBarDown(ref oilTemp, oilLimit, downOilTemp, ref OilprogressBar);

        //�մ� ����
        SpawningCustomer();

        //���� �����
        if (Time.timeScale == 0)
        {
            if(Input.anyKeyDown)
            {
                GameStart();
            }
        }
    }
    public void CreateCustomer()//�մ� ���� �Լ�
    {
        if (customers!= null && customers.Count >=2 && customers.Last().index == 2)
            return;
        Customer.Create(customerPrefab, customerFirstPosition.position);//Customer Ŭ������ �����Լ� ȣ��

    }
    public void OrderTextReset()//�ֹ� �ؽ�Ʈ ����(�����)
    {
        orderText.text = " ";
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

        orderedChicken.RandomAmount();//ġŲ�� �� ���ϱ�
        waitingTime = customers.Peek().WaitingTime;//�մԿ� ���� �γ��� ����
        downWaitingTime = customers.Peek().DownWaitingTime;//�մԿ� ���� �γ��� ����ġ ����
        OrderTextReset();//�ؽ�Ʈ ����

        string chickenOrderTxt ="";
        switch (customers.Peek().customerType)
        {
            case Customer.CustomerType.NORMAL:
            case Customer.CustomerType.LATTE:
                chickenOrderTxt = $"ġŲ {orderedChicken.Amount}���� �ּ���";
                break;
            case Customer.CustomerType.ROBBER:
                chickenOrderTxt = "���� ġŲ �������� ����� ġŲ�� ������ �Ծ��.";
                break;
        }
        orderText.DOText(chickenOrderTxt, 3f).OnComplete(() => {
            switch (customers.Peek().customerType)
            {
                case Customer.CustomerType.NORMAL:
                    break;
                case Customer.CustomerType.LATTE:
                    OrderTextReset();
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
                    });
                    break;
                case Customer.CustomerType.ROBBER:
                    break;
            }
            orderState = OrderState.ORDERED;//���� ��ȯ(ġŲ�� ���� �� �� �ְ�)
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

    public void EndDispose()//�մ� ���� �� ���� ���� ����(�ֹ��� ���� �� �ִ� ������ ���·� ����)
    {
        customers.Peek().ExitTheStore();//����
        orderState = OrderState.ORDER;//���� ����
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
        /*if(customers.Peek().index == 1)
        {
            customers.Last().index--;
            customers.Last().MovePosition();
        }*/
        customers.Dequeue();//�մ� ����
    }
    public void ResetWorkHistory()//���� ��ô�� ����
    {
        FriedCurValue = 0;
        FriedprogressBar.value = 0;
    }
    public void FriedChicken()
    {
        float workingPower = 25;//�󸶳� �����ϴ���

        //FriedCurValue += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FriedCurValue += workingPower;//Ƣ�� ���� ����
            SoundManager.Instance.Fried.Play();//���� ���
        }
        if (FriedCurValue >= 100)//�� Ƣ������ ����
        {
            orderedChicken.IsGood = IsRightOilTemp();//ġŲ�� ���� üũ
            madeChickens.Add(orderedChicken);//���� ġŲ�� �߰�
            FriedCurValue = 0;//�󸶳� Ƣ������� ����
        }
        FriedprogressBar.value = FriedCurValue / 100;//�󸶳� Ƣ������� ǥ��
    }

    private bool IsRightOilTemp()//ġŲ�� �ϼ����� ����
    {
        return oilTemp >= minOilTemp && oilTemp <= maxOilTemp;//�⸧�� �µ��� �ϼ��� ���� �ּڰ�,�ִ밪�� ���߾�����(�� ���� ���� ����)
    }

    public void OilTempUp()//�⸧�� �µ��� �ö󰡰����ִ� �Լ�
    {
        float workingPower = 50;

        if (Input.GetKeyDown(KeyCode.Space) && oilTemp < oilLimit)
        {
            oilTemp += workingPower;
            SoundManager.Instance.Oil.Play();
        }

    }
    private void SpawningCustomer()//�մ��� �����ϴ� �Լ�
    {
        if (orderState == OrderState.ORDERED) return;

        int rand = Random.Range(0, 200);
        if (rand == 5) CreateCustomer();
    }
    public void GameOver()//������ ���¸� �������ִ� �Լ�
    {
        foreach (var item in customers)
        {
            Destroy(item.gameObject);
        }
        customers.Clear();
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
    }
    public void GameStart()//���� ���� ���ִ� �Լ�
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
