
using UnityEngine;
using UnityEngine.UI;
using SheetCodes;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance { private set; get; }

    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private Text messageText = default;
    [SerializeField] private Button okButton = default;

    private readonly EventVariable<PopupManager, bool> isVisible;
    private readonly Queue<PopupMessageIdentifier> queuedPopups;

    private PopupManager()
    {
        isVisible = new EventVariable<PopupManager, bool>(this, false);
        queuedPopups = new Queue<PopupMessageIdentifier>();
    }

    private void Awake()
    {
        instance = this;
        okButton.onClick.AddListener(OnPress_OKButton);
        isVisible.onValueChangeImmediate += OnValueChanged_IsVisible;
    }

    private void OnValueChanged_IsVisible(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            canvasGroup.Show();
        }
        else
        {
            canvasGroup.Hide();
        }
    }

    public void QueueMessage(PopupMessageIdentifier identifier)
    {
        Debug.Log(identifier);
        if(isVisible.value)
        {
            queuedPopups.Enqueue(identifier);
            return;
        }

        isVisible.value = true;
        messageText.text = identifier.GetRecord().Message;
    }

    private void OnPress_OKButton()
    {
        if(queuedPopups.Count > 0)
        {
            PopupMessageIdentifier identifier = queuedPopups.Dequeue();
            messageText.text = identifier.GetRecord().Message;
            return;
        }

        isVisible.value = false;
    }

    private void OnDestroy()
    {
        okButton.onClick.RemoveListener(OnPress_OKButton);
        isVisible.onValueChange -= OnValueChanged_IsVisible;
    }
}