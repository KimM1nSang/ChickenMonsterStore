using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int index = 0; //손님의 순서
    public AudioSource CustomerCome;

    public bool isAngry = false;
    private void Awake()
    {
        CustomerCome = gameObject.GetComponent<AudioSource>();
    }

    public enum CustomerType //손님의 타입
    {
        NORMAL = 0,
        LATTE = 1,
        ROBBER = 2,
        SHIBADOGE = 3,
        MANDU = 4,
        JUJU = 5,
        TEDDYBEAR = 6
    }
    public CustomerType customerType = CustomerType.NORMAL;
    //생성
    public static void Create(Transform customerPrefab, Vector3 originPos,bool isEventCustomer)
    {
        Transform createCustomer = Instantiate(customerPrefab, originPos, Quaternion.identity);//생성

        Customer c = createCustomer.GetComponent<Customer>();

        
        c.index = Mathf.Clamp(GameManager.Instance.customers.Count, 0, 2);//순서 설정

        if (isEventCustomer)
        {
            int eventNum = GameManager.Instance.eventNum;
            
            c.SettingEventCustomer(eventNum);
            c.gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.eventCustomerImage[eventNum-1];
        }
        else
        {
            //Debug.Log(c.index);

            c.SettingCustomer();//세팅 함수 호출
            int rand = UnityEngine.Random.Range(0, GameManager.Instance.customerSprites.Length);
            c.gameObject.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.customerSprites[rand];
            //index++;
        }

        c.MovePosition();//순서에 맞는 위치 이동
        GameManager.Instance.customers.Enqueue(c);//생성한 손님을 큐에 삽입
    }
    

    //이동 위치 미리 정해두기(하드코딩)
    private Vector3[] positions = new Vector3[4] { new Vector3(0, 1f, 1.5f), new Vector3(0, 2.5f,1.6f), new Vector3(0, 4f, 1.7f), new Vector3(0, 5.5f, 1.8f) };


    public void MovePosition()
    {
        if(index >-1) //인덱스가 -1 초과 일때, 음수의 손님의 이동을 제외시킴
        {
            transform.DOMove(positions[index], .1f).OnComplete(() => //이동 후 아래의 로직 실행
            {
                GameManager.Instance.CameraShaking(.3f);//카메라 흔들기
                CustomerCome.Play();//음악 재생
                if (index == 0)//가장 가까운 손님일때(첫번째 손님일때)
                {
                    GameManager.Instance.MenuOrder();//주문을 받는다
                }
            });
        }
    }
    public void SettingCustomer()//세팅 로직
    {
        int _customerType = UnityEngine.Random.Range(0, 200);//손님의 타입을 랜덤으로 정함

        if (_customerType < 160)
        {
            customerType = CustomerType.NORMAL;
            WaitingTimeSetting(UnityEngine.Random.Range(800,1000), UnityEngine.Random.Range(100, 200));
        }
        else if (_customerType < 195)
        {
            customerType = CustomerType.LATTE;
            WaitingTimeSetting(UnityEngine.Random.Range(1200, 2000),50);
        }
        else
        {
            customerType = CustomerType.ROBBER;
            WaitingTimeSetting(UnityEngine.Random.Range(800, 1000), UnityEngine.Random.Range(200, 250));
        }

        //Debug.Log(customerType);
    }

    public void SettingEventCustomer(int eventNum)
    {
        customerType = (CustomerType)(eventNum + 2);
        WaitingTimeSetting(1000,50);
    }

    private void WaitingTimeSetting(int watingTime,int downWaitingTime)
    {
        WaitingTime = watingTime;
        DownWaitingTime = downWaitingTime;
    }
    //인내심
    public float WaitingTime { get; private set; }
    public float DownWaitingTime { get; private set; }

    //퇴장
    public void ExitTheStore(Vector3 exitpos) //퇴장 로직
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();//렌더러를 받아옴
        Vector3 exitPosition = transform.position - exitpos;//이동 위치

        //닷트윈 시퀸스
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(exitPosition, .5f));
        seq.Join(sr.DOFade(0f, 1f));
        seq.AppendCallback(() =>//트윈이 끝났을때
        {
            if (SaveGame.Instance.data.isDayTime)
            {
                GameManager.Instance.PullCustomer();//게임 매니저의 PullCustomer 함수 호출
            }
            else
            {
                Destroy(gameObject);//현 오브젝트 삭제
            }
            //Debug.Log(index);
            //gameObject.SetActive(false);
        });

    }
}
