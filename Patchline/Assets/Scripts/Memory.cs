
using System;
using System.Collections.Generic;
using System.Linq;

public class Memory
{
    private Dictionary<string, int> memory = new Dictionary<string, int>();
    private bool InError = false;
    private string ErrorMessage = "";

    public int Get(string name)
    {
        if(int.TryParse(name, out int value))
        {
            return value;
        }
        if (!memory.ContainsKey(name))
        {
            ErrorMessage = $"Variable {name} not found";
            InError = true;
            return -1;
        }
        return memory[name];
    }
    public int GetOrElse(string name, int @else)
    {
        try
        {
            return Get(name);
        }
        catch 
        { 
            return @else; 
        }
    }
    public void Set(string name, string value)
    {
        if (int.TryParse(value, out int intValue))
        {
            memory[name] = intValue;
        }
        else if (memory.ContainsKey(value))
        {
            memory[name] = memory[value];
        }
        else
        {
            ErrorMessage = $"Invalid value: {value}";
            InError = true;
        }
    } 
    public MemoryState GetState()
    {
        return new MemoryState
        {
            InError = InError,
            ErrorMessage = ErrorMessage,
            Memory = memory.Select(x => x.Key + " = " + x.Value).ToArray()
        };
    }
    public bool CheckCondition(string arg1, string arg2, Condition condition)
    {
        int val1 = Get(arg1);
        int val2 = Get(arg2);
        return condition switch
        {
            Condition.Equal => val1 == val2,
            Condition.NotEqual => val1 != val2,
            Condition.GreaterThan => val1 > val2,
            Condition.LessThan => val1 < val2,
            Condition.GreaterThanOrEqual => val1 >= val2,
            Condition.LessThanOrEqual => val1 <= val2,
            _ => throw new Exception("Invalid condition")
        };
    }
}
