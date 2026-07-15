using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using File = System.IO.File;

public class SaveData
{
    public int LivelloCorrente { get; set; } = 0;
    public int Crediti { get; set; } = 0;
    public int ResetCounter { get; set; } = 0;
    public int ResetRimasti { get; set; } = 3;
    public string NomeGiocatore { get; set; } = "";
}

public class Global : MonoBehaviour
{
    public static Global State { get; private set; }

    public int LivelloCorrente { get; set; } = 0;
    public int Crediti { get; set; } = 0;
    public int ResetCounter { get; set; } = 0;
    public int ResetRimasti { get; set; } = 3;
    public string NomeGiocatore { get; set; } = "";

    private void Awake()
    {
        if (State != null && State != this)
        {
            Destroy(gameObject);
            return;
        }
        State = this;
        Carica();
        DontDestroyOnLoad(gameObject);
    }

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public static void Salva()
    {
        SaveData dati = new SaveData
        {
            Crediti = State.Crediti,
            ResetCounter = State.ResetCounter,
            ResetRimasti = State.ResetRimasti,
            NomeGiocatore = State.NomeGiocatore,
            LivelloCorrente = State.LivelloCorrente
        };

        string json = JsonUtility.ToJson(dati, true);

        System.IO.File.WriteAllText(SavePath, json);

        Debug.Log($"Partita salvata in: {SavePath}");
    }

    private static bool Carica()
    {
        if (!EsisteSalvataggio())
        {
            Debug.Log("Nessun salvataggio trovato.");
            return false;
        }

        string json = System.IO.File.ReadAllText(SavePath);
        SaveData dati = JsonUtility.FromJson<SaveData>(json);

        State.LivelloCorrente = dati.LivelloCorrente;
        State.ResetCounter = dati.ResetCounter;
        State.ResetRimasti = dati.ResetRimasti;
        State.Crediti = dati.Crediti;
        State.NomeGiocatore = dati.NomeGiocatore;

        Debug.Log("Partita caricata.");
        return true;
    }

    public static void EliminaSalvataggio()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
    }

    private static bool EsisteSalvataggio()
    {
        return File.Exists(SavePath);
    }
}