using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47Script : Weapon {

    public GameObject cover;

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


    void Start () {
        shootSound = gameObject.GetComponent<AudioSource>();

        zoomSensivity = 0.9f;
        verticalRebound = 36f;
        horizontalRebound = 10f;
        zoomFov = 45;
        positions = new float[2, 3] {
            {0.1f,  -0.15f,  0.35f},
            {0,     -0.1f, 0.3f}
        };
        bulletSpeed = 380;

        objX = positions[0, 0];
        objY = positions[0, 1];
        objZ = positions[0, 2];
        rotX = gameObject.transform.localRotation.x;
        rotY = gameObject.transform.localRotation.y;
        rotZ = gameObject.transform.localRotation.z;

        bullet = Resources.Load("Ammunition/Bullet 7.62/Mesh/Bullet") as GameObject;
        shell = Resources.Load("Ammunition/Bullet 7.62/Mesh/Shell") as GameObject;
        shootParticle = Resources.Load("Particles/WFX_MF FPS RIFLE1") as GameObject;
    }
	
	void Update () {

        if(enableState == 0) {
            if(zoom) {
                Zoom();
            } else {
                Unzoom();
            }
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
    /*
    public override bool Shoot() {
        if(shootable) {
            shootable = false;

            GameObject obj = Instantiate(bullet);
            obj.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.48f + gameObject.transform.up * 0.03f;
            obj.transform.rotation = gameObject.transform.rotation;
            obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            obj.AddComponent<BoxCollider>();
            obj.AddComponent<BulletScript>();
            Destroy(obj, 10);
            Rigidbody r = obj.AddComponent<Rigidbody>();
            r.interpolation = RigidbodyInterpolation.Interpolate;
            r.AddForce(obj.transform.forward * bulletSpeed, ForceMode.VelocityChange);
            r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            obj = Instantiate(shootParticle);
            obj.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f;
            obj.transform.rotation = gameObject.transform.rotation;
            Destroy(obj, 1);
            
            reboundZ = 0.12f;
            reboundRotX = 3f;

            return true;
        }

        return false;
    }*/

    public override bool Shoot(Vector3 point) {


        if(shootable) {
            if(Vector3.Distance(point, gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f) < 1) {
                point = Vector3.zero;
            }
            shootable = false;

            GameObject obj = Instantiate(bullet);
            obj.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f;
            if(point != Vector3.zero)
                obj.transform.LookAt(point);
            else
                obj.transform.rotation = gameObject.transform.rotation;
            obj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            obj.AddComponent<BoxCollider>();
            obj.AddComponent<BulletScript>();
            Destroy(obj, 10);
            Rigidbody r = obj.AddComponent<Rigidbody>();
            r.interpolation = RigidbodyInterpolation.Interpolate;
            r.AddForce(obj.transform.forward * bulletSpeed, ForceMode.VelocityChange);
            r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            obj = Instantiate(shootParticle);
            obj.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f;
            obj.transform.rotation = gameObject.transform.rotation;
            Destroy(obj, 1);

            shootSound.Play();

            reboundZ = 0.12f;
            reboundRotX = 3;

            return true;
        }

        return false;
    }

    public override void Prepare() {

    }

    public override bool NeedPrepare() {
        return false;
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
            reboundZ -= 1f * Time.deltaTime;
        }

        if(reboundZ < 0) {
            reboundZ = 0;
        }

        if(reboundRotX > 0) {
            reboundRotX -= 30f * Time.deltaTime;
        }

        if(reboundRotX < 0) {
            reboundRotX = 0;
        }

        if(reboundZ == 0 && reboundRotX == 0) {
            shootable = true;
        }
    }

    protected override void Enable() {

    }

    protected override void Disable() {

    }

    public override void SetEnable(bool b) {
        gameObject.SetActive(b);
    }

}
