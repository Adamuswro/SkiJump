using UnityEngine;

public class Distance : MonoBehaviour {
    public GameObject target;
    private WindGenerator wind;
	// Use this for initialization
	void Start () {
        Vector2 distance = target.transform.position - transform.position;

        Debug.Log(distance.magnitude*2);

    }

}
