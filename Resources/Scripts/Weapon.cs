using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    protected GameObject bullet;
    protected GameObject shell;
    protected GameObject shootParticle;

    protected float zoomSensivity = 0;
    protected float verticalRebound = 0;
    protected float horizontalRebound = 0;
    protected float zoomFov = 0;
    protected float[,] positions;
    protected float bulletSpeed = 0;

    protected int enableState = 0;

    protected bool shootable = true;

    public float GetZoomSensivity() {
        return zoomSensivity;
    }
    //public abstract bool Shoot();
    public abstract bool Shoot(Vector3 point);
    public abstract void Prepare();
    public abstract bool NeedPrepare();
    public abstract bool SetZoom(bool b);
    public float GetZoomFov() {
        return zoomFov;
    }
    protected abstract void Zoom();
    protected abstract void Unzoom();
    protected abstract void Rebound();
    public float GetVerticalRebound() {
        return verticalRebound;
    }
    public float GetHorizontalRebound() {
        return horizontalRebound;
    }
    protected abstract void Enable();
    protected abstract void Disable();
    public abstract void SetEnable(bool b);
    public int GetEnableState() {
        return enableState;
    }
}
