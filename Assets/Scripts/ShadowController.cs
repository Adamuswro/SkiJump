using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour {
    public float xOffset;
    public float yOffset;

    private GameObject _playerObject;
    private PlayerController _player;
    private Transform[] _playerComponents;
    private Transform[] _shadowComponents;
    // Use this for initialization
    void Start () {
        _playerObject = GameObject.Find("Player");
        _playerComponents = _playerObject.GetComponentsInChildren<Transform>();
        _shadowComponents = GetComponentsInChildren<Transform>();
        _player = _playerObject.GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit2D hit = Physics2D.Raycast(_player.transform.position, -Vector2.up,100, 1 << LayerMask.NameToLayer("HillSide"));
        if (hit.collider != null)
        {
            if (_shadowComponents.Length == _playerComponents.Length)
            { 
                for (int i=0; i<10; i++)
                {
                    _shadowComponents[i].localPosition = _playerComponents[i].localPosition;
                    _shadowComponents[i].rotation = _playerComponents[i].rotation;
                    _shadowComponents[i].gameObject.SetActive(_playerComponents[i].gameObject.activeSelf);
                }
            }
            transform.localEulerAngles = new Vector3(180, 0,- _playerObject.transform.localEulerAngles.z);
            transform.position = hit.point - new Vector2(xOffset, yOffset);
        }
        
    }
}
