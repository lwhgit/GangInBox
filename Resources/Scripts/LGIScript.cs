using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGIScript : MonoBehaviour {
    //WFX_ExplosiveSmoke

    private GameObject hitParticle;

    void Start () {
        hitParticle = Resources.Load("Particles/WFX_Explosion") as GameObject;
    }
	
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.name.StartsWith("Bullet")) {
            GameObject p = Instantiate(hitParticle);
            p.transform.position = transform.position;
            Destroy(p, 6);
            Destroy(gameObject);
        }
    }
}
