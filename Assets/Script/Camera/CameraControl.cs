using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour {

	public int boundary = 30;
	public int speed = 5;

	private int screen_height = Screen.height;
	private int screen_width = Screen.width;

    public int MaxHeight = 100; //최대 높이
    public int MinHeight = 15; //최소 높이

    private Vector3 mouse_position;

    public GameObject QuestUI;
    public GameObject ManagementUI;
    public GameObject MapUI;

	// Use this for initialization
	void Start () {
        
	}

    // Update is called once per frame
    void Update()
    {

        if (Camera.main.transform.position.x < 0)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + Mathf.Sqrt(3) * GameManager.Instance.Game.Terrain.Width, Camera.main.transform.position.y, Camera.main.transform.position.z);
        if (Camera.main.transform.position.x > Mathf.Sqrt(3) * GameManager.Instance.Game.Terrain.Width)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - Mathf.Sqrt(3) * GameManager.Instance.Game.Terrain.Width, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (!QuestUI.activeSelf && !ManagementUI.activeSelf)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < MaxHeight)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    Camera.main.transform.Translate(0, 0, -10);
            }
            else if (Camera.main.transform.position.y > MaxHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MaxHeight, Camera.main.transform.position.z);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > MinHeight)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    Camera.main.transform.Translate(0, 0, 10);
            }
            else if (Camera.main.transform.position.y < MinHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, MinHeight, Camera.main.transform.position.z);
            }



            if (Input.mousePosition.x > screen_width - boundary && !Input.GetMouseButton(0))
                Camera.main.transform.Translate(5 * speed * Time.deltaTime, 0, 0);

            if (Input.mousePosition.x < 0 + boundary && !Input.GetMouseButton(0))
                Camera.main.transform.Translate(-5 * speed * Time.deltaTime, 0, 0);



            if (Input.mousePosition.y > screen_height - boundary && !Input.GetMouseButton(0))
            {
                if (Camera.main.transform.position.z < -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
                    Camera.main.transform.Translate(0, 3 * speed * Time.deltaTime * Mathf.Sqrt(3), 3 * speed * Time.deltaTime);
            }
            else if (Camera.main.transform.position.z > -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3))
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10 - (Camera.main.transform.position.y - 10) / Mathf.Sqrt(3));



            if (Input.mousePosition.y < 0 + boundary && !Input.GetMouseButton(0))
            {
                if (Camera.main.transform.position.z > -120)
                    Camera.main.transform.Translate(0, -3 * speed * Time.deltaTime * Mathf.Sqrt(3), -3 * speed * Time.deltaTime);
            }
            else if (Camera.main.transform.position.z < -120)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -120);

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                mouse_position = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
                Camera.main.transform.position -= mouse_position;
            }

            if (is_mouse_on_minimap() && MapUI.activeSelf)
            {
                float adjust_by_zoom;
                adjust_by_zoom = 24.5f - (2 - ViewFieldSquareControl.ViewInstance.zoom_counter) * 6;

                if (Input.GetMouseButton(0))
                {
                    Camera.main.transform.position = new Vector3(
                        (0 + 220 * (Input.mousePosition.x - (783.75f * Screen.width / 1022)) / (236.25f * Screen.width / 1022)), //705, 315
                        Camera.main.transform.position.y,
                        (-120f + 99 * Input.mousePosition.y / (112.5f * Screen.height / 639) - adjust_by_zoom) //150
                        );
                }
            }
        }
    }
        
    public bool is_mouse_on_minimap()
    {
        if (Input.mousePosition.x > 783.75f * Screen.width / 1022 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < 140 * Screen.height / 639)
            return true;
        else
            return false;
    }
}

