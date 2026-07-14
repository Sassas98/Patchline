using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameMaster : MonoBehaviour
{
    private TextMeshProUGUI std, wk, vars;
    private string text = "LET TEMP = 10\nIF TEMP > 5\n   SET TEMP = -9";
    private string work = "";
    private string goals = "TEMP >= 100";
    private bool running = false;
    [SerializeField] private GameObject btnOption;
    [SerializeField] private GameObject set_modal;

    // Start is called before the first frame update
    void Start()
    {
        std = GameObject.Find("Text_STD").GetComponent<TextMeshProUGUI>();
        std.SetText(text);
        vars = GameObject.Find("vars").GetComponent<TextMeshProUGUI>();
        wk = GameObject.Find("Text_Work").GetComponent<TextMeshProUGUI>();
        GameObject.Find("Text_Goals").GetComponent<TextMeshProUGUI>().SetText(goals);

        set_modal.SetActive(true);
        GameObject.Find("set_back").GetComponent<Button>()
            .onClick.AddListener(() => set_modal.SetActive(false));
        GameObject.Find("CANC").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                if (running) return;
                work = work.Split("\n").Length < 3 ? string.Empty :
                string.Join("", work.Split("\n")[..^2].Select(x => x + "\n"));
                wk.SetText(work);
            });
        GameObject.Find("WAIT").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                if (running) return;
                work += "WAIT\n";
                wk.SetText(work);
            });
        GameObject.Find("set_show_btn").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                if (!running) 
                    ShowSetModal();
            });
        GameObject.Find("PLAY").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                if (!running)
                {
                    running = true;
                    StartRun();
                }
            });
        GameObject.Find("set_canc").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                var input = GameObject.Find("set_input").GetComponent<TextMeshProUGUI>();
                var parts = input.text.Split(" ");
                var last = parts.Last();
                if(last == "0")
                {
                    if(parts.Length > 1)
                    {
                        input.text = string.Join(" ", parts[..^2]);
                    }
                }
                else
                {
                    last = last[..^1];
                    if (string.IsNullOrEmpty(last) || !int.TryParse(last, out _))
                        last = "0";
                    if (parts.Length > 0)
                        input.text = string.Join(" ", parts[..^1]) + " " + last;
                    else input.text = last;
                }
            });
        GameObject.Find("ADD_SET").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                var value = GetAllVariables()[GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>().value].Trim();
                var input = GameObject.Find("set_input").GetComponent<TextMeshProUGUI>().text.Trim();
                work += $"SET {value} = {input}\n";
                set_modal.SetActive(false);
                wk.SetText(work);
            });
        set_modal.SetActive(false);
    }

    private GameExecuter exec;

    private void StartRun()
    {
        exec = new GameExecuter(text, goals, work);
        InvokeRepeating(nameof(ExecuteRun), 0f, 1.5f);
    }

    private void ExecuteRun()
    {
        var data = exec.GetData();
        if (data.IsEnded || data.Memory.InError)
        {
            CancelInvoke(nameof(ExecuteRun));
            std.SetText(text);
            wk.SetText(work);
            vars.text = data.Memory.ErrorMessage;
            running = false;
            return;
        }
        std.SetText(MarkText(text, data.StdRow, false));
        wk.SetText(MarkText(work, data.PlayerRow, true));
        vars.text = data.Memory.Memory;
        exec.MakeOneStep();
    }

    private string MarkText(string text, int line, bool patch)
    {
        string color = patch ? "#1EFC1E" : "#FB3640";
        string[] parts = text.Split("\n");
        if (parts.Length <= line) return text;
        parts[line] = $"<color={color}>{parts[line]}</color>";
        return string.Join("\n", parts);
    }

    public void ShowSetModal()
    {
        set_modal.SetActive(true);
        var input = GameObject.Find("set_input").GetComponent<TextMeshProUGUI>();
        input.text = "0";
        var content = GameObject.Find("set_content");
        content.Childrens().ForEach(e => Destroy(e));

        var vars = GetAllVariables();
        foreach (var v in vars)
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => input.text = string.Join(" ", input.text.Split(" ")[..^1]) + " " + v);
        }
        foreach (var v in GetOperators())
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => input.text += " " + v + " 0");
        }

        foreach (var n in Enumerable.Range(0, 10))
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = n.ToString();
            btn.GetComponent<Button>().onClick.AddListener(() => {
                var parts = input.text.Split(" ");
                var last = parts[parts.Length - 1];
                parts = parts[..^1];
                last += n;
                if (int.TryParse(last, out int value))
                    last = value.ToString();
                else last = n.ToString();
                if (parts.Length > 0)
                    input.text = string.Join(" ", parts) + " " + last;
                else input.text = last;
            });
        }

        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = "-VAR";
            btn.GetComponent<Button>().onClick.AddListener(() => {
                var parts = input.text.Split(" ");
                var last = parts[parts.Length - 1];
                parts = parts[..^1];
                last = last[0] == '-' ? last.Substring(1) : "-" + last;
                if (parts.Length > 0)
                    input.text = string.Join(" ", parts) + " " + last;
                else input.text = last;
            });
        }

        var dd = GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(vars);
    }

    private List<string> GetOperators()
    {
        return new List<string>
        {
            "+", "-", "*", "/", "%"
        };
    }

    private List<string> GetAllVariables(){
        return (text + "\n" + work)
            .Split("\n")
            .SelectMany(x => x.Split(" "))
            .Where(x => !string.IsNullOrEmpty(x))
            .Where (x => !IsCMD(x))
            .Where (x => x.All(c => char.IsLetter(c)))
            .Select (x => x.ToUpper())
            .Distinct().ToList();
    }

    private bool IsCMD(string word)
    {
        return Enum.GetNames(typeof(CMD))
            .Select(z => z.ToUpper())
            .Contains(word.Trim().ToUpper());
    }
}
