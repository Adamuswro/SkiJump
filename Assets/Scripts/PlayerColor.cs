using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerColor : MonoBehaviour {
    public jumperBody JumperBody;
    private Color _bodyColor;
    private SpriteRenderer _spriteRenderer;
    private GameManager _gameManager;

	void Awake() {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
        {
            Debug.Log(this.gameObject.name + " couldn't acces Game Manager Component!");
            SceneManager.LoadScene("Loading");
            return;
        }
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();

        _bodyColor = new Color();
        _bodyColor = _gameManager.Player.Cloth.GetColor(JumperBody);
        _spriteRenderer.color = _bodyColor;
    }
	
    public void ColorUpdate()
    {
        _bodyColor = _gameManager.Player.Cloth.GetColor(JumperBody);
        _spriteRenderer.color = _bodyColor;
    }
}
