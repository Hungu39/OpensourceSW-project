using UnityEngine;
using Photon.Pun;
using TMPro;

public class WaitingRoomChat : MonoBehaviourPun
{
    [Header("Chat UI")]
    public TextMeshProUGUI chatDisplay;
    public TMP_InputField chatInput;

    void Start()
    {
        chatDisplay.text = $"<b><color=#00FF00>[시스템]</color> {PhotonNetwork.NickName}님이 입장하셨습니다.</b>";

        // 💡 Update()를 지우고, 인풋필드에서 엔터키를 쳤을 때 자동으로 함수가 실행되도록 연결!
        if (chatInput != null)
        {
            chatInput.onSubmit.AddListener(delegate { SendChatMessage(); });
        }
    }

    public void SendChatMessage()
    {
        Debug.Log("1. 전송 함수 실행됨! (엔터키 인식 성공)");

        if (string.IsNullOrWhiteSpace(chatInput.text))
        {
            Debug.Log("2. 빈칸이라서 전송 취소됨.");
            return;
        }

        string message = chatInput.text;
        Debug.Log($"3. 보낼 메시지: {message}");

        chatInput.text = "";
        chatInput.ActivateInputField();

        if (photonView == null)
        {
            Debug.LogError("🚨 앗! 오브젝트에 Photon View 컴포넌트가 없습니다!");
            return;
        }

        photonView.RPC("RPC_ReceiveChat", RpcTarget.All, PhotonNetwork.NickName, message);
        Debug.Log("4. 서버로 RPC 무전 날림!");
    }

    [PunRPC]
    void RPC_ReceiveChat(string senderNickname, string message)
    {
        Debug.Log($"5. RPC 무전 받음! 보낸사람: {senderNickname}, 내용: {message}");
        chatDisplay.text += $"\n[{senderNickname}] {message}";
    }
}