using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private TextMeshProUGUI std;
    private string text = "let x = 10\nif x > 5\n x = -9";

    // Start is called before the first frame update
    void Start()
    {
        std = GameObject.Find("Text_STD").GetComponent<TextMeshProUGUI>();
        std.SetText(MarkText(text, 1, false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string MarkText(string text, int line, bool patch)
    {
        string color = patch ? "#FB3640" : "#FB3640";
        string[] parts = text.Split("\n");
        parts[line] = $"<color={color}>{parts[line]}</color>";
        return string.Join("\n", parts);
    }
}
