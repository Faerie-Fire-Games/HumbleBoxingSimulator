using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Opponent : MonoBehaviour
{
    public string fighterName;
    public string fighterType;
    public int power;
    public int toughness;
    public int agility;
    public int willpower;
    public int HP; //Because I'm silly I'm gonna specify that these are hero points not health points. This doesn't matter. But it matters to me.
    [HideInInspector]public List<int> statRolls;

    private List<string> names = new List<string>(){ "Astrea", "Haley", "Alexis", "Samson", "Goliath", "Higgs", "Trevor", "Trina", "Ruby", "Ryuji", "Nozomi", "Hua Cheng", "Jovan", "Gabisile", "Maria"};
    private List<string> types = new List<string>() { "Scrappy", "Bulwark", "Professional", "Brawler"};

    // UI stuff
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text toughnessText;
    [SerializeField] private TMP_Text agilityText;
    [SerializeField] private TMP_Text willpowerText;

    private void Start()
    {
        newFighter();
    }


    public void newFighter()
    {
        fighterName = names[Random.Range(0, names.Count)];
        nameText.text = fighterName;
        fighterType = types[Random.Range(0, types.Count)];
        typeText.text = fighterType;

        // The Following method replicates the "4d6 drop lowest" method of stat generation from dungeons and dragons. Simply because I like sticking to tradition.
        // We are rolling 4 six sided dice, but only taking the sum of the highest three. This gives us a slightly biased value between 3 and 18.
        for (int i = 0; i < 4; i++)
        {
            List<int> rolls = new List<int>() { Random.Range(1, 6), Random.Range(1, 6), Random.Range(1, 6), Random.Range(1, 6) };
            rolls.Remove(rolls.Min());
            statRolls.Add(rolls[0] + rolls[1] + rolls[2]);
        }

        // Here we are assigning our stats in accordance with your type.
        // For example, a scrappy fighter will always have it's best stat be willpower. Then we add a bonus for their best stat and a minus to their worst.
        statRolls.Sort();

        switch (fighterType){
            case "Scrappy":
                power = statRolls[1];
                toughness = statRolls[0] - 2;
                agility = statRolls[2] + 1;
                willpower = statRolls[3] + 3;
                break;
            case "Bulwark":
                power = statRolls[2] + 1;
                toughness = statRolls[3] + 3;
                agility = statRolls[0] - 2;
                willpower = statRolls[1];
                break;
            case "Professional":
                power = statRolls[2] + 1;
                toughness = statRolls[1];
                agility = statRolls[3] + 3;
                willpower = statRolls[0] - 2;
                break;
            case "Brawler":
                power = statRolls[3] + 3;
                toughness = statRolls[2] + 1;
                agility = statRolls[0] - 2;
                willpower = statRolls[1];
                break;
        }

        powerText.text = power.ToString();
        toughnessText.text = toughness.ToString();
        agilityText.text = agility.ToString();
        willpowerText.text = willpower.ToString();

        HP = toughness * 5;

    }

}
