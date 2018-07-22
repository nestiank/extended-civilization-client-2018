using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;

public class AlarmManager : MonoBehaviour
{
    public GameObject Alarm;
    public GameObject AlarmQueue;
    private List<GameObject> AlarmQList;

	// Use this for initialization
	void Start ()
    {
        AlarmQList = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (AlarmQueue.transform.childCount <= 0 )
            Alarm.SetActive(false);
	}
}
