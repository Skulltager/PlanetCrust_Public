
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { private set; get; }
    [SerializeField] private LevelBoundsEntities levelBounds = default;

    public LevelBoundsEntities LevelBounds => levelBounds;

    private LevelPreparation[] levelPreparations;

    private void Awake()
    {
        instance = this;
        levelPreparations = GetComponentsInChildren<LevelPreparation>();
    }

    public void Initialize()
    {
        foreach (LevelPreparation levelPreparation in levelPreparations)
            levelPreparation.Initialize(levelBounds);
    }
}