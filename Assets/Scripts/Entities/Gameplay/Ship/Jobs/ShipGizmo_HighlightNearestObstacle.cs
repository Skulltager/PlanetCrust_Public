
using Unity.Entities;
using UnityEngine;

public class ShipGizmo_HighlightNearestObstacle : MonoBehaviour
{
    private ShipSystem_AI_HighlightNearestObstacle highlightsNearestObstacleSystem;

    private void Awake()
    {
        highlightsNearestObstacleSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ShipSystem_AI_HighlightNearestObstacle>();
    }

    private void OnDrawGizmos()
    {
        if (highlightsNearestObstacleSystem == null)
            return;

        for (int i = 0; i < highlightsNearestObstacleSystem.pointsToHighlight.Length - 1; i++)
        {
            Vector3 fromPoint = highlightsNearestObstacleSystem.pointsToHighlight[i];
            Vector3 toPoint = highlightsNearestObstacleSystem.pointsToHighlight[i + 1];
            if (toPoint == Vector3.zero)
            {
                i++;
                continue;
            }

            Gizmos.DrawLine(fromPoint, toPoint);
        }
    }
}