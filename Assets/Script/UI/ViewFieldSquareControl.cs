using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFieldSquareControl : MonoBehaviour {
    Vector3 Scaler;
    int zoom_counter;
	// Use this for initialization
	void Start () {
        Scaler = new Vector3(0.22f, 0.22f, 0.22f);
        zoom_counter = 2;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Mouse ScrollWheel") < 0 && zoom_counter < 2)
        {
            GetComponent<Transform>().localScale += Scaler;
            transform.Translate(0, 0, 2.5f);
            zoom_counter++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && zoom_counter > 0)
        {
            GetComponent<Transform>().localScale -= Scaler;
            transform.Translate(0, 0, -2.5f);
            zoom_counter--;
        }
    }
}
