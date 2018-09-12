using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

public class SkillButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(GameManager.Instance.selectedActor != null)
        {
            var skill = GameManager.Instance.selectedActor.ActiveSkills[Convert.ToInt32(gameObject.name[5] - '1')];
            UIManager.Instance.skillDescription.transform.GetChild(0).GetComponent<Text>().text = skill.SkillName + ": " + skill.SkillDescription;
            UIManager.Instance.skillDescription.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UIManager.Instance.skillDescription.SetActive(false);
    }

    public static Sprite GetSkillIcon(CivModel.Actor unit, int idx)
    {
        char[] sep = { '.' };
        string name = unit.ToString().Split(sep)[2];
        string skillIconName;
        switch (name)
        {
            case "JediKnight":
                {
                    if (unit.Owner == GameManager.Instance.Game.GetPlayerHwan())
                        skillIconName = "hwan_jedi";
                    else if (unit.Owner == GameManager.Instance.Game.GetPlayerFinno())
                        skillIconName = "finno_jedi";
                    else skillIconName = "hwan_jedi";
                }
                break;
            case "JackieChan":
                skillIconName = "hwan_jackie_chan";
                break;
            case "LEOSpaceArmada":
                skillIconName = "hwan_spaceship";
                break;
            case "ProtoNinja":
                {
                    if (unit.Owner == GameManager.Instance.Game.GetPlayerHwan())
                        skillIconName = "hwan_ninja";
                    else if (unit.Owner == GameManager.Instance.Game.GetPlayerFinno())
                        skillIconName = "finno_ninja";
                    else skillIconName = "hwan_ninja";
                }
                break;
            case "UnicornOrder":
                skillIconName = "hwan_unicorn";
                break;
            case "Spy":
                {
                    if (unit.Owner == GameManager.Instance.Game.GetPlayerHwan())
                        skillIconName = "hwan_spy";
                    else if (unit.Owner == GameManager.Instance.Game.GetPlayerFinno())
                        skillIconName = "finno_spy";
                    else skillIconName = "hwan_spy";
                }
                break;
            case "AncientSorcerer":
                skillIconName = "finno_sorcerer";
                break;
            case "AutismBeamDrone":
                skillIconName = "finno_autism";
                break;
            case "ElephantCavalry":
                skillIconName = "finno_elephant";
                break;
            case "GenghisKhan":
                skillIconName = "finno_genghis_khan";
                break;
            default:
                skillIconName = "city_range_attack";
                break;
        }

        return Resources.Load<Sprite>("SkillIcon/" + skillIconName);
    }
}
