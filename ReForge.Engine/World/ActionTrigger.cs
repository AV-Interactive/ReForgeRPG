namespace ReForge.Engine.World;
using ReForge.Engine.Core;

public class ActionTrigger: Behavior
{
    public List<ActionCommand> OnEnterActions { get; set; } = new();
    public List<ActionCommand> OnExitActions { get; set; } = new();
    
    public override void Update(float deltaTime)
    {
        
    }

    public override Behavior Clone()
    {
        return new ActionTrigger
        {
            OnEnterActions = this.OnEnterActions,
            OnExitActions = this.OnExitActions
        };
    }

    public override void OnReceivedEvent(string eventName, object? data = null)
    {
        if (eventName == "OnCollisionEnter")
        {
            foreach (var action in OnEnterActions)
            {
                if (action.TargetSelf)
                {
                    Execute(action);
                }
            }
        }

        if (eventName == "OnCollisionExit")
        {
            foreach (var action in OnExitActions)
            {
                // On éxecute les actions
            }
        }
    }

    void Execute(ActionCommand command, Entity? entity = null)
    {
        Entity? target = (command.TargetSelf) ? Owner : entity;

        if (command.Verb == ActionVerb.Destroy)
        {
            if (target != null) Engine.Instance.DestroyEntity(target);
        }
    }
}
