using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using System;

public class PlayerHandler : MonoBehaviour
{

    // 게임의 컨셉
    // 괴물들도 좋아하는 치킨을 팔아재끼자

    // 게임 설명
    // 괴물 손님들은 쥰내게 예민해서 기름 온도를 잘못 맞춰서 치킨을 주면 화가난 나머지 가게 주인을 찢을수도 있다.
    // 기름 온도를 중간에서 벗어나지 않게 계속 조절해 주며 손님이 주문한 곗수에 맞추어 치킨을 튀겨 대접하면 된다.
    // 손님 하나하나 제한 시간이 있으며 제한시간은 손님의 인내심 이다.
    // 손님이 마음에 들지 않거나 강도가 가게에 왔다면 총을 쏴서 퇴치 하자.

    // 위치 설명
    // 좌측 = 기름 온도 조절
    // 우측 = 치킨 튀기기
    // 위쪽 = 손님 접대
    // 가운데 = 총

    private Transform player;

    [Header("플레이어 위치들")]
    [SerializeField] private Transform[] playerPositions;

    private enum PlayerState
    {
        IDLE =0,
        GUN = 1
    }
    private PlayerState playerState = PlayerState.IDLE;
    private enum PlayerDir
    {
        IDLE = 0,
        UP = 1,
        LEFT = 2,
        RiGHT = 3
    }
    private PlayerDir playerDir = PlayerDir.IDLE;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();

    }
    // Update is called once per frame

    private void LateUpdate()
    {
        if (Time.timeScale == 0) return;
        CheckplayerDir();
        MoveAsplayerDir();
        Working();
    }

    private void Working()
    {
        switch (playerDir)
        {
            case PlayerDir.IDLE:
                Gun();
                break;
            case PlayerDir.UP:
                //손님 상대하기
                GameManager.Instance.DisposeChicken();
                break;
            case PlayerDir.LEFT:
                //기름 온도 올리기
                GameManager.Instance.OilTempUp();
                break;
            case PlayerDir.RiGHT:
                //치킨 튀기기
                GameManager.Instance.FriedChicken();
                break;
        }

        ResetWorkHistory();
    }
    private void Gun()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerState == PlayerState.IDLE && GameManager.Instance.orderState == GameManager.OrderState.BETWEENORDER) 
        {
            playerState = PlayerState.GUN;
            Debug.Log(playerState);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.bullet > 0 && playerState == PlayerState.GUN && GameManager.Instance.orderState != GameManager.OrderState.ORDER)
        {
            playerState = PlayerState.IDLE;
            Debug.Log(playerState);
            GameManager.Instance.bullet--;
            GameManager.Instance.BulletImage[GameManager.Instance.bullet].GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0); 
            DOTween.KillAll();
            GameManager.Instance.CameraShaking(2f);
            GameManager.Instance.orderState = GameManager.OrderState.ORDER;
            GameManager.Instance.OrderTextReset();
            GameManager.Instance.EndDispose();
        }
        else if(GameManager.Instance.bullet <= 0)
        {
            //발사 실패 애니메이션
        }

    }

    private void ResetWorkHistory()
    {
        if (playerDir == PlayerDir.IDLE)
        {
            GameManager.Instance.ResetWorkHistory();
        }
        else
        {
            playerState = PlayerState.IDLE;
        }
    }

    private void MoveAsplayerDir()
    {
        player.position = playerPositions[(int)playerDir].position;
    }

    private void CheckplayerDir()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isHorizontal = h != 0 && v == 0 ;
        bool isLeft = h != 0 && h == -1;
        bool isUp = v != 0 && v == 1;
        bool isIdle = h == 0 && v == 0;
        playerDir = isIdle ? PlayerDir.IDLE : isHorizontal ? (isLeft ? PlayerDir.LEFT : PlayerDir.RiGHT) : (isUp ? PlayerDir.UP : PlayerDir.IDLE) ;
    }
}
