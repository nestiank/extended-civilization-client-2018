using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject MapUI;
    public GameObject ManagementUI;
    public GameObject QuestUI;

    public void MapUIActive()
    {
        MapUI.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
    }

    public void ManagementUIActive()
    {
        ManagementUI.SetActive(true);
        MapUI.SetActive(false);
        QuestUI.SetActive(false);
    }

    public void QuestUIActive()
    {
        QuestUI.SetActive(true);
        MapUI.SetActive(false);
        ManagementUI.SetActive(false);
    }
}
