using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraScript : MonoBehaviour {

	private GameObject bullet;

    private float mouseSensitivity = 1;
    private float cameraRotX = 0;
    private float cameraRotY = 0;

    void Start () {
        bullet = gameObject.transform.parent.gameObject;
	}
	
	void Update () {
        //Mouse();
	}

    private void Mouse() {
        cameraRotY = Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraRotX = Input.GetAxis("Mouse Y") * mouseSensitivity;
        if(cameraRotX > 90) {
            cameraRotX = 90;
        } else if(cameraRotX < -90) {
            cameraRotX = -90;
        }
        gameObject.transform.RotateAround(bullet.transform.position, Vector3.right, -cameraRotX * (((int) gameObject.transform.eulerAngles.y) / 90 % 2 == 1 ? -1 : 1));
        gameObject.transform.RotateAround(bullet.transform.position, Vector3.up, cameraRotY);
        gameObject.transform.LookAt(bullet.transform);

        Debug.Log(((int) gameObject.transform.eulerAngles.y) + "\t" + ((int) gameObject.transform.eulerAngles.y));
    }
}
