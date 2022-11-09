using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechniqueUsed : MonoBehaviour
{

    public FightSystem fightSystem;

    [HideInInspector]public string opponentsNextMove;
    private List<string> techniques = new List<string>() { "Attack", "Counter", "Reposition", "Feint" }; 

    public void TechniqueSelect(string technique) // This function is called from the UI button component on each action button
    {
        opponentsNextMove = techniques[Random.Range(0, techniques.Count)]; //AI? No. No this motherfucker just does whatever.
        fightSystem.NextRound(technique, opponentsNextMove); // This calls the massive function in FightSystem.cs
    }
}
