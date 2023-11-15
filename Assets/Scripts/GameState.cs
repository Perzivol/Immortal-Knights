using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum Character {DonkeyKong, Link, None};
    public static Character P1_Character;
    public static Character P2_Character;

    public static GameObject P1;
    public static GameObject P2;

    public static int lives;
    public static int time;

    public static List<string> winners = new List<string>();

    private static bool init = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void Init()
    {
        P1 = GameObject.Find(P1_Character.ToString());
        P2 = GameObject.Find(P2_Character.ToString());

        if(!init)
        {
            winners.Add(P1_Character.ToString());
            winners.Add(P2_Character.ToString());
            init = true;
        }

        if(P1 != null)
        {
            P1.tag = "Player";
        }

        if(P2 != null)
        {
            P2.tag = "AI";
        }
    }

    public static void ResetGame()
    {
        init = false;
        Init();
    }

   
    

}
