using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartText : MonoBehaviour
{
    private int counter = 0;

    private void Start()
    {
        Invoke(nameof(Step), 5f);
    }

    private void Step()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
}