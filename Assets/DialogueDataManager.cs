using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using PixelCrushers;
public class DialogueDataManager : MonoBehaviour
{
    public Text[] textDisplays;
    public float[] diceRolls;
    public float rollDificulty;
    float physMod;
    float socMod;
    float menMod;
    // Start is called before the first frame update
    void Start()
    {
        LoadGame();

        diceRolls = new float[4];
        for (int i = 0; i < diceRolls.Length ; i++)
        {
            diceRolls[i] = Random.Range(1, 10);
        }
        physMod = DialogueLua.GetVariable("PlayerStats.Physical").AsFloat;
        socMod = DialogueLua.GetVariable("PlayerStats.Social").AsFloat;
        menMod = DialogueLua.GetVariable("PlayerStats.Mental").AsFloat;
        CheckDiceRolls();
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
        SaveGame();
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
    }
    public void StartNewGame()
    {
        SaveSystem.ClearSavedGameData();
    }
    public void SaveGame()
    {
        SaveSystem.SaveToSlot(0);
    }
    public void LoadGame()
    {
        SaveSystem.LoadFromSlot(0);
    }
}
