using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGScript : Weapon {
    
    private GameObject loadedWarhead;

    private AudioSource shootSound;

    private bool zoom = false;

    private float tick = 0;

    private float objX = 0;
    private float objY = 0;
    private float objZ = 0;
    private float rotX = 0;
    private float rotY = 0;
    private float rotZ = 0;

    private float reboundX = 0;
    private float reboundY = 0;
    private float reboundZ = 0;

    private float reboundRotX = 0;
    private float reboundRotY = 0;
    private float reboundRotZ = 0;

    private int prepareState = 0;

    void Start() {
        shootSound = gameObject.GetComponent<AudioSource>();

        zoomSensivity = 0.3f;
        verticalRebound = 8f;
        horizontalRebound = 1.6f;
        zoomFov = 30;
        positions = new float[2, 3] {
            {0.25f,  -0.1f,  0f},
            {0.001f,     -0.22f, 0f}
        };
        bulletSpeed = 100;

        objX = positions[0, 0];
        objY = positions[0, 1];
        objZ = positions[0, 2];
        rotX = gameObject.transform.localRotation.x;
        rotY = gameObject.transform.localRotation.y;
        rotZ = gameObject.transform.localRotation.z;

        bullet = Resources.Load("Weapons/RPG/warhead") as GameObject;
        shootParticle = Resources.Load("Particles/WFX_MF FPS RIFLE1") as GameObject;

        loadedWarhead = gameObject.transform.FindChild("warhead").gameObject;
    }

    void Update() {
        tick += Time.deltaTime;

        if(enableState == 0) {
            if(zoom) {
                Zoom();
            } else {
                Unzoom();
            }
        }


        if(prepareState == 1) {
            SpawnWarhead();
        } else if(prepareState == 2) {
            LoadWarhead();
        }

        Rebound();
        gameObject.transform.localPosition = new Vector3(objX, objY, objZ) + new Vector3(reboundX, reboundY, -reboundZ);
        gameObject.transform.localRotation = Quaternion.Euler(rotX - reboundRotX, rotY, rotZ);

        if(enableState == 1) {
            Enable();
        } else if(enableState == 2) {
            Disable();
        }
    }

    public override bool Shoot(Vector3 point) {

        
        if(shootable) {
            if(Vector3.Distance(point, gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f) < 1) {
                point = Vector3.zero;
            }
            shootable = false;

            loadedWarhead.transform.parent = null;

            if(point != Vector3.zero)
                loadedWarhead.transform.LookAt(point);
            else
                loadedWarhead.transform.rotation = gameObject.transform.rotation;

            loadedWarhead.AddComponent<BoxCollider>();
            loadedWarhead.AddComponent<WarheadScript>();
            //obj.AddComponent<BulletScript>();
            Destroy(loadedWarhead, 10);
            Rigidbody r = loadedWarhead.AddComponent<Rigidbody>();
            r.interpolation = RigidbodyInterpolation.Interpolate;
            r.AddForce(loadedWarhead.transform.forward * bulletSpeed, ForceMode.VelocityChange);
            r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            loadedWarhead = Instantiate(shootParticle);
            loadedWarhead.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f;
            loadedWarhead.transform.rotation = gameObject.transform.rotation;
            Destroy(loadedWarhead, 1);

            shootSound.Play();

            reboundZ = 0.2f;
            reboundRotX = 5;

            return true;
        }

        return false;
    }

    public override void Prepare() {
        if(prepareState == 0 && !shootable)
            prepareState = 1;
    }

    public override bool NeedPrepare() {
        return true;
    }

    public override bool SetZoom(bool b) {
        zoom = b;

        return true;
    }

    protected override void Zoom() {
        float xCurGap = objX - positions[1, 0];
        float yCurGap = objY - positions[1, 1];
        float zCurGap = objZ - positions[1, 2];
        float xGap = positions[1, 0] - positions[0, 0];
        float yGap = positions[1, 1] - positions[0, 1];
        float zGap = positions[1, 2] - positions[0, 2];
        float deltaX = xGap * 8f * Time.deltaTime;
        float deltaY = yGap * 8f * Time.deltaTime;
        float deltaZ = zGap * 8f * Time.deltaTime;
        objX = objX + deltaX;
        objY = objY + deltaY;
        objZ = objZ + deltaZ;

        if(Mathf.Abs(xCurGap) <= Mathf.Abs(deltaX)) {
            objX = positions[1, 0];
        }
        if(Mathf.Abs(yCurGap) <= Mathf.Abs(deltaY)) {
            objY = positions[1, 1];
        }
        if(Mathf.Abs(zCurGap) <= Mathf.Abs(deltaZ)) {
            objZ = positions[1, 2];
        }

    }

    protected override void Unzoom() {
        float xCurGap = objX - positions[0, 0];
        float yCurGap = objY - positions[0, 1];
        float zCurGap = objZ - positions[0, 2];
        float xGap = positions[1, 0] - positions[0, 0];
        float yGap = positions[1, 1] - positions[0, 1];
        float zGap = positions[1, 2] - positions[0, 2];
        float deltaX = xGap * 8f * Time.deltaTime;
        float deltaY = yGap * 8f * Time.deltaTime;
        float deltaZ = zGap * 8f * Time.deltaTime;
        objX = objX - deltaX;
        objY = objY - deltaY;
        objZ = objZ - deltaZ;

        if(Mathf.Abs(xCurGap) <= Mathf.Abs(deltaX)) {
            objX = positions[0, 0];
        }
        if(Mathf.Abs(yCurGap) <= Mathf.Abs(deltaY)) {
            objY = positions[0, 1];
        }
        if(Mathf.Abs(zCurGap) <= Mathf.Abs(deltaZ)) {
            objZ = positions[0, 2];
        }
    }

    protected override void Rebound() {
        if(reboundZ > 0) {
            reboundZ -= 0.3f * Time.deltaTime;
        }

        if(reboundZ < 0) {
            reboundZ = 0;
        }

        if(reboundRotX > 0) {
            reboundRotX -= 8f * Time.deltaTime;
        }

        if(reboundRotX < 0) {
            reboundRotX = 0;
        }
    }

    protected override void Enable() {

    }

    protected override void Disable() {

    }

    public override void SetEnable(bool b) {
        //enableState = b ? 1 : 2;
        gameObject.SetActive(b);
    }

    private void SpawnWarhead() {
        loadedWarhead = Instantiate(bullet);
        loadedWarhead.transform.parent = gameObject.transform;
        loadedWarhead.transform.localPosition = new Vector3(0, 0.0002f, 0.02f);
        loadedWarhead.transform.localRotation = Quaternion.Euler(0, 0, 0);

        prepareState = 2;
    }

    private void LoadWarhead() {
        float z = loadedWarhead.transform.localPosition.z;

        z -= 0.01f * Time.deltaTime;

        if (z < 0.012f) {
            z = 0.012f;
            prepareState = 0;
            shootable = true;
        }

        loadedWarhead.transform.localPosition = new Vector3(0, 0.0002f, z);
    }
}
