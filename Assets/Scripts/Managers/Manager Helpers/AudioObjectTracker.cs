
using FMODUnity;
using UnityEngine;

public class AudioObjectTracker : MonoBehaviour
{
    public static AudioObjectTracker instance { private set; get; }
    public readonly EventVariable<AudioObjectTracker, Transform> followRotation;
    public readonly EventVariable<AudioObjectTracker, Transform> followPosition;

    private AudioObjectTracker()
    {
        followPosition = new EventVariable<AudioObjectTracker, Transform>(this, null);
        followRotation = new EventVariable<AudioObjectTracker, Transform>(this, null);
    }

    private void Awake()
    {
        instance = this;
        followPosition.onValueChange += OnValueChanged_FollowPosition;
        followRotation.onValueChange += OnValueChanged_FollowRotation;
    }

    private void OnValueChanged_FollowPosition(Transform oldValue, Transform newValue)
    {
        if (newValue != null)
            transform.position = newValue.position;
    }

    private void OnValueChanged_FollowRotation(Transform oldValue, Transform newValue)
    {
        if (newValue != null)
            transform.rotation = newValue.rotation;
    }

    private void LateUpdate()
    {
        if (followPosition.value != null)
            transform.position = followRotation.value.position;

        if (followRotation.value != null)
            transform.rotation = followRotation.value.rotation;
    }

    private void OnDestroy()
    {
        followPosition.onValueChange -= OnValueChanged_FollowPosition;
        followRotation.onValueChange -= OnValueChanged_FollowRotation;
    }
}