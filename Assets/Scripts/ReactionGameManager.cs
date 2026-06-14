using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ReactionGameManager : MonoBehaviourPun
{
    [Header("UI Elements")]
    public Image backgroundButtonImage;
    public TextMeshProUGUI centerInfoText;

    [Header("Result UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultMessageText;
    public TextMeshProUGUI hostResultText;
    public TextMeshProUGUI guestResultText;

    private int hostScore = 0;
    private int guestScore = 0;

    private bool isWaitingRed = false;
    private bool isRedScreen = false;

    private float startTime;
    private float hostTime = 0f;
    private float guestTime = 0f;
    private int submitCount = 0;

    void Start()
    {
        StartNewRound();
    }

    string GetHostNickName()
    {
        if (PhotonNetwork.MasterClient != null)
        {
            return PhotonNetwork.MasterClient.NickName;
        }
        return "HOST";
    }

    string GetGuestNickName()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.IsMasterClient)
            {
                return p.NickName;
            }
        }
        return "GUEST";
    }

    void StartNewRound()
    {
        isWaitingRed = true;
        isRedScreen = false;
        submitCount = 0;
        hostTime = 0f;
        guestTime = 0f;

        // 💡 게임 시작(대기) 시 결과창을 꺼줍니다.
        if (resultPanel != null) resultPanel.SetActive(false);

        backgroundButtonImage.color = Color.gray;
        centerInfoText.text = "WAIT...";

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
        isWaitingRed = false;
        isRedScreen = true;

        backgroundButtonImage.color = Color.red;
        centerInfoText.text = "CLICK!!!";

        startTime = Time.time;
    }

    public void OnScreenClicked()
    {
        if (isWaitingRed)
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

        // 여기서는 데이터만 저장합니다.
        if (isHost)
        {
            hostTime = time;
        }
        else
        {
            guestTime = time;
        }

        if (submitCount == 2)
        {
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        string hostNick = GetHostNickName();
        string guestNick = GetGuestNickName();
        string winMessage = "";

        if (hostTime < guestTime)
        {
            hostScore++;
            TotalScoreManager.hostTotalScore++;
            winMessage = $" {hostNick} WIN! ";
        }
        else if (guestTime < hostTime)
        {
            guestScore++;
            TotalScoreManager.guestTotalScore++;
            winMessage = $"{guestNick} WIN! ";
        }
        else
        {
            winMessage = "DRAW!";
        }

        // 💡 둘 다 클릭 완료 시 패널을 켜고, 텍스트에 결과를 뿌려줍니다.
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);

            if (resultMessageText != null)
                resultMessageText.text = winMessage;

            if (hostResultText != null)
                hostResultText.text = $"[{hostNick}]\n{Mathf.RoundToInt(hostTime * 1000f)}ms";

            if (guestResultText != null)
                guestResultText.text = $"[{guestNick}]\n{Mathf.RoundToInt(guestTime * 1000f)}ms";
        }

        StartCoroutine(NextRoundDelay());
    }

    private IEnumerator NextRoundDelay()
    {
        // 💡 결과창을 여유 있게 볼 수 있도록 5초 대기 후 에이밍 게임으로 이동
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("두 번째 게임: 에이밍 대결로 넘어갑니다!");
            PhotonNetwork.LoadLevel("AimingGameScene");
        }
    }
}