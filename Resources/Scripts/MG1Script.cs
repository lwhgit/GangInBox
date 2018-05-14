using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG1Script : MonoBehaviour {

    private GameObject lgi;
    public GameObject numView;

    private TextMesh tm;

    private int count = 0;
    private int num = 0;

    private float tick = 0;

    private bool running = false;
    
	void Start () {
		lgi = Resources.Load("etc/LeeGangIn") as GameObject;
        tm = numView.GetComponent<TextMesh>();
    }
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Keypad1)) {
            running = !running;

            if (running) {
                tick = 0;
            }
        }

        if (running) {
            tick += Time.deltaTime;

            if(tick >= 2.5f) {
                count = Random.Range(1, 4);

                for(int i = 0;i < count;i++) {
                    GameObject obj = Instantiate(lgi);
                    obj.transform.position = gameObject.transform.position + new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(10f, 20f));
                    obj.AddComponent<LGIScript>();
                    Rigidbody r = obj.GetComponent<Rigidbody>();
                    r.maxAngularVelocity = 80;
                    r.AddForce(new Vector3(Random.Range(-8f, 8f), Random.Range(8f, 12f), Random.Range(-8f, 8f)), ForceMode.VelocityChange);
                    r.AddTorque(new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)), ForceMode.VelocityChange);
                    Destroy(obj, 10);
                    tick = 0;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        num++;
        tm.text = "" + num;
    }
}
