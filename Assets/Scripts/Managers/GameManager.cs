using Gameplay.Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager thisInstance;

    public TurnManager turnManager;
    public PlayerStats playerStats;
    
    public static GameManager Instance
    {
        get { return thisInstance ??= new GameManager(); }
    }
}
