using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodWind : MonoBehaviour {
    public float lifeTime;

    private float startAlfa;
    private float lifeLeft;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private SpriteRenderer sprite;

	// Use this for initialization
    void Start ()
    {
        lifeLeft = lifeTime;
        sprite = GetComponent<SpriteRenderer>();
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        startAlfa = sprite.color.a;
        transform.Rotate(new Vector3(0, 0, -45f));
        rb.velocity = playerRb.velocity;
    }

    void Update()
    {
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0)
            Destroy (gameObject);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (lifeLeft / lifeTime) * startAlfa);
    }

}
