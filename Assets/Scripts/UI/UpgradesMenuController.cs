using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Gameplay.Player;

[RequireComponent(typeof(UIDocument))]
public class UpgradesMenuController : MonoBehaviour
{
    public enum StatType { HP, LancePower, LandmineStrength, ExplosionSize }

    // Const amounts added per stat type upgrade
    private const int HP_UPGRADE = 25;
    private const float LANCE_POWER_UPGRADE = 5f;
    private const float LANDMINE_STRENGTH_UPGRADE = 10f;
    private const float EXPLOSION_SIZE_UPGRADE = 0.5f;

    private readonly Dictionary<Button, StatType> buttonStats = new Dictionary<Button, StatType>();

    private VisualElement root;
    private VisualElement cursorContainer;
    private VisualElement cursor;
    private Button hoveredButton;
    private bool chosen;

    /*
    ================
    ENABLE / DISABLE
    ================
    */

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        cursorContainer = root.Q<VisualElement>("CursorContainer");
        cursor = root.Q<VisualElement>("Cursor");

        buttonStats.Clear();
        buttonStats[root.Q<Button>("HPButton")] = StatType.HP;
        buttonStats[root.Q<Button>("LancePowerButton")] = StatType.LancePower;
        buttonStats[root.Q<Button>("LandmineStrengthButton")] = StatType.LandmineStrength;
        buttonStats[root.Q<Button>("ExplosionSizeButton")] = StatType.ExplosionSize;
        

        foreach (KeyValuePair<Button, StatType> pair in buttonStats)
        {
            pair.Key.RegisterCallback<ClickEvent>(OnButtonClicked);
            pair.Key.RegisterCallback<PointerEnterEvent>(OnButtonHovered);
        }

        chosen = false;
        hoveredButton = root.Q<Button>("HPButton");
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnDisable()
    {
        foreach (KeyValuePair<Button, StatType> pair in buttonStats)
        {
            pair.Key.UnregisterCallback<ClickEvent>(OnButtonClicked);
            pair.Key.UnregisterCallback<PointerEnterEvent>(OnButtonHovered);
        }
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
    
    /*
    ===================
    BUTTON INPUT EVENTS
    ===================
    */

    private void OnButtonClicked(ClickEvent evt)
    {
        if (chosen) { return; }
        if (SceneTransitionManager.Instance.isTransitioning) { return; }

        Button button = evt.currentTarget as Button;
        if (button == null || !buttonStats.TryGetValue(button, out StatType statType)) { return; }
        
        chosen = true;
        ApplyUpgrade(statType);
        GameManager.Instance.AdvanceLevel();
        SceneTransitionManager.Instance.ChangeScene(GameManager.NextSceneName());
    }

    private void OnButtonHovered(PointerEnterEvent evt)
    {
        hoveredButton = evt.currentTarget as Button;
        MoveCursorTo(hoveredButton);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        MoveCursorTo(hoveredButton);
    }

    /*
    =============
    UPGRADE LOGIC
    =============
    */
    
    private void ApplyUpgrade(StatType statType)
    {

        PlayerStatsData stats = GameManager.Instance.savedStats;
        if (stats == null)
        {
            Debug.LogError("No saved player stats found. Start from a battle scene first.");
            return;
        }

        switch (statType)
        {
            case StatType.HP:
                stats.hp += HP_UPGRADE;
                break;
            
            case StatType.LancePower:
                stats.damage += LANCE_POWER_UPGRADE;
                break;
            
            case StatType.LandmineStrength:
                stats.landmineStrength += LANDMINE_STRENGTH_UPGRADE;
                break;

            case StatType.ExplosionSize:
                stats.explosionSize += EXPLOSION_SIZE_UPGRADE;
                break;
        }
    }

    /*
    ============
    CURSOR LOGIC
    ============
    */

    private void MoveCursorTo(Button target)
    {
        if (target == null || cursor == null || cursorContainer == null ) { return; }
        if (float.IsNaN(target.layout.height) || float.IsNaN(target.worldBound.height)) { return; }
    
        float targetCenterY = cursorContainer.WorldToLocal(target.worldBound.center).y;

        float cursorCenterY = cursor.layout.y + cursor.layout.height / 2f;

        cursor.style.translate = new Translate(0f, targetCenterY - cursorCenterY);
    }

}
