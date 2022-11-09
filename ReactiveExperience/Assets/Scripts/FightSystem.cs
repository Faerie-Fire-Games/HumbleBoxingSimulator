using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// So this whole script manages setting up the fight environment, and then simulating each round of combat. 
// It spawns the logs, decides the outcomes of moves with dice rolls and stats
// It's surprisingly similar to DnD. This was mostly a mistake. DnD just does that to a motherfucker I guess. 
// P.S. I know that if a script is this long it REALLY needs to be divided up. I'm so sorry you have to read this.
// In my defense.. I WAS BORN STUPID *however*, I WILL not die hungry. Video Games forever, Kotaku.com

public class FightSystem : MonoBehaviour
{
    //Player and opponent stats. Name, power, toughness, agility, willpower, hp, armour class, and knocked out status. p prefix = player. o prefix = opponent
    private string pName;
    private int pPow;
    private int pTgh;
    private int pAgl;
    private int pWil;
    private int pHP;
    private int pAC; //This is the armour class. Basically your ability to tank a hit.
    private bool pKO = false;
    private string oName;
    private int oPow;
    private int oTgh;
    private int oAgl;
    private int oWil;
    private int oHP;
    private int oAC; 
    private bool oKO = false;

    public CharacterStats playerStats;
    public Opponent opponentStats;

    public Slider pSlider, oSlider;
    public GameObject logBox;
    public Transform newLogPos, logHistory;
    public int logDistance; // Dictates the distance between logs


    [SerializeField] private TMP_Text pAgilityText, oAgilityText;

    void Start()
    {
        GetPlayerStats();
        NewOpponent();
        NextRound("RoundStart", "RoundStart");
    }

    // Here it is (:
    // The 285 line function!
    // This is where everything that happens in a round is decided.
    // This function is *called* from TechniqueUsed.cs script. The paramaters are passed in from there also. 
    public void NextRound(string pMove, string oMove)
    {
        print("New Round");

        if(!pKO && !oKO)
        {
            string report = ""; // We will add to this in the switch statement

            // So this massive switch dictates what happens when you choose a technique.
            // Techniques: Attack, Reposition, CounterAttack, Feint
            // It's fairly complex.. But hey it works!
            switch (pMove)
            {
                case "RoundStart":
                    break;

                case "Attack":
                    var feintBonus = 0;  // This just means there is a consequence for spamming feint
                    if (oMove == "Feint")
                    {
                        feintBonus = 4;
                    }

                    if(oMove != "Counter")
                    {
                        if(D20(Modifier(pAgl)) + feintBonus >= oAgl - Modifier(pPow))
                        {
                            report += "The attack lands. ";

                            if (D20(Modifier(pPow)) + feintBonus >= oAC - Modifier(pWil))
                            {
                                var damage = Random.Range(5+Modifier(pPow), 12+Modifier(pPow)) + Modifier(pWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                                print("You dealt " + damage);
                                oHP -= damage;
                                report += pName + " squarely hits " + oName + "right where it hurts!";
                            }
                            else {
                                report += "But " + oName + " blocks the hit while gritting their teeth.";
                            }
                        }
                        else
                        {
                            report += pName + " attacks, but " + oName + " is too fast. They dodge it swiftly.";
                        }

                    }
                    else
                    {
                        report += pName + " swings out, put only punches air. " + oName + "appears suddenly striking with a hook to your side!";
                    }
                    break;


                case "Counter":
                    report += pName + " watches for the incoming hit. . . ";
                    if(oMove == "Attack")
                    {
                        report += "The attack comes! " + pName + " parries the glove and returns with a jab straight to the face.";
                        var damage = Random.Range(5 + Modifier(pPow), 12 + Modifier(pPow)) + Modifier(pWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                        print("You dealt " + damage);
                        oHP -= damage;
                    }
                    else if (oMove == "Feint")
                    {
                        report += pName + " spots a punch being thrown and begins to parry. It's a feint though! With their guard down " + oName + "lands a hit.";
                    }
                    else
                    {
                        report += " But nothing comes. ";
                    }
                    break;


                case "Reposition":
                    if (oMove != "Attack")
                    {
                        report += pName + ", light on their feet, successfully repositions into a better spot. (Agility Raised)";
                        pAgl += 1;
                        pAgilityText.text = (int.Parse(pAgilityText.text) + 1).ToString();
                    }
                    else
                    {
                        report += "While trying to move " + oName + " catches " + pName + " off guard, blocking their reposition!";
                    }
                    break;


                case "Feint":
                    if (oMove == "Counter")
                    {
                        report += pName + " feints an attack causing " + oName + " to dodge left, not realising they have dodged into " + pName + "'s second attack!";
                        var damage = Random.Range(5 + Modifier(pPow), 12 + Modifier(pPow)) + Modifier(pWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                        print("You dealt " + damage);
                        oHP -= damage;
                    }
                    else
                    {
                        report += pName + " feints an attack, but " + oName + " doesn't fall for it";
                    }
                    break;
            }

            if(pMove != "RoundStart") // This just makes it so we don't start the game with two empty boxes
            {
                print(report);
                logHistory.transform.position = new Vector3(logHistory.position.x, logHistory.position.y + logDistance, logHistory.position.z);
                GameObject LogBox = Instantiate(logBox, newLogPos.position, newLogPos.rotation, logHistory.transform);
                LogBox.transform.GetChild(0).GetComponent<TMP_Text>().text = report;
                LogBox.GetComponent<Image>().color = new Color32((byte)80, (byte)156, (byte)86, (byte)130);
            }

            // THE FOLLOWING IS A LOT LIKE ABOVE
            // FUNCTIONALITY IS THE SAME. ONLY DIFFERENCE IS WORDING BECAUSE IT'S THE ENEMIES ACTIONS
            report = "";
            switch (oMove)
            {
                case "RoundStart":
                    break;

                case "Attack":
                    var feintBonus = 0; // This just means there is a consequence for spamming feint
                    if (oMove == "Feint")
                    {
                        feintBonus = 4;
                    }

                    if (pMove != "Counter")
                    {
                        if (D20(Modifier(oAgl)) + feintBonus >= oAgl - Modifier(oPow))
                        {
                            report += oName + " attcks you ferociously. ";

                            if (D20(Modifier(oPow)) + feintBonus >= pAC - Modifier(oWil))
                            {
                                var damage = Random.Range(1 + Modifier(oPow), 7 + Modifier(oPow)) + Modifier(oWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                                print("Opponent dealt " + damage);
                                pHP -= damage;
                                report += "the strike lands deftly. You wince in pain.";
                            }
                            else
                            {
                                report += "But " + pName + " remains undisturbed, blocking the hit easily.";
                            }
                        }
                        else
                        {
                            report += oName + " attacks, but " + pName + " is too fast. They dodge it swiftly.";
                        }
                    }
                    else
                    {
                        report += oName + " is reeling from the counter!";
                    }
                    break;


                case "Counter":
                    if (pMove == "Attack")
                    {
                        report += oName + " looks pleased with themself.";
                        var damage = Random.Range(5 + Modifier(oPow), 12 + Modifier(oPow)) + Modifier(oWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                        print("Opponent dealt " + damage);
                        pHP -= damage;
                    }
                    else if (pMove == "Feint")
                    {
                        report += oName + " winces and throws their guard up again.";
                    }
                    else
                    {
                        report += oName + " seems to be analysing your movements.";
                    }
                    break;


                case "Reposition":
                    if (pMove != "Attack")
                    {
                        report += oName + ", light on their feet, successfully repositions into a better spot. (Agility Raised)";
                        pAgl += 1;
                        oAgilityText.text = (int.Parse(oAgilityText.text) + 1).ToString();
                    }
                    else
                    {
                        report += "While trying to move " + pName + " catches " + oName + " off guard, blocking their reposition!";
                    }
                    break;


                case "Feint":
                    if (pMove == "Counter")
                    {
                        report += oName + " smirks at you condescendingly.";
                        var damage = Random.Range(5 + Modifier(oPow), 12 + Modifier(oPow)) + Modifier(oWil); // I know it's a weird equation. All it's really doing is buffing your damage in accorddance with power, and then giving a little boost with wil.
                        print("You dealt " + damage);
                        pHP -= damage;
                    }
                    else
                    {
                        report += oName + " feints an attack, but " + pName + " doesn't fall for it";
                    }
                    break;
            }

            if(oMove != "RoundStart") // Enemy action log box
            {
                print(report);
                logHistory.transform.position = new Vector3(logHistory.position.x, logHistory.position.y + logDistance, logHistory.position.z);
                GameObject EnemyLogBox = Instantiate(logBox, newLogPos.position, newLogPos.rotation, logHistory.transform);
                EnemyLogBox.transform.GetChild(0).GetComponent<TMP_Text>().text = report;
                EnemyLogBox.GetComponent<Image>().color = new Color32((byte)156, (byte)80, (byte)80, (byte)130);
            }

            if (D20(Modifier(pTgh)) < 10) // This simulates continually getting backed into a corner as the fight goes on. So you have to occasionally reposition. Using toughness because if you use agility death spirals occur
            {
                pAgl--;
            }
            if (D20(Modifier(oTgh)) < 10)
            {
                oAgl--;
            }

            #region CheckIfDead
            // CHECK IF DEAD
            if (pHP <= 0)
            {
                if (D20(Modifier(pWil)) >= 8)
                {
                    var saveLog = pName + " IS ON THE ROPES. BUT THEY GRIT THEIR TEETH AND HOLD ON.";
                    logHistory.transform.position = new Vector3(logHistory.position.x, logHistory.position.y + logDistance, logHistory.position.z);
                    GameObject LogBox = Instantiate(logBox, newLogPos.position, newLogPos.rotation, logHistory.transform);
                    LogBox.transform.GetChild(0).GetComponent<TMP_Text>().text = saveLog;
                    LogBox.GetComponent<Image>().color = new Color32((byte)162, (byte)159, (byte)70, (byte)130);
                }
                else
                {
                    pKO = true;
                }
            }
            if (oHP <= 0)
            {
                if (D20(Modifier(oWil)) >= 8)
                {
                    var saveLog = oName + " IS ON THE ROPES. BUT THEY GRIT THEIR TEETH AND HOLD ON.";
                    logHistory.transform.position = new Vector3(logHistory.position.x, logHistory.position.y + logDistance, logHistory.position.z);
                    GameObject LogBox = Instantiate(logBox, newLogPos.position, newLogPos.rotation, logHistory.transform);
                    LogBox.transform.GetChild(0).GetComponent<TMP_Text>().text = saveLog;
                    LogBox.GetComponent<Image>().color = new Color32((byte)162, (byte)159, (byte)70, (byte)130);
                }
                else
                {
                    oKO = true;
                }
            }

            if (pKO)
            {
                SceneManager.LoadScene(0); // I know it's unceremonious but this is already a late submission.
            }
            if (oKO)
            {
                var saveLog = oName + " HIT'S THE GROUND UNCONCIOUS!\nYOU WIN!!!";
                logHistory.transform.position = new Vector3(logHistory.position.x, logHistory.position.y + logDistance, logHistory.position.z);
                GameObject LogBox = Instantiate(logBox, newLogPos.position, newLogPos.rotation, logHistory.transform);
                LogBox.transform.GetChild(0).GetComponent<TMP_Text>().text = saveLog;
                LogBox.GetComponent<Image>().color = new Color32((byte)162, (byte)159, (byte)70, (byte)130);
                pHP = playerStats.HP;
                pAgl = playerStats.agility;
                NewOpponent();
                //NextRound("RoundStart", "RoundStart");
                oKO = false;
            }
            #endregion

            pSlider.value = pHP;
            oSlider.value = oHP;
            
            

            pAgilityText.text = pAgl.ToString();
            oAgilityText.text = oAgl.ToString();

        }

    }

    void GetPlayerStats()
    {
        pName = playerStats.fighterName;
        pPow = playerStats.power;
        pTgh = playerStats.toughness;
        pAgl = playerStats.agility;
        pWil = playerStats.willpower;
        pHP = playerStats.HP;
        pAC = 10 + Modifier(pTgh);
        print("Player AC " + pAC);

        pSlider.maxValue = pHP;
        pSlider.value = pSlider.maxValue;
    }
    
    void NewOpponent()
    {
        opponentStats.newFighter();
        oName = opponentStats.fighterName;
        oPow = opponentStats.power;
        oTgh = opponentStats.toughness;
        oAgl = opponentStats.agility;
        oWil = opponentStats.willpower;
        oHP = opponentStats.HP;
        oAC = 10 + Modifier(oTgh);
        print("Opponent AC " + oAC);

        oSlider.maxValue = oHP;
        oSlider.value = oSlider.maxValue;
    }

    int Modifier(int statValue) // This is the bonus added to each roll in accordance with your skill in that attribute. You get a bigger bonus if your raw score is better. 
    {
        if (statValue == 10)
        {
            return 0;
        }
        else {
            var inverseModifier = ((10 - statValue) / 2);
            return -inverseModifier;
        }
        // Weird algorithm here I know. It follows this pattern. Every two points above 10, the bonus increases by 1. And decreses by 1 for every 2 points below 10.
        // e.g 12 will be +1. 6 will be -2.
    }

    int D20(int modifier) //Rolls a d20, and then adds the modifier bonus.
    {
        var naturalRoll = Random.Range(1, 21);
        if (naturalRoll == 20) { return 100; } // Simulates a critical hit on a natural 20
        else { return naturalRoll + modifier; }
    }
}
