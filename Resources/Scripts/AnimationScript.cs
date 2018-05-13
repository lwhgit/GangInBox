using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {

    private Texture[] textures = new Texture[46];
    private Material material;
    private MeshRenderer mRenderer;
    private float tick = 0;
    private float speed = 15f;

    // Use this for initialization
    void Start() {
        material = Resources.Load("Materials/LGIMaterial") as Material;

        for (int i = 0;i < 46;i++) {
            textures[i] = Resources.Load("Textures/lgi/_lgi__0" + (i + 1)) as Texture;
        }
    }

    // Update is called once per frame
    void Update() {
        tick += speed * Time.deltaTime;
        material.mainTexture = textures[(int) (tick)];
        if (tick > 44) {
            tick = 0;
        }
    }
}
