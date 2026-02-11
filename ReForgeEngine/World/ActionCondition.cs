using System;
using ReForge.Engin.Core;

namespace ReForge.Engine.World;

public enum ActionConditionType
{
    Switch, 
    Variable
}

public enum ConditionOperator
{
    Equal, 
    NotEqual,
    Greater,
    Less, 
    GreaterOrEqual, 
    LessOrEqual
}

public class ActionCondition
{
    // ID stable pour l'UI (préserve l'état des TreeNodes indépendamment de l'index)
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Key { get; set; } = "";
    public ActionConditionType Type { get; set; }
    public ConditionOperator Operator { get; set; }
    public float Value { get; set; }

    public bool Evaluate()
    {
        if (Type == ActionConditionType.Switch)
        {
            bool currentValue = ProjectManager.GetSwitch(Key);
            bool targetValue = Value != 0;
            return Operator == ConditionOperator.Equal ?
                currentValue == targetValue : 
                currentValue != targetValue;
        }
        else
        {
            float currentValue = ProjectManager.GetVariable(Key);
            return Operator switch
            {
                ConditionOperator.Equal => currentValue == Value,
                ConditionOperator.NotEqual => currentValue != Value,
                ConditionOperator.Greater => currentValue > Value,
                ConditionOperator.Less => currentValue < Value,
                ConditionOperator.GreaterOrEqual => currentValue >= Value,
                ConditionOperator.LessOrEqual => currentValue <= Value,
                _ => false
            };
        }
    }
}
