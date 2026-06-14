using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime; // 💡 Player 목록을 뒤져서 닉네임을 찾기 위해 꼭 필요해!
using System.Collections;

public class FinalResultManager : MonoBehaviour
{
    [Header("Final UI")]
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI scoreText;

    // 대기방으로 돌아가기 전 띄워둘 시간
    public float Delay = 5f;

    void Start()
    {
        // 1. 방장과 게스트의 닉네임 불러오기
        string hostNick = GetHostNickName();
        string guestNick = GetGuestNickName();

        // 2. TotalScoreManager에 저장된 최종 점수 불러오기
        int finalHost = TotalScoreManager.hostTotalScore;
        int finalGuest = TotalScoreManager.guestTotalScore;

        // 3. 점수 텍스트에 닉네임 적용
        scoreText.text = $"{hostNick}  <color=#FFFF00>{finalHost}</color> : <color=#FFFF00>{finalGuest}</color>  {guestNick}";

        // 4. 최종 승자 텍스트에 닉네임 적용
        if (finalHost > finalGuest)
        {
            winnerText.text = $"{hostNick} WIN!";
        }
        else if (finalGuest > finalHost)
        {
            winnerText.text = $"{guestNick} WIN!";
        }
        else
        {
            winnerText.text = " DRAW... ";
        }

        // 씬이 시작되자마자 자동 복귀 타이머 작동
        StartCoroutine(ReturnToWaitingRoom());
    }

    // 💡 방장 닉네임 찾는 함수
    string GetHostNickName()
    {
        if (PhotonNetwork.MasterClient != null)
        {
            return PhotonNetwork.MasterClient.NickName;
        }
        return "HOST";
    }

    // 💡 게스트 닉네임 찾는 함수
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

    private IEnumerator ReturnToWaitingRoom()
    {
        // 설정한 시간(returnDelay)만큼 결과창 보여주기
        yield return new WaitForSeconds(Delay);

        // 다음 게임을 위해 누적된 static 최종 점수 0으로 초기화
        TotalScoreManager.ResetScores();

        // 방장(MasterClient)만 씬을 이동시켜서 다 같이 대기실로 복귀
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("WaitingRoomScene");
        }
    }
}