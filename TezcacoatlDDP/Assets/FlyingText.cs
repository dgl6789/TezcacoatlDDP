using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlyingText : MonoBehaviour {

    [SerializeField] float Lifetime;
    float t;

    float lerpTime;

    [SerializeField] TextMeshPro text;
    [SerializeField] Vector2 velocityRange;
    float velocity;
    [SerializeField] Vector2 rotationRange;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, Lifetime);

        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(rotationRange.x, rotationRange.y)));
        velocity = Random.Range(velocityRange.x, velocityRange.y);

        t = Lifetime;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.up * (velocity * Time.deltaTime);
        velocity *= 0.95f;

		if(t > 0) {
            t -= Time.deltaTime;

            lerpTime = t / Lifetime;

            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - Mathf.Lerp(1f, 0f, lerpTime));
        }
	}
}
