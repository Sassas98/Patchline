using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

public class GameMaster : MonoBehaviour
{
    private int line_counter = 0;

    private readonly string[] energy_colors = { "#3BC55C", "#c5ac3b", "#c53963" };
    private readonly (string Keyword, string Simboli, string Variabili, string Numeri)[] palette
        = new (string, string, string, string)[]
        {
            ("#FF4FD8", "#46F0FF", "#A8FF60", "#FFD84A"),
            ("#A56F94", "#6F8E93", "#849674", "#A99969"),
            ("#FF2B45", "#FF6B35", "#FF9F1C", "#FFE45E")
        };
    private readonly string select_color = "#E8E8E8";
    private TextMeshProUGUI std, wk, gs, vars, runs, ene;
    private Button if_button;
    private string text = "LET TEMP = 10\nIF TEMP > 5\n   SET TEMP = -9";
    private string work = "";
    private string goals = "TEMP >= 100";
    private bool running = false;
    private bool if_submit = false;
    private GameExecuter exec;
    private float run_time = 1.6f;
    private LevelMaster levelMaster;
    private CMD cmd = CMD.Set;
    private List<GameObject> change_button, static_button, to_delete;
    private CodePaletteApplier palette_applier;
    [SerializeField] private GameObject btnOption;
    [SerializeField] private GameObject set_modal;
    [SerializeField] private GameObject if_modal;
    [SerializeField] private AudioClip theme, error1, error2, help, so_verify, senior
                                     , btn1, btn2, btn3, cancel, fail, step1, step2, success;
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private bool stepflag = false;

    private void UpdateInfoLabels()
    {
        runs.SetText(MarkText($"RUNS [{Global.State.Vite}]", energy_colors[Global.State.Vite > 2 ? 0 : Global.State.Vite == 2 ? 1 : 2]));
        ene.SetText(MarkText($"ENERGY [{Global.State.Energia}]", energy_colors[Global.State.Energia >= 10 ? 0 : Global.State.Energia >= 5 ? 1 : 2]));
    }

    private void BtnEffect() 
    {
        //var rnd = UnityEngine.Random.Range(0, 3);
        sfxSource.PlayOneShot(btn1);
    }
    private void CancelEffect() => sfxSource.PlayOneShot(cancel);
    private void FailEffect() => sfxSource.PlayOneShot(fail);
    private void SuccessEffect() => sfxSource.PlayOneShot(success);
    private void StepEffect()
    {
        sfxSource.PlayOneShot(stepflag ? step1 : step2);
        stepflag = !stepflag;
    }

    private void ChangeMusic()
    {
        musicSource.clip =
            Global.State.Vite > 2 ? theme :
            Global.State.Vite == 2 ? error1 :
            Global.State.StepCorrente < 4 ? error2 :
            Global.State.LivelloCorrente % 5 == 0 ? so_verify :
            Global.State.LivelloCorrente % 3 == 0 ? help :
            senior;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private string MarkText(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }

    public void UpdatePalette()
    {
        (string Keyword, string Simboli, string Variabili, string Numeri)
            = palette[Global.State.Vite == 3 ? 0 : Global.State.Vite == 2 ? 1 : 2];
        palette_applier.SetPalette(Keyword, Simboli, Variabili, Numeri);
    }

    private void UpdateLegacyLine()
    {
        UpdatePalette();
        std.SetText(palette_applier.MarkText(text, line_counter - 1, "#46F0FF"));
    }

    private void UpdateWorkCode()
    {
        UpdatePalette();
        wk.SetText(palette_applier.MarkText(work, -1, select_color));
    }

    public void SetUpLevel()
    {
        HandleButtonInLevel();
        work = string.Empty;
        var ll = levelMaster.GetLevel(Global.State.LivelloCorrente, Global.State.StepCorrente);
        text = ll.Code;
        goals = ll.Goals;
        Global.State.Vite = Global.State.StepCorrente == 4 ? 1 : 3;
        Global.State.Energia = Global.State.StepCorrente == 0 || Global.State.StepCorrente == 4 ? ll.Energy : Global.State.Energia + ll.Energy;
        gs.SetText(goals);
        vars.SetText("");
        wk.SetText("");
        UpdateInfoLabels();
        line_counter = 0;
        UpdateLegacyLine();
        ChangeMusic();
        Global.Salva();
    }

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>(); 
        musicSource.loop = true;
        musicSource.volume = 0.4f;
        sfxSource.loop = false;
        sfxSource.volume = 1f;
        ChangeMusic();
        palette_applier = new CodePaletteApplier();
        runs = GameObject.Find("RUNS").GetComponent<TextMeshProUGUI>();
        ene = GameObject.Find("ENERGY").GetComponent<TextMeshProUGUI>();
        //lv = GameObject.Find("LV").GetComponent<TextMeshProUGUI>();
        levelMaster = new LevelMaster();
        var l = levelMaster.GetLevel(Global.State.LivelloCorrente, Global.State.StepCorrente);
        text = l.Code;
        goals = l.Goals;
        Global.State.Energia = Math.Max(Global.State.Energia, l.Energy);
        UpdateInfoLabels();
        std = GameObject.Find("Text_STD").GetComponent<TextMeshProUGUI>();
        UpdateLegacyLine();
        vars = GameObject.Find("vars").GetComponent<TextMeshProUGUI>();
        wk = GameObject.Find("Text_Work").GetComponent<TextMeshProUGUI>();
        gs = GameObject.Find("Text_Goals").GetComponent<TextMeshProUGUI>();
        gs.SetText(goals);

        set_modal.SetActive(true);
        if_modal.SetActive(true);
        GameObject.Find("set_back").GetComponent<Button>()
            .onClick.AddListener(() => {
                CancelEffect();
                set_modal.SetActive(false);
            });
        GameObject.Find("if_back").GetComponent<Button>()
            .onClick.AddListener(() => {
                CancelEffect();
                if_modal.SetActive(false);
            });
        GameObject.Find("CONTINUE").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                if (++Global.State.StepCorrente > 4) 
                { 
                    Global.State.LivelloCorrente++; 
                    Global.State.StepCorrente = 0; 
                }
                Global.Salva();
                SetUpLevel();
                BtnEffect();
            });
        GameObject.Find("CANC").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                CancelEffect();
                if (running || string.IsNullOrWhiteSpace(work)) return;
                var lines = work.Split("\n");
                Global.State.Energia += Enum.Parse<CMD>(lines[lines.Length - 2].Trim().Split(" ")[0], true).DaiCosto();
                UpdateInfoLabels();
                work = lines.Length < 3 ? string.Empty :
                string.Join("", lines[..^2].Select(x => x + "\n"));
                var spaces = lines.Last().Length - lines.Last().TrimStart().Length;
                for (int i = 0; i < spaces; i++) work += " ";
                UpdateWorkCode();
                line_counter--;
                UpdateLegacyLine();
            });
        GameObject.Find("<--").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                CancelEffect();
                if (running || string.IsNullOrWhiteSpace(work)) return;
                var lines = work.Split("\n");
                var spaces = lines.Last().Length - lines.Last().TrimStart().Length;
                if (spaces < 3) return;
                work = string.Join("", lines[..^1].Select(x => x + "\n")) 
                    + string.Join("", Enumerable.Range(0, spaces - 3).Select(_ => " "));
                UpdateWorkCode();
            });
        GameObject.Find("WAIT").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (running || Global.State.Energia < CMD.Wait.DaiCosto()) return;
                AddOnCodeWork("WAIT");
                Global.State.Energia-= CMD.Wait.DaiCosto();
                UpdateInfoLabels();

            });
        GameObject.Find("SKIP").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (running || Global.State.Energia < CMD.Skip.DaiCosto()) return;
                AddOnCodeWork("SKIP");
                Global.State.Energia-= CMD.Skip.DaiCosto();
                UpdateInfoLabels();

            });
        GameObject.Find("STOP").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (running || Global.State.Energia < CMD.Stop.DaiCosto()) return;
                AddOnCodeWork("STOP");
                Global.State.Energia-= CMD.Stop.DaiCosto();
                UpdateInfoLabels();

            });
        GameObject.Find("ELSE").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (running || Global.State.Energia < CMD.Else.DaiCosto()) return;
                var lines = work.Split("\n");
                var spaces = lines.Last().Length - lines.Last().TrimStart().Length;
                if (spaces >= 3)  work = string.Join("", lines[..^1].Select(x => x + "\n"))
                                       + string.Join("", Enumerable.Range(0, spaces - 3).Select(_ => " "));
                AddOnCodeWork("ELSE", true);
                Global.State.Energia-= CMD.Else.DaiCosto();
                UpdateInfoLabels();

            });
        GameObject.Find("LIST").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (running || Global.State.Energia < CMD.List.DaiCosto()) return;
                int num = (text + "\n" + wk).Split("\n")
                    .Where(x => x.StartsWith("LIST L"))
                    .Select(x => x.Replace("LIST L", ""))
                    .Select(n => int.TryParse(n, out num) ? num : -1)
                    .DefaultIfEmpty(-1).Max() + 1;
                AddOnCodeWork("LIST L" + num);
                Global.State.Energia-= CMD.List.DaiCosto();
                UpdateInfoLabels();

            });
        GameObject.Find("SET").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Set.DaiCosto())) 
                    ShowSetModal();
            });
        GameObject.Find("LET").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Let.DaiCosto())) 
                    ShowLetModal();
            });
        GameObject.Find("PUSH").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Push.DaiCosto())) 
                    ShowPushModal();
            });
        GameObject.Find("INJECT").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Inject.DaiCosto())) 
                    ShowInjectModal();
            });
        GameObject.Find("IF").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.If.DaiCosto()))
                    ShowIfModal();
            });
        GameObject.Find("ELIF").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Elif.DaiCosto()))
                    ShowElifModal();
            });
        GameObject.Find("LOOP").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!(running || Global.State.Energia < CMD.Loop.DaiCosto()))
                    ShowLoopModal();
            });
        GameObject.Find("PLAY").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                BtnEffect();
                if (!running)
                {
                    running = true;
                    StartRun();
                }
            });
        GameObject.Find("set_canc").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                CancelEffect();
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
        GameObject.Find("if_canc").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                CancelEffect();
                var input = GameObject.Find("if_input").GetComponent<TextMeshProUGUI>();
                var parts = input.text.Split(" ");
                var last = parts.Last();
                if(last == "0")
                {
                    if(parts.Length > 1)
                    {
                        input.text = string.Join(" ", parts[..^2]);
                        if (GetLogicOperator().Contains(parts[parts.Length-2]) || GetLogicSeparator().Contains(parts[parts.Length - 2]))
                        {
                            ChangeIfSubmit();
                        }
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
                BtnEffect();
                var value = GameObject.Find("Set_Dropdown").Childrens()[0]
                    .GetComponent<TextMeshProUGUI>().text.Trim();
                var input = GameObject.Find("set_input").GetComponent<TextMeshProUGUI>().text.Trim();
                AddOnCodeWork((cmd == CMD.Set ? string.Empty : cmd.ToString().ToUpper() + " ") + $"{value} = {input}");
                set_modal.SetActive(false);
                Global.State.Energia -= cmd.DaiCosto();
                UpdateInfoLabels();
            });
        if_button = GameObject.Find("ADD_IF").GetComponent<Button>();
        if_button.onClick.AddListener(() =>
        {
            BtnEffect();
            if (cmd == CMD.Elif)
            {
                var lines = work.Split("\n");
                var spaces = lines.Last().Length - lines.Last().TrimStart().Length;
                if (spaces >= 3) work = string.Join("", lines[..^1].Select(x => x + "\n"))
                                        + string.Join("", Enumerable.Range(0, spaces - 3).Select(_ => " "));
            }
            var input = GameObject.Find("if_input").GetComponent<TextMeshProUGUI>().text.Trim();
            AddOnCodeWork($"{cmd.ToString().ToUpper()} {input}", true);
            if_modal.SetActive(false);
            Global.State.Energia -= cmd.DaiCosto();
            UpdateInfoLabels();
        });

        static_button = new List<GameObject>
        {
            GameObject.Find("SET"),
            GameObject.Find("WAIT"),
            GameObject.Find("CANC"),
            GameObject.Find("PLAY"),
        };
        change_button = new List<GameObject>
        {
            GameObject.Find("LET"),
            GameObject.Find("LOOP"),
            GameObject.Find("<--"),
            GameObject.Find("STOP"),
            GameObject.Find("SKIP"),
            GameObject.Find("IF"),
            GameObject.Find("ELSE"),
            GameObject.Find("ELIF"),
            GameObject.Find("LIST"),
            GameObject.Find("PUSH"),
            GameObject.Find("INJECT"),
            GameObject.Find("CONTINUE"),
        };
        HandleButtonInLevel();
        set_modal.SetActive(false);
        if_modal.SetActive(false);
    }

    private void HandleButtonInLevel()
    {
        var lvl = Global.State.LivelloCorrente;
        static_button.ForEach(x => x.SetActive(true));
        change_button.ForEach(x => x.SetActive(true));
        if (lvl < 1) change_button.ForEach(x => x.SetActive(false));
        else if(lvl < 3) change_button.Skip(1).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 5) change_button.Skip(3).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 7) change_button.Skip(4).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 8) change_button.Skip(5).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 11) change_button.Skip(7).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 14) change_button.Skip(9).ToList().ForEach(x => x.SetActive(false));
        else if(lvl < 17) change_button.Skip(10).ToList().ForEach(x => x.SetActive(false));
        else change_button.Skip(11).ToList().ForEach(x => x.SetActive(false));

    }
    private void AddOnCodeWork(string line, bool space = false)
    {
        var lastLine = work.Split("\n").Last();
        var spaces = lastLine.Length - lastLine.TrimStart().Length;
        if (space) spaces += 3;
        work += line + "\n";
        for(int i = 0; i < spaces; i++) 
        {
            work += " ";
        }
        UpdateWorkCode();
        line_counter++;
        UpdateLegacyLine();
    }

    private void StartRun()
    {
        exec = new GameExecuter(text, goals, work);
        run_time = 1.0f;
        Invoke(nameof(ExecuteRun), run_time);
    }

    private void ExecuteRun()
    {
        var data = exec.GetData();
        if (data.StepCount >= 100)
        {
            data.Memory.InError = true;
            data.Memory.ErrorMessage = "Loop detected";
        }

        vars.text = data.Memory.InError ? data.Memory.ErrorMessage : data.Memory.Memory;
        var g = string.Join("\n", data.Goals.Select(x => x.Label));
        for(int i = 0; i < data.Goals.Length; i++)
        {
            g = MarkText(g, i, data.Goals[i].Result);
        }
        gs.SetText(g);
        if (data.IsEnded || data.Memory.InError)
        {
            UpdateLegacyLine();
            UpdateWorkCode();
            running = false;
            if(data.IsEnded && !data.Memory.InError && data.Goals.All(x => x.Result))
            {
                static_button.ForEach(x => x.SetActive(false));
                change_button.ForEach(x => x.SetActive(false));
                change_button.Last().SetActive(true);
                SuccessEffect();
            }
            else
            {
                if(--Global.State.Vite <= 0)
                {
                    Global.State.StepCorrente = 0;
                    Global.State.Energia = 0;
                    Global.State.Vite = 3;
                    Global.Salva();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                UpdateLegacyLine();
                UpdateWorkCode();
                UpdateInfoLabels();
                ChangeMusic();
                FailEffect();
                Global.Salva();
            }
            return;
        }
        StepEffect();
        std.SetText(palette_applier.MarkText(text, data.StdRow, select_color));
        wk.SetText(palette_applier.MarkText(work, data.PlayerRow, select_color));
        exec.MakeOneStep();
        if(run_time > 0.2f)
            run_time -= 0.06f;
        Invoke(nameof(ExecuteRun), run_time);
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
        cmd = CMD.Set;
        InitSetLetModal();
        var dd = GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(GetAllVariables());
    }

    public void ShowLetModal()
    {
        cmd = CMD.Let;
        InitSetLetModal();
        var dd = GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(GetNewVariables());
    }

    public void ShowPushModal()
    {
        cmd = CMD.Push;
        InitSetLetModal();
        var dd = GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(GetListVariables());
    }

    public void ShowInjectModal()
    {
        cmd = CMD.Inject;
        InitSetLetModal();
        var dd = GameObject.Find("Set_Dropdown").GetComponent<TMP_Dropdown>();
        dd.ClearOptions();
        dd.AddOptions(GetListVariables());
    }

    public void ShowIfModal()
    {
        cmd = CMD.If;
        InitIfModal();
    }

    public void ShowElifModal()
    {
        cmd = CMD.Elif;
        InitIfModal();
    }

    public void ShowLoopModal()
    {
        cmd = CMD.Loop;
        InitIfModal();
    }

    private void InitSetLetModal()
    {
        set_modal.SetActive(true);
        var input = GameObject.Find("set_input").GetComponent<TextMeshProUGUI>();
        GameObject.Find("CMD").GetComponent<TextMeshProUGUI>().SetText(cmd == CMD.Set ? "" : cmd.ToString().ToUpper());
        input.text = "0";
        var content = GameObject.Find("set_content");
        content.Childrens().ForEach(e => Destroy(e));

        if(Global.State.LivelloCorrente > 0)
        {
            var vars = GetAllVariables();
            foreach (var v in vars)
            {
                GameObject btn = Instantiate(btnOption, content.transform);
                btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    BtnEffect();
                    input.text = string.Join(" ", input.text.Split(" ")[..^1]) + " " + v; 
                });
            }
        }
        foreach (var v in GetOperators())
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                BtnEffect(); 
                input.text += " " + v + " 0"; 
            });
        }

        foreach (var n in Enumerable.Range(0, 10))
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = n.ToString();
            btn.GetComponent<Button>().onClick.AddListener(() => {
                BtnEffect();
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
                BtnEffect();
                var parts = input.text.Split(" ");
                var last = parts[parts.Length - 1];
                parts = parts[..^1];
                last = last[0] == '-' ? last.Substring(1) : "-" + last;
                if (parts.Length > 0)
                    input.text = string.Join(" ", parts) + " " + last;
                else input.text = last;
            });
        }
    }

    private void ChangeIfSubmit()
    {
        if_submit = !if_submit;
        if_button.gameObject.SetActive(if_submit);
        var content = GameObject.Find("if_content");
        to_delete.ForEach(e => Destroy(e));
        to_delete = new List<GameObject>();
        var list = if_submit ? GetLogicSeparator() : GetLogicOperator();
        var input = GameObject.Find("if_input").GetComponent<TextMeshProUGUI>();
        foreach (var v in list)
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                BtnEffect();
                input.text += " " + v + " 0";
                ChangeIfSubmit();
            });
            to_delete.Add(btn);
        }
    }

    private void InitIfModal()
    {
        if_modal.SetActive(true);
        var input = GameObject.Find("if_input").GetComponent<TextMeshProUGUI>();
        GameObject.Find("CMD2").GetComponent<TextMeshProUGUI>().SetText(cmd.ToString().ToUpper());
        input.text = "0";
        var content = GameObject.Find("if_content");
        content.Childrens().ForEach(e => Destroy(e));
        if_submit = false;
        if_button.gameObject.SetActive(if_submit);
        to_delete = new List<GameObject>();

        var vars = GetAllVariables();
        foreach (var v in vars)
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => 
            { 
                BtnEffect(); 
                input.text = string.Join(" ", input.text.Split(" ")[..^1]) + " " + v; 
            });
        }
        foreach (var v in GetOperators())
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => { BtnEffect(); input.text += " " + v + " 0"; });
        }

        foreach (var n in Enumerable.Range(0, 10))
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = n.ToString();
            btn.GetComponent<Button>().onClick.AddListener(() => {
                BtnEffect();
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
                BtnEffect();
                var parts = input.text.Split(" ");
                var last = parts[parts.Length - 1];
                parts = parts[..^1];
                last = last[0] == '-' ? last.Substring(1) : "-" + last;
                if (parts.Length > 0)
                    input.text = string.Join(" ", parts) + " " + last;
                else input.text = last;
            });
        }
        foreach (var v in GetLogicOperator())
        {
            GameObject btn = Instantiate(btnOption, content.transform);
            btn.Childrens()[0].GetComponent<TextMeshProUGUI>().text = v;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                BtnEffect();
                input.text += " " + v + " 0";
                ChangeIfSubmit();
            });
            to_delete.Add(btn);
        }
    }

    private List<string> GetOperators()
    {
        return new List<string>
        {
            "+", "-", "*", "/", "%"
        };
    }

    private List<string> GetLogicOperator()
    {
        return new List<string>
        {
            "==", "!=", "<", ">", "<=", ">="
        };
    }

    private List<string> GetLogicSeparator()
    {
        return new List<string>
        {
            "AND", "OR"
        };
    }

    private List<string> GetAllVariables(){
        return GetBasicVariables()
            .Concat(
                GetListVariables()
                .SelectMany(x =>(Global.State.LivelloCorrente < 17 ? 
                    new string[] { "LENGTH", "LAST", "POP" } : 
                    new string[] { "LENGTH", "FIRST", "LAST", "POP", "SHIFT" } )
                .Select(y => y + ":" + x))
            )
            .ToList();
    }

    private List<string> GetBasicVariables(){
        return (text + "\n" + work)
            .Split("\n")
            .Select(x => x.Split(" "))
            .Where(x => x[0].ToUpper() == "LET")
            .Select (x => x[1].ToUpper()).ToList();
    }

    public List<string> GetNewVariables()
    {
        var vars = GetBasicVariables()
            .Concat(GetListVariables())
            .ToList();
        return new string[]
        {
            "base",
            "args",
            "data",
            "item",
            "node",
            "temp"
        }
        .Concat(GetGoalVariables())
        .Distinct()
        .Select (x => x.ToUpper())
        .Where(x => !vars.Contains(x))
        .OrderBy (x => x).ToList();
    }

    private List<string> GetGoalVariables()
    {
        return goals.Split("\n")
            .SelectMany(x => x.Split(" "))
            .Where(x => x.All(c => char.IsLetter(c)))
            .Distinct().ToList();
    }

    public List<string> GetListVariables()
    {
        return (text + "\n" + work)
                .Split("\n")
                .Select(x => x.Split(" "))
                .Where(x => x[0].ToUpper() == "LIST")
                .Select(x => x[1].ToUpper()).Distinct().ToList();
    }

    private bool IsCMD(string word)
    {
        return Enum.GetNames(typeof(CMD))
            .Select(z => z.ToUpper())
            .Contains(word.Trim().ToUpper());
    }
}
