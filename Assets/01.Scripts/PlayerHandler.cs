using UnityEngine;
using DG.Tweening;

public class PlayerHandler : MonoBehaviour
{

    // 게임의 컨셉
    // 괴물들도 좋아하는 치킨을 팔아재끼자

    // 게임 설명
    // 괴물 손님들은 쥰내게 예민해서 기름 온도를 잘못 맞춰서 치킨을 주면 화가난 나머지 가게 주인을 찢을수도 있다.
    // 기름 온도를 중간에서 벗어나지 않게 계속 조절해 주며 손님이 주문한 곗수에 맞추어 치킨을 튀겨 대접하면 된다.
    // 손님 하나하나 제한 시간이 있으며 제한시간은 손님의 인내심 이다.
    // 손님이 마음에 들지 않거나 강도가 가게에 왔다면 총을 쏴서 퇴치 하자.

    // 시간 개녕
    //  낮과 밤, 낮에는 장사 밤에는 장사 준비(상점)
    // 상점
    //  재료 구입
    //   재료는 가격에 영향을 줌
    //
    //   생닭
    //    A,B,C,D,E,F급 생닭
    //
    //    확률
    //    F = 50;
    //    E = 25;
    //    D = 12.5f;
    //    C = 6.25;
    //    B = 3.125;
    //    A = 1.5625;
    //  
    //    상점
    //    0.1/0.5/0.85
    //
    //   튀김옷
    //    A,B,C,D,E,F급 튀김옷
    //   
    //  기름 구입
    //   한번만 사도 됨
    //    A,B,C,D,E,F급 기름
    //    기름의 등급에 따라 치킨의 완성도를 높이기 쉬워짐

    //  직원 고용
    //   A,B,C,D,E,F급 직원
    //   청소 담당
    //    평판 증가도가 오름
    //   카운터 담당
    //    판매시 획득 재화가 증가함
    // 평판
    //  평판에 따라 손님의 출현 빈도가 달라짐

    // 위치 설명
    // 좌측 = 기름 온도 조절
    // 우측 = 치킨 튀기기
    // 위쪽 = 손님 접대
    // 가운데 = 총

    // 엔딩
    // 지원군이 도착하고 나서의 이야기
    // 평판,돈,총 발사 수에 따라 달라진다.

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
    //
    // 한 사이클
    //
    // 정비 시간 추가 (완)
    // 날짜 시스템 추가 (완)
    // 엔딩 추가
    // 10일 마다 이벤트 발생

    private Transform player;

    [Header("플레이어 위치들")]
    [SerializeField] private Transform[] playerPositions;//플레이어의 위치값들
    [Header("플레이어 스프라이트들")]
    [SerializeField] private Sprite[] playerSprites;

    private enum PlayerState//총기 상태
    {
        IDLE =0,//총을 들지 않은 상태
        GUN = 1//총을 든 상태
    }
    private PlayerState playerState = PlayerState.IDLE;
    private enum PlayerDir//플레이어의 위치와 방향
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
        player = GameObject.Find("Player").GetComponent<Transform>();//플레이어 지정

    }
    // Update is called once per frame

    private void LateUpdate()
    {
        CheckplayerDir();//입력 체크
        MoveAsplayerDir();//입력에 따른 위치 이동

        Working();//입력에 따른 행동
    }

    private void Working()
    {
        switch (playerDir)//플레이어의 상태(위치)에 따른 행동
        {
            case PlayerDir.IDLE:
                //장전 or 발사
                Gun();
                break;
            case PlayerDir.UP:
                //손님 상대하기(치킨 제공)
                GameManager.Instance.DisposeChicken();
                break;
            case PlayerDir.LEFT:
                //기름 온도 올리기(바의 값상승)
                GameManager.Instance.OilTempUp();
                break;
            case PlayerDir.RiGHT:
                //치킨 튀기기(바의 값상승)
                GameManager.Instance.FriedChicken();
                break;
        }

        ResetWorkHistory();//상태 복구
    }
    private void Gun()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerState == PlayerState.IDLE && GameManager.Instance.customers.Count>=1 && GameManager.Instance.orderState != GameManager.OrderState.ORDER && GameManager.Instance.isPanelOn) 
        {
            //총을 들기
            playerState = PlayerState.GUN;
            Debug.Log(playerState);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance.bullet > 0 && playerState == PlayerState.GUN && GameManager.Instance.customers.Count >= 1&& GameManager.Instance.orderState != GameManager.OrderState.ORDER && GameManager.Instance.isPanelOn)
        {
            //발사
            playerState = PlayerState.IDLE;
            GameManager.Instance.customers.Peek().isAngry = false;
            //Debug.Log(playerState);
            SaveGame.Instance.data.shootNum++;
            GameManager.Instance.bullet--;//총알 소모
            GameManager.Instance.BulletImage[GameManager.Instance.bullet].GetComponent<UnityEngine.UI.Image>().color = new Color(0,0,0); //총알 소모 이미지 변환
            DOTween.Kill("Talk");//닷트윈 정지
            SoundManager.Instance.GunSound.Play();//음원 재생
            GameManager.Instance.CameraShaking(2f);//카메라 흔들기
            GameManager.Instance.TextReset(GameManager.Instance.orderText);//택스트를 비운다
            GameManager.Instance.EndDispose();//손님을 퇴장 시킨다.
            //예외 처리
            /*if(GameManager.Instance.index >=2)
            {
                GameManager.Instance.index -= GameManager.Instance.customers.Count-2;
            }*/
            

            //발사 성공 애니메이션 필요
        }
        else if(GameManager.Instance.bullet <= 0)
        {
            //발사 실패 애니메이션
        }

    }

    private void ResetWorkHistory()//플레이어의 상태를 초기와 같이 만든다
    {
        if (playerDir == PlayerDir.IDLE)//플레이어가 중립의 위치 일때
        {
            GameManager.Instance.ResetWorkHistory();//일의 진행도 리셋
        }
        else
        {
            playerState = PlayerState.IDLE;//총을 들지 않은 상태
        }
    }

    private void MoveAsplayerDir()//방향에 따른 위치 이동
    {
        player.position = playerPositions[(int)playerDir].position;
    }

    private void CheckplayerDir()//입력에 따른 플레이어의 위치 상태 변환
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isHorizontal = h != 0 && v == 0 ;
        bool isLeft = h != 0 && h == -1;
        bool isUp = v != 0 && v == 1;
        bool isIdle = h == 0 && v == 0;
        playerDir = isIdle ? PlayerDir.IDLE : isHorizontal ? (isLeft ? PlayerDir.LEFT : PlayerDir.RiGHT) : (isUp ? PlayerDir.UP : PlayerDir.IDLE) ;
        player.GetComponent<SpriteRenderer>().sprite = playerSprites[(int)playerDir];
        if (h != 0)
        {
            player.localScale = new Vector3(8 * h, player.localScale.y, player.localScale.z);
        }
    }
}
