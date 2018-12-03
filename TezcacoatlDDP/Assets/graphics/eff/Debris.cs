using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour {

    [SerializeField] float lifetime;
    [SerializeField] float gravity;

    [SerializeField] Sprite[] sprites;

    [SerializeField] Vector2 velocitXRange;
    [SerializeField] Vector2 velocityYRange;
    [SerializeField] Vector2 angVelRange;
    Vector2 velocity;
    float angVel;
    float a;

	// Use this for initialization
	void Start () {
        velocity = new Vector2(Random.Range(velocitXRange.x, velocitXRange.y), Random.Range(velocityYRange.x, velocityYRange.y));

        angVel = Random.Range(angVelRange.x, angVelRange.y);
        a = Random.Range(-180f, 180f);

        Destroy(gameObject, lifetime);

        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
	}
	
	// Update is called once per frame
	void Update () {
        a += angVel;

        velocity = new Vector3(velocity.x, velocity.y - gravity, 0f);

        transform.position += (Vector3)velocity;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, a));
	}
}
