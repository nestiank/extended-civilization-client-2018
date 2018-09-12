using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;

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

        if (SR is CivModel.Quests.AutismBeamAmplificationCrystal)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "finno_crystal");
        else if (SR is CivModel.Quests.GatesOfRlyeh)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "finno_gate");
        else if (SR is CivModel.Quests.InterstellarEnergyExtractor)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "finno_energy");
        else if (SR is CivModel.Quests.Necronomicon)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "finno_necronimicon");
        else if (SR is CivModel.Quests.SpecialResourceAirspaceDomination)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "hwan_spacetrack");
        else if (SR is CivModel.Quests.SpecialResourceAlienCommunication)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "hwan_pyramid");
        else if (SR is CivModel.Quests.SpecialResourceAutismBeamReflex)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "hwan_autism_ray_reflection");
        else if (SR is CivModel.Quests.SpecialResourceCthulhuProjectInfo)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "hwan_cthulhu_info");
        else if (SR is CivModel.Quests.SpecialResourceMoaiForceField)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "hwan_moai");
        else if (SR is CivModel.Quests.Ubermensch)
            transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("SpecialResource/" + "finno_ubermensch");

        return this.gameObject;
    }
}
