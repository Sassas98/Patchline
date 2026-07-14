
using System;
using System.Collections.Generic;
using System.Linq;

public class Memory
{
    private Dictionary<string, int> memory = new Dictionary<string, int>();
    public bool InError {get; private set;} = false;
    private string ErrorMessage = "";

    public int Get(string name)
    {
        if(int.TryParse(name, out int value))
        {
            return value;
        }
        if (!memory.ContainsKey(name))
        {
            SetOnError($"Variable {name} not found");
            return -1;
        }
        return memory[name];
    }
    public void Let(string name, int value)
    {
        if (memory.ContainsKey(name))
        {
            SetOnError("Impossible declare again " + name);
        }
        else Save(name, value);
    }
    public void Set(string name, int value)
    {
        if (!memory.ContainsKey(name))
        {
            SetOnError("This variable wasn't declared: " + name);
        }
        else Save(name, value);
    }
    private void Save(string name, int value)
    {
        memory[name] = value;
    } 
    public void SetOnError(string error){
        ErrorMessage = error;
        InError = true;
    }
    public MemoryState GetState()
    {
        return new MemoryState
        {
            InError = InError,
            ErrorMessage = ErrorMessage,
            Memory = string.Join("; ", memory.Select(x => x.Key + " : " + x.Value))
        };
    }
    public bool CheckCondition(string arg1, string arg2, Condition condition)
    {
        var err = InError;
        int val1 = Get(arg1);
        int val2 = Get(arg2);
        if(!err && InError)
        {
            InError = false;
            ErrorMessage = string.Empty;
            return false;
        }
        return condition switch
        {
            Condition.Equal => val1 == val2,
            Condition.NotEqual => val1 != val2,
            Condition.GreaterThan => val1 > val2,
            Condition.LessThan => val1 < val2,
            Condition.GreaterThanOrEqual => val1 >= val2,
            Condition.LessThanOrEqual => val1 <= val2,
            _ => false
        };
    }
}
