using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : MonoBehaviour {

    bool flying = false;
    Animator anim;

    [SerializeField] Vector2 angularVelocityRange;
    float angVelocity;
    float a;

    [HideInInspector] public Vector3 desiredPos;
    Vector3 initPos;
    [SerializeField] float smoothing;
    float s;

	// Use this for initialization
	void Start () {
        initPos = transform.position;

        anim = GetComponent<Animator>();
        a = 0;
        s = 0;
	}
	
	// Update is called once per frame
	void Update () {
        s += Mathf.Clamp01(Time.deltaTime * smoothing);

        if (flying) {
            a = (a + angVelocity) % 360;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, a));
        } else {
            transform.position = Vector3.Lerp(initPos, desiredPos, s);
        }
	}

    public void DoStepUp() {
        s = 0;
        initPos = transform.position;
    }

    public void DoFly() {
        flying = true;

        angVelocity = Random.Range(angularVelocityRange.x, angularVelocityRange.y);
        GetComponent<SpriteRenderer>().sortingOrder = -99;

        Destroy(gameObject, 2.25f);
    }
}
