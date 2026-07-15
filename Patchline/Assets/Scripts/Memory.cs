
using System;
using System.Collections.Generic;
using System.Linq;

public class Memory
{
    private Dictionary<string, int> memory = new Dictionary<string, int>();
    private Dictionary<string, List<int>> listMemory = new Dictionary<string, List<int>>();
    public bool InError {get; private set;} = false;
    private string ErrorMessage = "";

    #region Lettura
    public int Get(string name)
    {
        if(int.TryParse(name, out int value))
        {
            return value;
        }
        if (name.StartsWith('[') && name.EndsWith(']'))
        {
            try
            {
                return CalculateHash(GetListFromString(name));
            }
            catch
            {
                SetOnError($"List {name} not valid");
                return -1;
            }
        }
        if (listMemory.ContainsKey(name))
        {
            return CalculateHash(listMemory[name]);
        }
        if (name.Contains(':'))
        {
            var parts = name.Split(":");
            return ListGet(parts[1], parts[0]);
        }
        if (!memory.ContainsKey(name))
        {
            SetOnError($"Variable {name} not found");
            return -1;
        }
        return memory[name];
    }
    private List<int> GetListFromString(string s)
    {
        var nums = s.Replace("[", "").Replace("]", "").Split(",");
        var result = new List<int>();
        foreach (var num in nums)
        {
            result.Add(int.Parse(num));
        }
        return result;
    }
    private int CalculateHash(List<int> list)
    {
        int result = 17;
        foreach (var num in list)
        {
            result = 31 * result + num;
        }
        return result;
    }
    private int ListGet(string name, string method)
    {
        if (!listMemory.ContainsKey(name))
        {
            SetOnError($"Variable {name} not found");
            return -1;
        }
        else if (listMemory[name].Count() == 0)
        {
            SetOnError($"List {name} is empty");
            return -1;
        }
        var result = -1;
        if (method == "SHIFT" || method == "FIRST")
        {
            result = listMemory[name].First();
            listMemory[name].RemoveAt(0);
        }
        else if (method == "POP" || method == "LAST")
        {
            result = listMemory[name].Last();
            listMemory[name].RemoveAt(listMemory[name].Count() - 1);
        }
        else if (method == "LENGTH")
        {
            result = listMemory[name].Count();
        }
        else
        {
            SetOnError($"Method {method} not known");
        }
        return result;
    }
    #endregion

    #region Scrittura
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
    public void List(string name)
    {
        listMemory[name] = new List<int>();
    }
    public void Push(string name, int value)
    {
        if (!listMemory.ContainsKey(name))
        {
            SetOnError("This list wasn't declared: " + name);
        }
        listMemory[name].Add(value);
    }
    public void Inject(string name, int value)
    {
        if (!listMemory.ContainsKey(name))
        {
            SetOnError("This list wasn't declared: " + name);
        }
        listMemory[name].Insert(0, value);
    }
    #endregion

    #region Utility
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
            Memory = string.Join("; ", memory.Select(x => x.Key + " : " + x.Value).Concat(
                    listMemory.Select(x => x.Key + " : [" + string.Join(",", x.Value.Select(x => x.ToString())) + "]")
                ))
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
    #endregion
}
