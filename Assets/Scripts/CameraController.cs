using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform target;
    private Camera cameraComponent;
    private float endScale;
    // Use this for initialization
    void Start () {
        cameraComponent = GetComponent<Camera>();
        endScale = cameraComponent.orthographicSize;
        cameraComponent.orthographicSize = 4;
    }
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (target.position.x, target.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "SpeedTriger")
            StartCoroutine(ZoomOut(1.5f));
    }

    private IEnumerator ZoomOut(float zoomTime)
    {
        float time = 0;
        while(time< zoomTime)
        {
            time += Time.deltaTime;
            cameraComponent.orthographicSize = Mathf.Lerp(4, endScale, time/ zoomTime);
            yield return null;
        }
    }
}
