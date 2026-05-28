using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypingGameManager : MonoBehaviourPun
{
    [Header("UI References")]
    public TextMeshProUGUI[] wordCells;
    public Image[] cellBackgrounds;
    public Image timerBar;
    public TMP_InputField myInputField;

    [Header("Host (RED) UI")]
    public TextMeshProUGUI hostNameText;
    public TextMeshProUGUI hostScoreText;
    public TextMeshProUGUI hostResultText;

    [Header("Guest (BLUE) UI")]
    public TextMeshProUGUI guestNameText;
    public TextMeshProUGUI guestScoreText;
    public TextMeshProUGUI guestResultText;

    private string[] wordBank = {
        "unity", "photon", "network", "multiplay", "opensource",
        "computer", "script", "canvas", "player", "game", "object", "transform", "vector",
        "update", "start", "awake", "component", "rigidbody", "collider", "prefab",
        "algorithm", "array", "list", "stack", "queue", "tree", "graph", "hash", "sort",
        "class", "struct", "interface", "delegate", "event", "lambda", "thread", "async",
        "integer", "float", "double", "string", "boolean", "variable", "function", "return",
        "physics", "material", "shader", "texture", "camera", "lighting", "animation", "audio",
        "server", "client", "packet", "latency", "protocol", "socket", "database", "query",
        "debug", "build", "compile", "error", "exception", "memory", "pointer", "reference"
    };

    private string[] currentWords;
    private int[] cellOwners;

    private int hostScore = 25;
    private int guestScore = 25;

    private float gameTime = 60f;
    private bool isGameActive = false;

    void Start()
    {
        hostNameText.text = $"{GetHostNickName()}\n(RED)";
        guestNameText.text = $"{GetGuestNickName()}\n(BLUE)";

        if (hostResultText != null) hostResultText.text = "";
        if (guestResultText != null) guestResultText.text = "";

        if (myInputField != null) myInputField.interactable = true;

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnInitialWords();
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

    void SpawnInitialWords()
    {
        string[] initialWords = new string[wordCells.Length];
        int[] initialOwners = new int[wordCells.Length];

        List<string> shuffledBank = new List<string>(wordBank);
        for (int i = 0; i < shuffledBank.Count; i++)
        {
            string temp = shuffledBank[i];
            int randomIndex = Random.Range(i, shuffledBank.Count);
            shuffledBank[i] = shuffledBank[randomIndex];
            shuffledBank[randomIndex] = temp;
        }

        for (int i = 0; i < wordCells.Length; i++)
        {
            initialWords[i] = shuffledBank[i];

            // 💡 세로 분할의 마법! 
            // 가로가 10칸이므로, 10으로 나눈 나머지가 0~4(왼쪽 절반)면 방장(0), 5~9(오른쪽 절반)면 게스트(1)의 땅으로 줍니다.
            initialOwners[i] = ((i % 10) < 5) ? 0 : 1;
        }

        photonView.RPC("RPC_SyncInitialWords", RpcTarget.All, initialWords, initialOwners);
    }

    [PunRPC]
    void RPC_SyncInitialWords(string[] words, int[] owners)
    {
        currentWords = words;
        cellOwners = owners;

        hostScore = 0;
        guestScore = 0;

        for (int i = 0; i < wordCells.Length; i++)
        {
            wordCells[i].text = currentWords[i];

            if (cellOwners[i] == 0)
            {
                cellBackgrounds[i].color = Color.red;
                hostScore++;
            }
            else
            {
                cellBackgrounds[i].color = Color.blue;
                guestScore++;
            }
        }

        hostScoreText.text = hostScore.ToString();
        guestScoreText.text = guestScore.ToString();

        isGameActive = true;
        StartCoroutine(GameTimerCoroutine());
    }

    private IEnumerator GameTimerCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        float currentTime = gameTime;
        while (currentTime > 0)
        {
            if (timerBar != null)
            {
                timerBar.fillAmount = currentTime / gameTime;
            }

            yield return null;
            currentTime -= Time.deltaTime;

            if (hostScore >= wordCells.Length || guestScore >= wordCells.Length)
            {
                break;
            }
        }

        isGameActive = false;
        if (timerBar != null) timerBar.fillAmount = 0f;

        DetermineWinner();
    }

    public void OnInputSubmit(string inputText)
    {
        if (!isGameActive) return;

        string cleanInput = inputText.Replace("\u200B", "").Trim().ToLower();
        bool isHost = PhotonNetwork.IsMasterClient;

        int targetIndex = -1;

        for (int i = 0; i < currentWords.Length; i++)
        {
            if (currentWords[i] == cleanInput)
            {
                if ((isHost && cellOwners[i] == 1) || (!isHost && cellOwners[i] == 0))
                {
                    targetIndex = i;
                    break;
                }
            }
        }

        if (targetIndex != -1)
        {
            string newRandomWord = wordBank[Random.Range(0, wordBank.Length)];
            photonView.RPC("RPC_StealWord", RpcTarget.All, targetIndex, newRandomWord, isHost);
        }

        StartCoroutine(ResetInputFieldCoroutine());
    }

    [PunRPC]
    void RPC_StealWord(int cellIndex, string newWord, bool isHostWhoStole)
    {
        if (isHostWhoStole && cellOwners[cellIndex] == 0) return;
        if (!isHostWhoStole && cellOwners[cellIndex] == 1) return;

        if (isHostWhoStole)
        {
            cellOwners[cellIndex] = 0;
            cellBackgrounds[cellIndex].color = Color.red;
            hostScore++;
            guestScore--;
        }
        else
        {
            cellOwners[cellIndex] = 1;
            cellBackgrounds[cellIndex].color = Color.blue;
            guestScore++;
            hostScore--;
        }

        currentWords[cellIndex] = newWord;
        wordCells[cellIndex].text = newWord;

        hostScoreText.text = hostScore.ToString();
        guestScoreText.text = guestScore.ToString();
    }

    void DetermineWinner()
    {
        if (hostScore > guestScore)
        {
            if (hostResultText != null) { hostResultText.text = "WIN"; hostResultText.color = Color.yellow; }
            if (guestResultText != null) { guestResultText.text = "LOSE"; guestResultText.color = Color.gray; }
        }
        else if (guestScore > hostScore)
        {
            if (hostResultText != null) { hostResultText.text = "LOSE"; hostResultText.color = Color.gray; }
            if (guestResultText != null) { guestResultText.text = "WIN"; guestResultText.color = Color.yellow; }
        }
        else
        {
            if (hostResultText != null) { hostResultText.text = "DRAW"; hostResultText.color = Color.white; }
            if (guestResultText != null) { guestResultText.text = "DRAW"; guestResultText.color = Color.white; }
        }

        if (myInputField != null) myInputField.interactable = false;

        StartCoroutine(EndAllGames());
    }

    private IEnumerator ResetInputFieldCoroutine()
    {
        yield return null;
        if (myInputField != null)
        {
            myInputField.text = "";
            myInputField.ActivateInputField();
        }
    }

    private IEnumerator EndAllGames()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }
}