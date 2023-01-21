using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using PixelCrushers;
using System.IO;
public class DialogueDataManager : MonoBehaviour
{
    public Button[] buttons;
    public Text[] textDisplays;
    public float[] diceRolls;
    public float rollDificulty;
    float physMod;
    float socMod;
    float menMod;
    public SaveFile saveFile;
    string savePath;
    private bool rolled;
    // Start is called before the first frame update
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/saveFile.JSON";
    }
    void Start()
    {
        if (!File.Exists(savePath))
        {
            File.CreateText(savePath).Close();
            StartNewGame();
        }
        string json = File.ReadAllText(savePath);
        saveFile = JsonUtility.FromJson<SaveFile>(json);
        LoadGame();
        Debug.Log(DialogueManager.lastConversationID);
        diceRolls = new float[4];
        for (int i = 0; i < diceRolls.Length ; i++)
        {
            diceRolls[i] = Random.Range(1, 10);
        }
        physMod = DialogueLua.GetVariable("PlayerStats.Physical").AsFloat;
        socMod = DialogueLua.GetVariable("PlayerStats.Social").AsFloat;
        menMod = DialogueLua.GetVariable("PlayerStats.Mental").AsFloat;
    }

    // Update is called once per frame
    void Update()
    {
        rollDificulty = DialogueLua.GetVariable("Roll.Difficulty").asFloat;
    }
    public void DisplayRoll()
    {
      
        textDisplays[0].text = "Difficulty: " + DialogueLua.GetVariable("Roll.Difficulty").asFloat;
        textDisplays[1].text = "Modifiers: " + physMod + "/" + socMod + "/" + menMod;
        textDisplays[2].text = "Results: " + (diceRolls[0]) + "/" + (diceRolls[1]) + "/" + (diceRolls[2]) + "/" + diceRolls[3];
        bool outcomeBool = DialogueLua.GetVariable("Roll.Success").asBool;
        if (outcomeBool)
        {
            textDisplays[3].text = "Success";
        }
        else
        {
            textDisplays[3].text = "Failed";
        }
        rolled = true;
        SaveGame(true);
    }
    public void CheckDiceRolls()
    {
        diceRolls[0] = diceRolls[0] + physMod;
        diceRolls[1] = diceRolls[1] + socMod;
        diceRolls[2] = diceRolls[2] + menMod;
        int sucessfullRolls = 0;
        foreach (float diceRoll in diceRolls)
        {
            if (diceRoll >= 6)
                sucessfullRolls++;
        }
        if (sucessfullRolls >= DialogueLua.GetVariable("Roll.Difficulty").asFloat)
            DialogueLua.SetVariable("Roll.Success", true);
        else
            DialogueLua.SetVariable("Roll.Success", false);
        DisplayRoll();
        CheckGameStatus();
    }
    public void StartNewGame()
    {
        PersistentDataManager.Reset();
        rolled = false;
        saveFile.lastConversationId = 0;
        SaveGame();
    }
    public void SaveGame(bool fromRoll = false)
    {
        saveFile.save = PersistentDataManager.GetSaveData();
        saveFile.rolled = rolled;
        saveFile.lastConversationId = DialogueManager.lastConversationID;
        if (fromRoll)
            saveFile.lastConversationId = 1;
        string json = JsonUtility.ToJson(saveFile);
        File.WriteAllText(savePath, json);
    }
    public void LoadGame()
    {
        PersistentDataManager.ApplySaveData(saveFile.save);
        rolled = saveFile.rolled;
        CheckGameStatus();
    }
    public void CheckGameStatus()
    {
        switch (saveFile.lastConversationId)
        {
            case 1:
                if (!rolled)
                    buttons[2].gameObject.SetActive(true);
                else
                    buttons[1].gameObject.SetActive(true);
                return;
            case 2:
                StartNewGame();
                buttons[0].gameObject.SetActive(true);
                return;
            default:
                buttons[0].gameObject.SetActive(true);
                break;
        }
    }
    public void OnConversationEnd()
    {
        SaveGame();
        CheckGameStatus();
    }
}
[System.Serializable]
public class SaveFile
{
    public string save;
    public bool rolled;
    public int lastConversationId;
}
