using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AimingGameManager : MonoBehaviourPun
{
    [Header("Settings")]
    public int targetCount = 3;
    public float targetDuration = 3f;

    [Header("UI & Area")]
    public RectTransform spawnArea;
    public GameObject targetPrefab;

    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI timerText;

    [Header("Result UI")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultMessageText;

    private int hostScore = 0;
    private int guestScore = 0;
    private float gameTime = 30f;
    private bool isGameActive = false;

    void Start()
    {
        myScoreText.text = "My Score: 0";
        timerText.text = "WAITING...";

        if (resultPanel != null) resultPanel.SetActive(false);

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

        for (int i = 0; i < targetCount; i++)
        {
            SpawnTarget();
        }
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

        StartCoroutine(TargetLifeCoroutine(newTarget));
    }

    private IEnumerator TargetLifeCoroutine(GameObject target)
    {
        yield return new WaitForSeconds(targetDuration);

        if (target != null)
        {
            Destroy(target);
            SpawnTarget();
        }
    }

    void OnTargetClicked(GameObject clickedTarget)
    {
        if (!isGameActive) return;

        Destroy(clickedTarget);

        photonView.RPC("RPC_AddAimingScore", RpcTarget.All, PhotonNetwork.IsMasterClient);

        SpawnTarget();
    }

    [PunRPC]
    void RPC_AddAimingScore(bool isHostWhoScored)
    {
        if (isHostWhoScored) hostScore++;
        else guestScore++;

        if (PhotonNetwork.IsMasterClient)
        {
            myScoreText.text = $"MY SCORE: {hostScore}";
        }
        else
        {
            myScoreText.text = $"MY SCORE: {guestScore}";
        }
    }

    void DetermineWinner()
    {
        foreach (Transform child in spawnArea)
        {
            Destroy(child.gameObject);
        }

        string hostNick = GetHostNickName();
        string guestNick = GetGuestNickName();
        string winMessage = "";

        if (hostScore > guestScore) winMessage = $"👑 {hostNick} WIN! 👑";
        else if (guestScore > hostScore) winMessage = $"👑 {guestNick} WIN! 👑";
        else winMessage = "🤝 DRAW! 🤝";

        if (resultPanel != null && resultMessageText != null)
        {
            resultPanel.SetActive(true);
            resultMessageText.text = $"{winMessage}\n\n[{hostNick}] {hostScore}  VS  {guestScore} [{guestNick}]";
        }

        StartCoroutine(GoToTypingGame());
    }

    private IEnumerator GoToTypingGame()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("TypingGameScene");
        }
    }
}