using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class SpecialResourcePrefab : MonoBehaviour {
    private Text[] textarguments;

    private GameManager gameManager;
    private Game game;


    void Awake()
    {
        //Debug.Log("call ProPre");
        textarguments = gameObject.GetComponentsInChildren<Text>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject MakeItem(ISpecialResource SR)
    {
        gameManager = GameManager.Instance;
        game = gameManager.Game;

        string nameofProduction = SpecialResourceTraits.GetSpecialResourceName(SR);

        foreach (Text txt in textarguments)
        {
            switch (txt.name)
            {
                case "ResourceName":
                    txt.text = nameofProduction + " ";
                    break;
                case "ResourceNum":
                    txt.text = "개수: " + Convert.ToInt32(game.PlayerInTurn.SpecialResource[SR]);
                    break;
            }
        }

        return this.gameObject;
    }
}
