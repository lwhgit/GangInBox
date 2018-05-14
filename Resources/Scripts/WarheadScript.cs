using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarheadScript : MonoBehaviour {

    private GameObject hitParticle;
    private GameObject flyParticle;

    private float tick = 0;

    void Start() {
        hitParticle = Resources.Load("Particles/Explosion single fallback") as GameObject;
        flyParticle = Resources.Load("Particles/WFX_ExplosiveSmoke Small") as GameObject;

    }

    void Update() {
        tick += Time.deltaTime;
        Debug.Log(tick);
        if (tick > 0.1f && ((int) tick) % 20 == 0) {
            GameObject particle = Instantiate(flyParticle);
            particle.transform.position = gameObject.transform.position;
            particle.transform.position = gameObject.transform.position;
            Destroy(particle, 10);
        }
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
        Destroy(p, 6);

        Destroy(gameObject);
    }
}
