using System.Collections.Generic;
using System.Threading;

public class EventVariableManager
{
    public static readonly EventVariableManager instance;

    static EventVariableManager()
    {
        instance = new EventVariableManager();
    }

    private readonly int mainThreadID;
    private readonly Queue<EventVariableBase> sideThreadEvents;
    private Queue<EventVariableBase> currentPushingEventStack;
    private bool running;

    private EventVariableManager()
    {
        sideThreadEvents = new Queue<EventVariableBase>();
        currentPushingEventStack = new Queue<EventVariableBase>();
        mainThreadID = Thread.CurrentThread.ManagedThreadId;
        running = false;
    }

    public void AddEvent(EventVariableBase eventVariableBase)
    {
        if (Thread.CurrentThread.ManagedThreadId == mainThreadID)
        {
            currentPushingEventStack.Enqueue(eventVariableBase);
            TriggerEvents();
        }
        else
        {
            sideThreadEvents.Enqueue(eventVariableBase);
        }
    }

    public void Update()
    {
        while (sideThreadEvents.Count > 0)
            AddEvent(sideThreadEvents.Dequeue());
    }

    private void TriggerEvents()
    {
        if (running)
            return;

        running = true;
        TriggerEvents(currentPushingEventStack);
        running = false;
    }

    private void TriggerEvents(Queue<EventVariableBase> events)
    {
        currentPushingEventStack = new Queue<EventVariableBase>();
        while (events.Count > 0)
        {
            EventVariableBase eventVariableBase = events.Dequeue();
            eventVariableBase.Trigger();
            if (currentPushingEventStack.Count > 0)
                TriggerEvents(currentPushingEventStack);
        }
    }
}
