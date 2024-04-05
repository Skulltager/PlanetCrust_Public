
using System.Collections;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    public static ManagerController instance { private set; get; }

    [SerializeField] private ControlManager controlManager = default;

    [SerializeField] private PrefabPoolManager prefabPoolManager = default;
    [SerializeField] private ParticleEmitManager particleEmitManager = default;
    [SerializeField] private EnemyHealthBarManager enemyHealthBarManager = default;
    [SerializeField] private WeaponIndicatorManager weaponIndicatorManager = default;
    [SerializeField] private HUD hud = default;
    [SerializeField] private PauseMenuUI pauseMenuUI = default;

    [SerializeField] private TransformCalculationHelper transformCalculationHelper = default;

    public bool gamePaused => Time.timeScale == 0;

    private void Awake()
    {
        instance = this;

        transformCalculationHelper.Initialize();
        controlManager.Initialize();
        hud.Initialize();

        prefabPoolManager.Initialize();
        enemyHealthBarManager.Initialize();
        particleEmitManager.Initialize();
    }

    public IEnumerator Routine_Initialize()
    {
        while (LevelManager.instance == null)
            yield return null;

        LevelBoundsEntities levelBounds = LevelManager.instance.LevelBounds;
        levelBounds.Initialize();

        prefabPoolManager.Reset();
        particleEmitManager.Reset();

        enemyHealthBarManager.Reset();
        pauseMenuUI.ResetUI();

        LevelManager.instance.Initialize();
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PathFindingSystem>().Initialize(levelBounds);
    }

    private void FixedUpdate()
    {
        if (gamePaused)
            return;

        weaponIndicatorManager.UpdateIndicators();
    }
}