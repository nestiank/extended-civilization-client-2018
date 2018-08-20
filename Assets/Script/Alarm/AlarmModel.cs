using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AlarmModel : MonoBehaviour, IPointerClickHandler {

    [HideInInspector]
    public Sprite alarmImage = null;
    [HideInInspector]
    public String alarmText = null;
    [HideInInspector]
    public Action alarmAction = null;
    [HideInInspector]
    public int leftTurn;

    public Image imageAlarm;
    public Text textAlarm;
    public Image isDiedImage;

    public void SetProperties(Sprite image, String text, Action action, int turn)
    {
        alarmImage = image;
        alarmText = text;
        alarmAction = action;
        leftTurn = turn;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DispAlarmData()
    {
        DispAlarmData(false);
    }

    public void DispAlarmData(bool isDied)
    {
        isDiedImage.gameObject.SetActive(isDied);
        imageAlarm.sprite = alarmImage;
        textAlarm.text = alarmText;
        if (alarmAction != null)
            GetComponent<Button>().onClick.AddListener(() => alarmAction());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AlarmManager.Instance.DeleteAlarm(this.gameObject);
        //if (eventData.button == PointerEventData.InputButton.Right)
            //AlarmManager.Instance.DeleteAlarm(this.gameObject);
    }

}
