using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public int boundary = 5;
	public int speed = 50;

	private int screen_height = Screen.height;
	private int screen_width = Screen.width;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < 150) { // 150 은 추후에 적절히 작은 값으로 수정 필요.
			Camera.main.transform.Translate(0, 0, -10);
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > 15) {
			Camera.main.transform.Translate(0, 0, 10);
		}
		if (Input.mousePosition.x > screen_width - boundary) {
			Camera.main.transform.Translate(2 * speed * Time.deltaTime, 0, 0);
		}
		if (Input.mousePosition.x < 0 + boundary) {
			Camera.main.transform.Translate(-2 * speed * Time.deltaTime, 0, 0);
		}
		if (Input.mousePosition.y > screen_height - boundary) {
			Camera.main.transform.Translate(0 , speed * Time.deltaTime * Mathf.Sqrt(3), speed * Time.deltaTime);
		}
		if (Input.mousePosition.y < 0 + boundary) {
			Camera.main.transform.Translate(0, -speed * Time.deltaTime * Mathf.Sqrt(3), -speed * Time.deltaTime);
		}
	}
}
