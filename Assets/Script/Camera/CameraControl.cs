using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public int boundary = 5;
	public int speed = 10;

	private int screen_height = Screen.height;
	private int screen_width = Screen.width;

    int MaxHeight = 30; //최대 높이
    int MinHeight = 15; //최소 높이

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < MaxHeight) {
	        Camera.main.transform.Translate(0, 0, -10);
        }
        else if (Camera.main.transform.position.y > MaxHeight)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MaxHeight, Camera.main.transform.position.z);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > MinHeight) {
            Camera.main.transform.Translate(0, 0, 10);
        }
        else if (Camera.main.transform.position.y < MinHeight)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MinHeight, Camera.main.transform.position.z);
        }

        if (Input.mousePosition.x > screen_width - boundary) {
            if (Camera.main.transform.position.x <= 220)
                Camera.main.transform.Translate(2 * speed * Time.deltaTime * Mathf.Abs(Input.mousePosition.x - (screen_width - boundary)) * (float)0.015 , 0, 0);

            else
                Camera.main.transform.position = new Vector3(5, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }

		if (Input.mousePosition.x < 0 + boundary) {
            if(Camera.main.transform.position.x >= 0)
			    Camera.main.transform.Translate(-2 * speed * Time.deltaTime * Mathf.Abs(Input.mousePosition.x - boundary) * (float)0.015 , 0, 0);

            else
                Camera.main.transform.position = new Vector3(215, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }




		if (Input.mousePosition.y > screen_height - boundary) {
            if(Camera.main.transform.position.z < -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
			    Camera.main.transform.Translate(0 , speed * Time.deltaTime * Mathf.Sqrt(3) * Mathf.Abs(Input.mousePosition.y - (screen_height - boundary)) * (float)0.015, speed * Time.deltaTime * Mathf.Abs(Input.mousePosition.y - (screen_height - boundary)) * (float)0.015);
        }
        else if(Camera.main.transform.position.z > -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3));



        if (Input.mousePosition.y < 0 + boundary) {
            if (Camera.main.transform.position.z > -120)
                Camera.main.transform.Translate(0, -speed * Time.deltaTime * Mathf.Sqrt(3) * Mathf.Abs(Input.mousePosition.y - boundary) * (float)0.015, -speed * Time.deltaTime * Mathf.Abs(Input.mousePosition.y - boundary) * (float)0.015);
        }
        else if(Camera.main.transform.position.z < -120)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -120);
    }
}
