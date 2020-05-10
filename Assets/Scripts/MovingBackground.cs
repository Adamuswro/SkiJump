using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour {

    public GameObject player;

    private Rigidbody2D rb;
    private Rigidbody2D playerRB;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        playerRB = player.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity = new Vector2(-playerRB.velocity.x * 0.15f, -playerRB.velocity.y * 0.15f) ;
    }
}
