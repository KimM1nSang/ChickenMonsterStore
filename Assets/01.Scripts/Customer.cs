using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public int index = 0; //손님의 순서
    public AudioSource CustomerCome;
    private void Awake()
    {
        CustomerCome = gameObject.GetComponent<AudioSource>();
    }

    //생성
    public static void Create(Transform customerPrefab, Vector3 originPos, int index)
    {
        Transform createCustomer = Instantiate(customerPrefab, originPos, Quaternion.identity);//생성

        Customer c = createCustomer.GetComponent<Customer>();

        c.index = index;//순서 설정

        c.SettingCustomer();//세팅 함수 호출

        //index++;
        c.MovePosition();//순서에 맞는 위치 이동
        GameManager.Instance.customers.Enqueue(c);//생성한 손님을 큐에 삽입
    }
    public enum CustomerType //손님의 타입
    {
        NORMAL,
        LATTE,
        ROBBER
    }
    public CustomerType customerType = CustomerType.NORMAL;

    //이동 위치 미리 정해두기(하드코딩)
    private Vector3[] positions = new Vector3[4] { new Vector3(0, 1f, 1.5f), new Vector3(0, 2.5f, 1.5f), new Vector3(0, 4f, 1.5f), new Vector3(0, 5.5f, 1.5f) };


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

        if (_customerType < 100)
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

        Debug.Log(customerType);
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
    public void ExitTheStore() //퇴장 로직
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();//렌더러를 받아옴
        Vector3 exitPosition = transform.position - new Vector3(1f, 0, 0);//이동 위치

        //닷트윈 시퀸스
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(exitPosition, .5f));
        seq.Join(sr.DOFade(0f, 1f));
        seq.AppendCallback(() =>//트윈이 끝났을때
        {
            GameManager.Instance.PullCustomer();//게임 매니저의 PullCustomer 함수 호출
            Destroy(gameObject);//현 오브젝트 삭제
        });

    }
}
