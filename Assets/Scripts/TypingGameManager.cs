using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // 💡 Player 클래스와 닉네임 정보를 쓰기 위해 반드시 추가!
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypingGameManager : MonoBehaviourPun
{
    [Header("UI References")]
    public TextMeshProUGUI[] wordCells;

    [Header("Host (Left) UI")]
    public TMP_InputField hostInputField;
    public TextMeshProUGUI hostScoreText;

    [Header("Guest (Right) UI")]
    public TMP_InputField guestInputField;
    public TextMeshProUGUI guestScoreText;

    private string[] wordBank = { "unity", "photon", "network", "multiplay", "opensource",
                                "computer", "script", "canvas", "player", "game", "object", "transform", "vector",
                                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"};

    private List<string> activeWords = new List<string>();

    private int hostScore = 0;
    private int guestScore = 0;

    void Start()
    {
        SetupMySide();

        // 💡 시작할 때 'Host', 'Guest' 대신 실제 닉네임으로 점수판 세팅
        hostScoreText.text = $"{GetHostNickName()}: 0";
        guestScoreText.text = $"{GetGuestNickName()}: 0";

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnInitialWords();
        }
    }

    // 💡 방장의 실제 닉네임을 가져오는 함수
    string GetHostNickName()
    {
        if (PhotonNetwork.MasterClient != null)
        {
            return PhotonNetwork.MasterClient.NickName;
        }
        return "방장";
    }

    // 💡 게스트(방장이 아닌 사람)의 실제 닉네임을 가져오는 함수
    string GetGuestNickName()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.IsMasterClient) return p.NickName;
        }
        return "상대방";
    }

    void SetupMySide()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            hostInputField.interactable = true;
            guestInputField.interactable = false;
        }
        else
        {
            guestInputField.interactable = true;
            hostInputField.interactable = false;
        }
    }

    void SpawnInitialWords()
    {
        List<int> randomIndices = new List<int>();

        for (int i = 0; i < wordCells.Length; i++)
        {
            randomIndices.Add(Random.Range(0, wordBank.Length));
        }

        photonView.RPC("RPC_SyncInitialWords", RpcTarget.All, randomIndices.ToArray());
    }

    [PunRPC]
    void RPC_SyncInitialWords(int[] indices)
    {
        activeWords.Clear();

        for (int i = 0; i < indices.Length; i++)
        {
            string pickedWord = wordBank[indices[i]];
            activeWords.Add(pickedWord);
            wordCells[i].text = pickedWord;
        }
    }

    public void OnInputSubmit(string inputText)
    {
        string cleanInput = inputText.Replace("\u200B", "").Trim().ToLower();

        if (activeWords.Contains(cleanInput))
        {
            photonView.RPC("RPC_RemoveWord", RpcTarget.All, cleanInput, PhotonNetwork.IsMasterClient);
        }

        StartCoroutine(ResetInputFieldCoroutine());
    }

    [PunRPC]
    void RPC_RemoveWord(string wordToRemove, bool isHostWhoWon)
    {
        if (!activeWords.Contains(wordToRemove)) return;

        activeWords.Remove(wordToRemove);

        for (int i = 0; i < wordCells.Length; i++)
        {
            if (wordCells[i].text == wordToRemove)
            {
                wordCells[i].text = "";
                break;
            }
        }

        // 💡 점수와 함께 실제 닉네임이 업데이트되도록 수정!
        if (isHostWhoWon)
        {
            hostScore++;
            hostScoreText.text = $"{GetHostNickName()}: {hostScore}";
            Debug.Log($"[{GetHostNickName()}] 획득: {wordToRemove}");
        }
        else
        {
            guestScore++;
            guestScoreText.text = $"{GetGuestNickName()}: {guestScore}";
            Debug.Log($"[{GetGuestNickName()}] 획득: {wordToRemove}");
        }

        if (activeWords.Count == 0)
        {
            Debug.Log("모든 단어가 소진되었습니다! 릴레이 게임 끝!");
            StartCoroutine(EndAllGames());
        }
    }

    private IEnumerator ResetInputFieldCoroutine()
    {
        yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            hostInputField.text = "";
            hostInputField.ActivateInputField();
        }
        else
        {
            guestInputField.text = "";
            guestInputField.ActivateInputField();
        }
    }

    private IEnumerator EndAllGames()
    {
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }
}