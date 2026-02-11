using ReForge.Engin.Core;

namespace ReForge.Engine.World;
using ReForge.Engine.Core;

[HiddenSelectableBehavior] 
public class ActionTrigger: Behavior
{
    public List<ActionCondition> Conditions { get; set; } = new();
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
            OnExitActions = this.OnExitActions,
            Conditions = this.Conditions
        };
    }

    public override void OnReceivedEvent(string eventName, object? data = null)
    {
        Entity? entity = data as Entity;

        if(!CheckConditions(entity)) return;
        
        if (eventName == "OnCollisionEnter")
        {
            foreach (var action in OnEnterActions)
            {
                Execute(action, entity);
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
        List<Entity> targets = new();

        if (command.TargetSelf)
        {
            targets.Add(Owner);
        }
        else if (!string.IsNullOrEmpty(command.TargetTag))
        {
            var found = Engine.Instance.CurrentScene.Entities
                .Where(e => e.HasTag(command.TargetTag.Trim()));
            targets.AddRange(found);
        }
        else if (entity != null)
        {
            targets.Add(entity);
        }

        foreach (var target in targets)
        {
            switch (command.Verb)
            {
                case ActionVerb.Destroy:
                    Engine.Instance.CurrentScene.DestroyEntity(target);
                    break;
                case ActionVerb.Teleport:
                    target.Position = command.Destination;
                    break;
                case ActionVerb.SetSwitch:
                    ProjectManager.SetSwitch(command.Key, command.Value != 0);
                    break;
                case ActionVerb.SetVariable:
                    ProjectManager.SetVariable(command.Key, command.Value);
                    break;
                case ActionVerb.AddValueVariable:
                    float currentAddValue = ProjectManager.GetVariable(command.Key);
                    ProjectManager.SetVariable(command.Key, currentAddValue + command.Value);
                    break;
                case ActionVerb.SubtractValueVariable:
                    float currentSubValue = ProjectManager.GetVariable(command.Key);
                    ProjectManager.SetVariable(command.Key, currentSubValue - command.Value);
                    break;
            }
        }
    }
    
    bool CheckConditions(Entity entity)
    {
        if(Conditions.Count == 0) return true;

        foreach (var cdtn in Conditions)
        {
            if (cdtn.Evaluate() == false)
            {
                return false;
            }
        }
        return true;
    }
}