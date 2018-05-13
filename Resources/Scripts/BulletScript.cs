using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    private GameObject hitParticle;
    private GameObject bulletHole;

    private GameObject subCamera;

    void Start() {
        hitParticle = Resources.Load("Particles/WFX_BImpact Concrete") as GameObject;
        bulletHole = Resources.Load("etc/bullet_hole0") as GameObject;
        /*
        subCamera = Instantiate(Resources.Load("etc/SubCamera") as GameObject);
        subCamera.SetActive(false);
        subCamera.transform.position = gameObject.transform.position - gameObject.transform.forward * 0.3f;
        subCamera.transform.parent = gameObject.transform;
        Config.subCamera = subCamera;*/
    }

    void Update() {
        //Key();
    }

    private void OnCollisionEnter(Collision collision) {
        GameObject c = collision.gameObject;
        Vector3 normal = collision.contacts[0].normal;

        float xAngle = Mathf.Atan2(normal.y, normal.z) * 180 / Mathf.PI;
        float yAngle = Mathf.Atan2(normal.x, normal.z) * 180 / Mathf.PI;
        float zAngle = Mathf.Atan2(normal.y, normal.x) * 180 / Mathf.PI;

        GameObject p = Instantiate(hitParticle);
        p.transform.position = transform.position;
        p.transform.Rotate(xAngle, yAngle, zAngle);
        Destroy(p, 1);

        GameObject h = Instantiate(bulletHole);
        h.transform.position = collision.contacts[0].point;
        h.transform.Translate(-h.transform.forward * 0.02f);
        h.transform.Rotate(xAngle, yAngle, zAngle);
        Destroy(p, 10);

        h.transform.parent = c.transform;

        Destroy(subCamera);
        Destroy(gameObject);
    }

    private void Key() {
        if(Input.GetKeyDown(KeyCode.G) && Config.subCamera.Equals(subCamera)) {
            subCamera.SetActive(!subCamera.activeSelf);

            if(subCamera.activeSelf) {
                Time.timeScale = 0.0003f;
                Time.fixedDeltaTime = 0.000001f;
            }
        }
    }
}
