using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nicknameInputField;
    public GameObject ErrorPopup;
    public GameObject ErrorPopup2;

    void Start()
    {
        if(ErrorPopup != null)
            ErrorPopup.SetActive(false);
        if (ErrorPopup2 != null)
            ErrorPopup2.SetActive(false);

        Debug.Log("포톤 마스터 서버에 접속을 시도합니다.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(); 
    }

    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 성공. 로비로 입장합니다.");
        PhotonNetwork.JoinLobby();
    }

    public void OnClickCreateRoom() 
    {
        if(string.IsNullOrEmpty(nicknameInputField.text))
        {
            if(ErrorPopup != null)
                ErrorPopup.SetActive(true);
            Debug.Log("닉네임을 먼저 입력해주세요");
            return;
        }

        PhotonNetwork.NickName = nicknameInputField.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        Debug.Log("방 생성을 요청합니다...");
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public void OnClickJoinRoom()
    {
        if (string.IsNullOrEmpty(nicknameInputField.text))
        {
            if (ErrorPopup != null) 
                ErrorPopup.SetActive(true);
            Debug.Log("닉네임을 먼저 입력해주세요");
            return;
        }

        PhotonNetwork.NickName = nicknameInputField.text;

        Debug.Log("빈 방 참가를 시도합니다...");
        PhotonNetwork.JoinRandomRoom();
    }

    public void CloseWarningPopup()
    {
        if (ErrorPopup != null)
            ErrorPopup.SetActive(false);
        if (ErrorPopup2 != null)
            ErrorPopup2.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공! 현재 방 인원: " + PhotonNetwork.CurrentRoom.PlayerCount + "명");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("WaitingRoomScene");  
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (ErrorPopup2 != null)
            ErrorPopup2.SetActive(true);
        Debug.Log("⚠️ 참가할 수 있는 빈 방이 없습니다. 새로 방을 만들어주세요.");
    }
}
