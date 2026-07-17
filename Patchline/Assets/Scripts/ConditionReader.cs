
using System;

abstract public class ConditionReader
{
    protected Condition GetCondition(string condition)
    {
        return condition switch
        {
            "==" => Condition.Equal,
            "!=" => Condition.NotEqual,
            ">" => Condition.GreaterThan,
            "<" => Condition.LessThan,
            ">=" => Condition.GreaterThanOrEqual,
            "<=" => Condition.LessThanOrEqual,
            _ => throw new Exception($"Invalid condition: {condition}")
        };
    }

    protected bool IsCondition(string condition)
    {
        try
        {
            GetCondition(condition); 
            return true;
        }
        catch 
        { 
            return false; 
        }
    }
}
