using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI; // 버튼 제어를 위해 추가
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 포톤용 해시테이블

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject hostPanel;          // 방장(왼쪽) 사각형 패널
    public GameObject guestPanel;         // 게스트(오른쪽) 사각형 패널

    public TextMeshProUGUI hostNameText;  // 방장 이름 텍스트
    public TextMeshProUGUI guestNameText; // 게스트 이름 텍스트
    public TextMeshProUGUI statusText;    // 상단 상태 텍스트 (Waiting...)

    [Header("New UI References")]
    public TextMeshProUGUI roomNameText;     // 좌측 상단 'New Text' (방 이름)
    public Button actionButton;              // 하단 버튼 자체
    public TextMeshProUGUI actionButtonText; // 중앙 하단 빈 버튼 안의 텍스트
    
    private const string IS_READY = "isReady";

    void Start()
    {
        // 1. 방 이름(또는 번호)을 가져와서 텍스트에 적용
        // (RandomRoom으로 들어왔다면 포톤이 부여한 임의의 방 이름이 뜸)
        roomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

        // 2. 내가 방장(Host)인지 일반 클라이언트(Guest)인지 체크해서 버튼 텍스트 변경
        if (PhotonNetwork.IsMasterClient)
        {
            actionButtonText.text = "Game Start";
            actionButton.interactable = false; // 방장은 처음엔 버튼 비활성화
        }
        else
        {
            actionButtonText.text = "Ready";
            actionButton.interactable = true;
        }

        // 씬에 들어오자마자 현재 방 인원 상태에 맞춰 UI 업데이트
        UpdateRoomUI();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // 버튼을 클릭했을 때 실행될 함수 (유니티 OnClick에 연결)
    public void OnClickAction()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // [방장] 게임 시작 (씬 이름은 실제 게임 씬 이름으로 변경하세요)
            Debug.Log("게임을 시작합니다!");
            PhotonNetwork.LoadLevel("ReactionGameScene");
        }
        else
        {
            // [클라이언트] 레디 설정
            SetReady(true);
        }
    }

    void SetReady(bool ready)
    {
        // 내 로컬 상태 변경 (버튼 비활성화 및 회색으로)
        actionButton.interactable = false;
        actionButtonText.text = "Ready!";

        // 포톤 서버에 나의 레디 상태 기록 (CustomProperties)
        Hashtable props = new Hashtable { { IS_READY, ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // 누군가의 CustomProperties가 변경되었을 때 실행되는 포톤 콜백
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log($" [콜백 수신] {targetPlayer.NickName}님의 데이터가 변경되었습니다!");

        // 변경된 속성에 "isReady"가 포함되어 있는지 확인
        if (changedProps.ContainsKey(IS_READY))
        {
            Debug.Log($" [상태 확인] 레디 상태가 {(bool)changedProps[IS_READY]} 로 변경됨!");
            CheckReadyStatus();
        }
    }

    void CheckReadyStatus()
    {
        // 방장일 때만 게스트의 레디 상태를 체크해서 시작 버튼 활성화
        if (PhotonNetwork.IsMasterClient)
        {
            bool allReady = false;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.IsMasterClient) continue; // 방장은 패스

                Debug.Log($" [방장 검사] 게스트({p.NickName})의 속성 검사 중...");

                // 게스트의 isReady 속성이 true인지 확인
                if (p.CustomProperties.ContainsKey(IS_READY))
                {
                    bool isGuestReady = (bool)p.CustomProperties[IS_READY];
                    Debug.Log($" [방장 검사] 게스트의 레디 값: {isGuestReady}");

                    if (isGuestReady)
                    {
                        allReady = true;
                    }
                }
                else
                {
                    Debug.Log($" [방장 검사] 아직 게스트의 레디 데이터가 없습니다.");
                }
            }

            Debug.Log($" [최종 결과] Game Start 버튼 활성화 여부: {allReady}");
            actionButton.interactable = allReady;
        }
    }

    // 현재 방에 있는 인원을 체크해서 UI를 켜고 끄는 함수
    void UpdateRoomUI()
    {
        // 방장은 항상 있으므로 왼쪽 패널 켜고 내 이름 넣기
        hostPanel.SetActive(true);
        hostNameText.text = PhotonNetwork.MasterClient.NickName;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 방에 2명이 꽉 찼다면 오른쪽 패널 켜기
            guestPanel.SetActive(true);
            statusText.text = "match complete! get ready.";

            // 상대방(마스터 클라이언트가 아닌 사람)의 닉네임 찾기
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (!p.IsMasterClient)
                {
                    guestNameText.text = p.NickName;
                    break;
                }
            }
            // 인원이 찼을 때 한 번 더 체크 (이미 레디하고 기다렸을 수도 있으니)
            CheckReadyStatus();
        }
        else
        {
            // 나 혼자라면 오른쪽 패널 끄기
            guestPanel.SetActive(false);
            statusText.text = "waiting opponent...";
            if (PhotonNetwork.IsMasterClient)
                actionButton.interactable = false;
        }
    }

    // 방에 있던 다른 플레이어가 들어왔을 때 자동 실행
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "님이 입장했습니다!");
        UpdateRoomUI(); // UI 다시 갱신
    }

    // 방에 있던 다른 플레이어가 나갔을 때 자동 실행
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "님이 퇴장했습니다.");

        // 핵심: 나간 사람(otherPlayer)이 방장(MasterClient)이었는지 확인
        if (otherPlayer.IsMasterClient)
        {
            Debug.Log("방장이 나갔으므로 방을 폭파합니다. 로비로 돌아갑니다.");
            PhotonNetwork.LeaveRoom(); // 남아있던 게스트도 강제로 방에서 나감
        }
        else
        {
            // 나간 사람이 일반 클라이언트라면 방은 터지지 않음
            Debug.Log("일반 클라이언트가 나갔습니다. 새로운 상대를 기다립니다.");
            UpdateRoomUI(); // UI를 다시 "waiting opponent..." 상태로 갱신
        }
    }

    // 1. Exit 버튼을 클릭했을 때 실행할 함수 (유니티 OnClick에 연결할 녀석)
    public void OnClickExit()
    {
        Debug.Log("방을 나갑니다...");
        // 포톤 서버에 현재 방에서 나가겠다고 요청
        PhotonNetwork.LeaveRoom();
    }

    // 2. 서버에서 방 퇴장 처리가 완전히 끝났을 때 자동으로 호출되는 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("로비 화면으로 돌아갑니다.");
        // 다시 닉네임 입력하고 방 만드는 첫 번째 씬으로 이동 
        // ("LobbyScene" 부분은 네가 만든 첫 번째 씬의 정확한 이름으로 바꿔줘!)
        SceneManager.LoadScene("LobbyScene");
    }

    
}