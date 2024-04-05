
using Unity.Entities;
using UnityEngine;

public class ShipGizmo_HighlightPaths : MonoBehaviour
{
    private ShipSystem_AI_HighlightPaths highlightsPathsSystem;
    private void Awake()
    {
        highlightsPathsSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ShipSystem_AI_HighlightPaths>();
    }

    private void OnDrawGizmos()
    {
        if (highlightsPathsSystem == null)
            return;

        for (int i = 0; i < highlightsPathsSystem.pointsToHighlight.Length - 1; i++)
        {
            Vector3 fromPoint = highlightsPathsSystem.pointsToHighlight[i];
            Vector3 toPoint = highlightsPathsSystem.pointsToHighlight[i + 1];
            if (toPoint == Vector3.zero)
            {
                i++;
                continue;
            }

            Gizmos.DrawLine(fromPoint, toPoint);
        }
    }
}