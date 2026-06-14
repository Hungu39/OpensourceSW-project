using UnityEngine;

public static class TotalScoreManager
{
    public static int hostTotalScore = 0;
    public static int guestTotalScore = 0;

    public static void ResetScores()
    {
        hostTotalScore = 0;
        guestTotalScore = 0;
    }
}