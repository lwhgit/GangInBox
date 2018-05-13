using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AWMScript : Weapon {

    public GameObject barrel;
    public GameObject barrel2;

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

        zoomSensivity = 0.1f;
        verticalRebound = 8f;
        horizontalRebound = 1.6f;
        zoomFov = 4;
        positions = new float[2, 3] {
            {0.1f,  -0.1f,  0.3f},
            {-0.00025f,     -0.0782f, 0.355f}
        };
        bulletSpeed = 480;

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
            OpenBarrel();
        } else if(prepareState == 2) {
            PullBarrel();
        } else if(prepareState == 3) {
            PushBarrel();
        } else if(prepareState == 4) {
            CloseBarrel();
        }

        Rebound();
        gameObject.transform.localPosition = new Vector3(objX, objY, objZ) + new Vector3(reboundX, reboundY, -reboundZ);
        gameObject.transform.localRotation = Quaternion.Euler(rotX - reboundRotX, rotY, rotZ);

        if (enableState == 1) {
            Enable();
        } else if (enableState == 2) {
            Disable();
        }
    }
    /*
    public override bool Shoot() {
        if(shootable) {
            shootable = false;

            GameObject obj = Instantiate(bullet);
            obj.transform.position = gameObject.transform.position + gameObject.transform.forward * 0.8f + gameObject.transform.up * 0.03f;
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

            reboundZ = 0.2f;
            reboundRotX = 3;

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
            if (point != Vector3.zero)
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

            reboundZ = 0.2f;
            reboundRotX = 3;

            return true;
        }

        return false;
    }

    public override void Prepare() {
        if (prepareState == 0 && !shootable)
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
            reboundZ -= 0.6f * Time.deltaTime;
        }

        if (reboundZ < 0) {
            reboundZ = 0;
        }

        if(reboundRotX > 0) {
            reboundRotX -= 30f * Time.deltaTime;
        }

        if(reboundRotX < 0) {
            reboundRotX = 0;
        }
    }

    protected override void Enable() {/*
        if(rotY > 0) {
            rotY -= 3 * Time.deltaTime * 220;

            if(rotY < 0) {
                rotY = 0;
            }
        }

        if(rotZ > 0) {
            rotZ -= 2 * Time.deltaTime * 220;

            if(rotZ < 0) {
                rotZ = 0;
            }
        }

        objX = Mathf.Cos((90 - rotZ - 18) * Mathf.PI / 180) * 0.316f;
        objZ = Mathf.Sin((90 - rotZ - 18) * Mathf.PI / 180) * 0.316f;

        if(rotY == 0 && rotZ == 0) {
            enableState = 0;
        }*/
    }

    protected override void Disable() {/*
        if (rotY < 270) {
            rotY += 3 * Time.deltaTime * 20;

            if (rotY > 270) {
                rotY = 270;
            }
        }

        if(rotZ < 180) {
            rotZ += 2 * Time.deltaTime * 20;

            if(rotZ > 180) {
                rotZ = 180;
            }
        }

        objX = Mathf.Cos((90 - rotZ - 18) * Mathf.PI / 180) * 0.316f;
        objZ = Mathf.Sin((90 - rotZ - 18) * Mathf.PI / 180) * 0.316f;

        if(rotY == 270 && rotZ == 180) {
            enableState = -1;
        }*/
    }

    public override void SetEnable(bool b) {
        //enableState = b ? 1 : 2;
        gameObject.SetActive(b);
    }

    private void OpenBarrel() {
        float rotX = barrel.transform.localEulerAngles.x;

        if(rotX < 315) {
            barrel.transform.Rotate(new Vector3(0f, -200f * Time.deltaTime, 0f));
        } else {
            prepareState = 2;
        }
    }

    private void CloseBarrel() {
        float rotX = barrel.transform.localEulerAngles.x;

        if(rotX > 275) {
            barrel.transform.Rotate(new Vector3(0f, 200f * Time.deltaTime, 0f));
        } else {
            prepareState = 0;
            shootable = true;
        }
    }

    private void PullBarrel() {
        float x1 = barrel.transform.localPosition.x;
        float y1 = barrel.transform.localPosition.y;
        float z1 = barrel.transform.localPosition.z;
        float x2 = barrel2.transform.localPosition.x;
        float y2 = barrel2.transform.localPosition.y;
        float z2 = barrel2.transform.localPosition.z;

        if(z2 > -0.17f) {
            z2 -= 0.4f * Time.deltaTime;
            z1 -= 0.4f * Time.deltaTime;
        } else {
            prepareState = 3;
            GameObject obj = Instantiate(shell);
            obj.transform.position = gameObject.transform.position + gameObject.transform.right * 0.04f + gameObject.transform.up * 0.03f - gameObject.transform.forward * 0.05f;
            obj.transform.rotation = gameObject.transform.rotation;
            obj.AddComponent<BoxCollider>();
            Rigidbody r = obj.AddComponent<Rigidbody>();
            r.maxAngularVelocity = 30;
            r.AddForce(obj.transform.right * 1.2f, ForceMode.VelocityChange);
            r.AddTorque(new Vector3(0, 50, 0), ForceMode.VelocityChange);
            Destroy(obj, 10);
        }

        barrel.transform.localPosition = new Vector3(x1, y1, z1);
        barrel2.transform.localPosition = new Vector3(x2, y2, z2);
    }

    private void PushBarrel() {
        float x1 = barrel.transform.localPosition.x;
        float y1 = barrel.transform.localPosition.y;
        float z1 = barrel.transform.localPosition.z;
        float x2 = barrel2.transform.localPosition.x;
        float y2 = barrel2.transform.localPosition.y;
        float z2 = barrel2.transform.localPosition.z;

        if(z2 < -0.08f) {
            z2 += 0.4f * Time.deltaTime;
            z1 += 0.4f * Time.deltaTime;
        } else {
            prepareState = 4;
        }

        barrel.transform.localPosition = new Vector3(x1, y1, z1);
        barrel2.transform.localPosition = new Vector3(x2, y2, z2);
    }
}
