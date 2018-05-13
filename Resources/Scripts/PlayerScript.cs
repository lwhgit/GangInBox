using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public GameObject head;
    public Camera mainCamera;
    public GameObject[] weapons;

    private Rigidbody rb;

    private Weapon weaponScript;

    private float mouseSensitivity = 1;
    private float speed = 4;
    private float cameraRotX = 0;
    private float cameraRotY = 0;
    private float reboundTick = 0;
    private float reboundDir = 0;

    private int weaponIndex = 0;

    private bool gameRunning = true;
    private bool zoom = false;

    void Start () {/*
        Time.timeScale = 0.0003f;
        Time.fixedDeltaTime = 0.000001f;*/

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !gameRunning;

        rb = gameObject.GetComponent<Rigidbody>();

        weaponScript = weapons[weaponIndex].GetComponent<Weapon>();
        Config.defaultFixedDeltaTime = Time.fixedDeltaTime;
    }
	
	void Update () {

        if(gameRunning) {
            Key();
            ZoomControl();
            Rebound();

            if(mainCamera.gameObject.activeSelf) {
                Mouse();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            gameRunning = !gameRunning;
            Cursor.lockState = gameRunning ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !gameRunning;
        }
    }

    private void Key() {
        if (Input.GetKey(KeyCode.W)) {
            gameObject.transform.position += gameObject.transform.forward * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.A)) {
            gameObject.transform.position -= gameObject.transform.right * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S)) {
            gameObject.transform.position -= gameObject.transform.forward * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.D)) {
            gameObject.transform.position += gameObject.transform.right * speed * Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(new Vector3(0, 5, 0), ForceMode.VelocityChange);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeWeapon(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeWeapon(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            ChangeWeapon(2);
        }
        /*
        if(Input.GetKeyDown(KeyCode.G)) {
            mainCamera.gameObject.SetActive(!mainCamera.gameObject.activeSelf);

            if(mainCamera.gameObject.activeSelf) {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Config.defaultFixedDeltaTime;
            }
        }*/
    }

    private void Mouse() {
        cameraRotY += Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraRotX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        if (cameraRotX > 90) {
            cameraRotX = 90;
        } else if(cameraRotX < -90) {
            cameraRotX = -90;
        }
        gameObject.transform.localRotation = Quaternion.Euler(0, cameraRotY, 0);
        head.transform.localRotation = Quaternion.Euler(cameraRotX, 0, 0);

        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        } else if(Input.GetMouseButtonUp(0)) {
            if(weaponScript.NeedPrepare()) {
                zoom = false;
                weaponScript.SetZoom(zoom);
                weaponScript.Prepare();
            }
        }

        if(Input.GetMouseButton(0) && !weaponScript.NeedPrepare()) {
            Shoot();
        }

        if(Input.GetMouseButtonDown(1)) {
            zoom = !zoom;
            weaponScript.SetZoom(zoom);
        }
    }

    private void Shoot() {
        RaycastHit hit;
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 1000f)) {
            if(weaponScript.Shoot(hit.point)) {
                reboundTick = 0.05f;
                reboundDir = Random.Range(0f, 1f) > 0.5f ? -1 : 1;
            }
        } else {
            if(weaponScript.Shoot(Vector3.zero)) {
                reboundTick = 0.05f;
                reboundDir = Random.Range(0, 1) > 0.5f ? -1 : 1;
            }
        }
    }

    private void ZoomControl() {
        if(zoom) {
            mouseSensitivity = weaponScript.GetZoomSensivity();
            mainCamera.fieldOfView -= 400 * Time.deltaTime;

            if(mainCamera.fieldOfView < weaponScript.GetZoomFov()) {
                mainCamera.fieldOfView = weaponScript.GetZoomFov();
            }
        } else {
            mouseSensitivity = 1;
            mainCamera.fieldOfView += 400 * Time.deltaTime;

            if(mainCamera.fieldOfView > 60) {
                mainCamera.fieldOfView = 60;
            }
        }
    }

    private void Rebound() {
        if (reboundTick > 0) {
            cameraRotX -= Random.Range(weaponScript.GetVerticalRebound() / 2, weaponScript.GetVerticalRebound()) * Time.deltaTime;
            cameraRotY += Random.Range(weaponScript.GetHorizontalRebound() / 2, weaponScript.GetHorizontalRebound()) * Time.deltaTime * reboundDir;
        }
        reboundTick -= Time.deltaTime;
    }

    private void ChangeWeapon(int index) {
        zoom = false;
        weaponScript.SetZoom(false);
        weaponScript.SetEnable(false);
        weaponIndex = index;

        weaponScript = weapons[weaponIndex].GetComponent<Weapon>();
        weaponScript.SetEnable(true);
    }
}
