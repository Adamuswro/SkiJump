using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private GameManager _gameManager;

    private void Init()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Menu cannot acces Game Manager component!");
    }

    public void Restart()
    {
        if (_gameManager == null)
            Init();
        if (_gameManager != null)
        {
            CompetitionManager competitionManager = _gameManager.ActualCompetition;
            if (competitionManager != null)
                competitionManager.RestartCompetition();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        if (_gameManager == null)
            Init();
        if (_gameManager != null)
        {
            CompetitionManager competitionManager = _gameManager.ActualCompetition;
            if (competitionManager != null)
                _gameManager.EndCompetition();
            else 
                SceneManager.LoadScene("Menu");
        }
    }

    public void StartTraining()
    {
        if (_gameManager == null)
            Init();
        if (_gameManager != null) _gameManager.StartTraining(hills.Prototype);
    }

    public void StartHotSeat()
    {
        if (_gameManager == null)
            Init();
        if (_gameManager != null) _gameManager.StartHotSeat(hills.Prototype);
    }
}
