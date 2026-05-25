using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AimingGameManager : MonoBehaviourPun
{
    [Header("UI & Area")]
    public RectTransform spawnArea;
    public GameObject targetPrefab;

    // 💡 실시간으로는 우측 상단에 내 점수만 보여줄 텍스트 하나만 사용!
    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI timerText;

    private int hostScore = 0;
    private int guestScore = 0;
    private float gameTime = 30f;
    private bool isGameActive = false;

    void Start()
    {
        // 시작할 때는 무조건 내 점수 0점으로 표시
        myScoreText.text = "My Score: 0";
        timerText.text = "WAITING...";

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_StartAimingGame", RpcTarget.All);
        }
    }

    string GetHostNickName()
    {
        return PhotonNetwork.MasterClient != null ? PhotonNetwork.MasterClient.NickName : "방장";
    }

    string GetGuestNickName()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.IsMasterClient) return p.NickName;
        }
        return "상대방";
    }

    [PunRPC]
    void RPC_StartAimingGame()
    {
        isGameActive = true;
        StartCoroutine(GameTimerCoroutine());
        SpawnTarget();
    }

    private IEnumerator GameTimerCoroutine()
    {
        float currentTime = gameTime;
        while (currentTime > 0)
        {
            timerText.text = $"TIME: {Mathf.CeilToInt(currentTime)}";
            yield return null;
            currentTime -= Time.deltaTime;
        }

        isGameActive = false;

        // 💡 타이머가 끝나면 결과를 계산하고 화면에 뿌림
        DetermineWinner();
    }

    void SpawnTarget()
    {
        if (!isGameActive) return;

        GameObject newTarget = Instantiate(targetPrefab, spawnArea);

        RectTransform targetRect = newTarget.GetComponent<RectTransform>();
        float randomX = Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2);
        float randomY = Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2);

        targetRect.anchoredPosition = new Vector2(randomX, randomY);

        Button targetBtn = newTarget.GetComponent<Button>();
        targetBtn.onClick.AddListener(() => OnTargetClicked(newTarget));
    }

    void OnTargetClicked(GameObject clickedTarget)
    {
        if (!isGameActive) return;

        Destroy(clickedTarget);

        // 내가 점수를 얻었다는 신호를 보냄 (방장 여부 패러메터 포함)
        photonView.RPC("RPC_AddAimingScore", RpcTarget.All, PhotonNetwork.IsMasterClient);

        SpawnTarget();
    }

    [PunRPC]
    void RPC_AddAimingScore(bool isHostWhoScored)
    {
        // 1. 내부적으로 데이터는 둘 다 계속 쌓아둠
        if (isHostWhoScored) hostScore++;
        else guestScore++;

        // 2. 💡 화면 UI에는 '내 화면 기준' 나의 점수만 실시간 갱신!
        if (PhotonNetwork.IsMasterClient)
        {
            myScoreText.text = $"MY SCORE: {hostScore}";
        }
        else
        {
            myScoreText.text = $"MY SCORE: {guestScore}";
        }
    }

    // 💡 게임이 종료되었을 때만 호출되는 결과 공개 함수
    void DetermineWinner()
    {
        // 화면에 남아있는 표적 전부 치우기
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }

        string hostNick = GetHostNickName();
        string guestNick = GetGuestNickName();
        string winMessage = "";

        // 1. 승패 판정 문구 작성
        if (hostScore > guestScore) winMessage = $"👑 {hostNick} WIN! 👑";
        else if (guestScore > hostScore) winMessage = $"👑 {guestNick} WIN! 👑";
        else winMessage = "🤝 DRAW! 🤝";

        // 2. 중앙 타이머 텍스트를 결과창 전광판으로 재활용하여 대공개
        timerText.text = $"{winMessage}\n\n[{hostNick}] {hostScore}  VS  {guestScore} [{guestNick}]";

        // 💡 3초 뒤에 타자 게임으로 넘어가는 코루틴 실행
        StartCoroutine(GoToTypingGame());
    }

    // 💡 다음 씬(타자 게임)으로 이동하는 코루틴
    private IEnumerator GoToTypingGame()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
        {
            // 괄호 안의 이름은 실제 유니티에 저장된 타자 게임 씬 이름이어야 해!
            PhotonNetwork.LoadLevel("TypingGameScene");
        }
    }
}