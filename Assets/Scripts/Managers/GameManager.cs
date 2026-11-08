using Gameplay.Player;
using UnityEngine;

public class GameManager
{
    public enum Level { Level1, Level2, Level3, Victory }

    private const string UPGRADES_SCENE = "UpgradesMenuController";

    static GameManager thisInstance;

    public TurnManager turnManager;
    public PlayerStats playerStats;
    public PlayerStatsData savedStats;
    public Level currentLevel = Level.Level1;
    public bool roundOver;
    
    
    public static GameManager Instance
    {
        get { return thisInstance ??= new GameManager(); }
    }

    public static string NextSceneName()
    {
        switch (Instance.currentLevel)
        {
            case Level.Level1: return "SampleScene";
            case Level.Level2: return "SampleScene";
            case Level.Level3: return "SampleScene";
            default: return "MainMenu";
        }
    }

    public int CurrentEnemyIndex()
    {
        switch (currentLevel)
        {
            case Level.Level1: return 0;
            case Level.Level2: return 1;
            case Level.Level3: return 2;
            default: return 0;
        }
    }

    public void AdvanceLevel()
    {
        if (currentLevel < Level.Victory) { currentLevel++; }
    }

    public void ResetRun()
    {
        savedStats = null;
        currentLevel = Level.Level1;
        
    }

    public void EndRound(bool PlayerWon)
    {
        if (roundOver) { return; }
        roundOver = true;

        Time.timeScale = 1;

        if (PlayerWon)
        {
            SceneTransitionManager.Instance.ChangeScene(UPGRADES_SCENE);
        }
        else
        {
            SceneTransitionManager.Instance.ChangeScene(NextSceneName());
        }
    }
    
}
