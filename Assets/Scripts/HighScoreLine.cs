using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HighScoreLine : MonoBehaviour
{
    public GameObject HillEnd;
    public Text highScoreText;
    // Use this for initialization

    
    void Update()
    {
        if (Input.GetKey(KeyCode.H))
            ResetHighScore();
    }

        void Start () {
        float highScore;

        highScore = PlayerPrefs.GetFloat("HighScore");

        if (highScore!=0)
        {
            moveLine();
        }
        //highScoreText.text = null;
    }
	
    
    private void moveLine()
    {
        float x, y;

        x = PlayerPrefs.GetFloat("HighScoreX");
        y = PlayerPrefs.GetFloat("HighScoreY");

        transform.position = new Vector2(x, y);
    }

    private void moveLine(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }

    public void ResetHighScore()
    {
        PlayerPrefs.SetFloat("HighScore", 0);
        PlayerPrefs.SetFloat("HighScoreX", 0);
        PlayerPrefs.SetFloat("HighScoreY", 0);
    }

    public void SetHighScore (float x, float y, float distace)
    {
        PlayerPrefs.SetFloat("HighScore", distace);
        PlayerPrefs.SetFloat("HighScoreX", x);
        PlayerPrefs.SetFloat("HighScoreY", y);
        StartCoroutine(HighScoreText());
    }  

    private IEnumerator HighScoreText()
    {
        bool show=false;

        while (true)
        {
            if (!show)
            {
                highScoreText.text = "New high score!";
                show = true;
            }
            else if (show)
            {
                highScoreText.text = "";
                show = false;
            }
                yield return new WaitForSeconds(0.5f);
        }
    }
}

