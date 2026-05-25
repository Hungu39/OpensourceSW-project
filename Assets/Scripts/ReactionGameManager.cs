using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // 💡 Player 클래스를 쓰기 위해 추가
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ReactionGameManager : MonoBehaviourPun
{
    [Header("UI Elements")]
    public Image backgroundButtonImage;
    public TextMeshProUGUI centerInfoText;

    public TextMeshProUGUI hostInfoText;  // 방장 전광판 (닉네임 표시)
    public TextMeshProUGUI guestInfoText; // 게스트 전광판 (닉네임 표시)

    private int hostScore = 0;
    private int guestScore = 0;

    private bool isWaitingForRed = false;
    private bool isRedScreen = false;

    private float startTime;
    private float hostTime = 0f;
    private float guestTime = 0f;
    private int submitCount = 0;

    void Start()
    {
        StartNewRound();
    }

    // 💡 방장의 실제 닉네임을 가져오는 함수
    string GetHostNickName()
    {
        if (PhotonNetwork.MasterClient != null)
        {
            return PhotonNetwork.MasterClient.NickName;
        }
        return "HOST";
    }

    // 💡 게스트(방장이 아닌 사람)의 실제 닉네임을 가져오는 함수
    string GetGuestNickName()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 방장이 아닌 플레이어를 찾으면 그 사람의 닉네임 반환
            if (!p.IsMasterClient)
            {
                return p.NickName;
            }
        }
        return "GUEST"; // 아직 게스트가 안 들어왔거나 못 찾았을 때 대치어
    }

    void StartNewRound()
    {
        isWaitingForRed = true;
        isRedScreen = false;
        submitCount = 0;
        hostTime = 0f;
        guestTime = 0f;

        backgroundButtonImage.color = Color.gray;
        centerInfoText.text = "WAIT...";

        // 라운드 시작할 때도 닉네임과 현재 점수를 미리 띄워줌
        hostInfoText.text = $"{GetHostNickName()}\nScore: {hostScore}";
        guestInfoText.text = $"{GetGuestNickName()}\nScore: {guestScore}";

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(RandomWaitCoroutine());
        }
    }

    private IEnumerator RandomWaitCoroutine()
    {
        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);
        photonView.RPC("RPC_TurnRed", RpcTarget.All);
    }

    [PunRPC]
    void RPC_TurnRed()
    {
        isWaitingForRed = false;
        isRedScreen = true;

        backgroundButtonImage.color = Color.red;
        centerInfoText.text = "CLICK!!!";

        startTime = Time.time;
    }

    public void OnScreenClicked()
    {
        if (isWaitingForRed)
        {
            Debug.Log("FAIL START!");
            return;
        }

        if (isRedScreen)
        {
            isRedScreen = false;
            float myReactionTime = Time.time - startTime;
            photonView.RPC("RPC_SubmitTime", RpcTarget.All, myReactionTime, PhotonNetwork.IsMasterClient);
        }
    }

    [PunRPC]
    void RPC_SubmitTime(float time, bool isHost)
    {
        submitCount++;

        // 💡 "Host:", "Guest:" 텍스트 대신 GetHostNickName(), GetGuestNickName() 함수를 넣어줬어!
        if (isHost)
        {
            hostTime = time;
            hostInfoText.text = $"{GetHostNickName()}: {hostTime:F3}초\nScore: {hostScore}";
        }
        else
        {
            guestTime = time;
            guestInfoText.text = $"{GetGuestNickName()}: {guestTime:F3}초\nScore: {guestScore}";
        }

        if (submitCount == 2)
        {
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        // 💡 승리 문구에도 실제 닉네임이 나오도록 변경
        if (hostTime < guestTime)
        {
            hostScore++;
            centerInfoText.text = $"{GetHostNickName()} WIN!";
        }
        else if (guestTime < hostTime)
        {
            guestScore++;
            centerInfoText.text = $"{GetGuestNickName()} WIN!";
        }
        else
        {
            centerInfoText.text = "DRAW!";
        }

        // 최종 스코어 갱신
        hostInfoText.text = $"{GetHostNickName()}: {hostTime:F3}\nScore: {hostScore}";
        guestInfoText.text = $"{GetGuestNickName()}: {guestTime:F3}\nScore: {guestScore}";

        StartCoroutine(NextRoundDelay());
    }

    private IEnumerator NextRoundDelay()
    {
        // 결과를 3초 동안 띄워둠
        yield return new WaitForSeconds(3f);

        // 💡 방장만 다음 씬으로 모두를 데려감
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("두 번째 게임: 에이밍 대결로 넘어갑니다!");
            PhotonNetwork.LoadLevel("AimingGameScene"); // 실제 에이밍 씬 이름으로 변경!
        }
    }   
}