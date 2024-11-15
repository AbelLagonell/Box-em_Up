public class GameStats {
    public int CurrentMultiplier;
    public float CPlayerHealth;
    public float MaxPlayerHealth;
    public float PlayTime;
    public int TotalScore;
    public int WaveCount;

    public GameStats() {
        CurrentMultiplier = 1;
        CPlayerHealth     = 10;
        MaxPlayerHealth   = 10;
        PlayTime          = 0;
        TotalScore        = 0;
        WaveCount         = 0;
    }
}