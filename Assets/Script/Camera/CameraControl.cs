using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public int boundary = 30;
	public int speed = 5;

	private int screen_height = Screen.height;
	private int screen_width = Screen.width;

    public int MaxHeight = 30; //최대 높이
    public int MinHeight = 15; //최소 높이

    private Vector3 mouse_position;

    public GameObject QuestUI;
    public GameObject ManagementUI;

	// Use this for initialization
	void Start () {
        
	}

    // Update is called once per frame
    void Update() {

        if (Camera.main.transform.position.x < 0)
            Camera.main.transform.position = new Vector3(215, Camera.main.transform.position.y, Camera.main.transform.position.z);
        if (Camera.main.transform.position.x > 220)
            Camera.main.transform.position = new Vector3(5, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (!QuestUI.activeSelf && !ManagementUI.activeSelf)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < MaxHeight)
            {
                Camera.main.transform.Translate(0, 0, -10);
            }
            else if (Camera.main.transform.position.y > MaxHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MaxHeight, Camera.main.transform.position.z);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > MinHeight)
            {
                Camera.main.transform.Translate(0, 0, 10);
            }
            else if (Camera.main.transform.position.y < MinHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MinHeight, Camera.main.transform.position.z);
            }
        }
        

        if (Input.mousePosition.x > screen_width - boundary && !Input.GetMouseButton(0)) {
            if (Camera.main.transform.position.x <= 220)
                Camera.main.transform.Translate(5 * speed * Time.deltaTime, 0, 0);
        }

		if (Input.mousePosition.x < 0 + boundary && !Input.GetMouseButton(0)) {
            if(Camera.main.transform.position.x >= 0)
			    Camera.main.transform.Translate(-5 * speed * Time.deltaTime, 0, 0);
        }



		if (Input.mousePosition.y > screen_height - boundary && !Input.GetMouseButton(0)) {
            if(Camera.main.transform.position.z < -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
			    Camera.main.transform.Translate(0 , 3 * speed * Time.deltaTime * Mathf.Sqrt(3), 3 * speed * Time.deltaTime);
        }
        else if(Camera.main.transform.position.z > -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3));



        if (Input.mousePosition.y < 0 + boundary && !Input.GetMouseButton(0)) {
            if (Camera.main.transform.position.z > -120)
                Camera.main.transform.Translate(0, -3 * speed * Time.deltaTime * Mathf.Sqrt(3), -3 * speed * Time.deltaTime);
        }
        else if(Camera.main.transform.position.z < -120)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -120);

        if(Input.GetMouseButton(0))
        {
            mouse_position = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
            Camera.main.transform.position -= mouse_position;
        }
        
    }
}



