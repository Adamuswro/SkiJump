using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotSeatCompetition: CompetitionManager
{
    private int _currentPlayerIndex;
    public bool HasWind;

    public float WindStrenght { get; private set; }
    public float WindAngle { get; private set; }
    private Text _nameText;
    public override void Result()
    {
        if (GameManager == null)
            Init();

        if (!HasWind)
        {
            Wind = FindObjectOfType<WindGenerator>();
            if (Wind == null)
                Debug.LogError("Competition Manager cannot acces Wind Generator component!");
            HasWind = true;
            if (Wind != null)
            {
                WindStrenght = Wind.windStrenght;
                WindAngle = Wind.windAngle;
            }
        }

        switch (Competition)
        {
            case CompetitionStatus.round:
                if (_currentPlayerIndex <= 1)
                {
                    ShowScoreTable();
                    Competition = CompetitionStatus.scoreTable;
                }
                else
                {
                    _currentPlayerIndex--;
                    SceneManager.LoadScene(GameManager.CurrentHill.HillName.ToString());
                }
                    break;
            case CompetitionStatus.scoreTable:
                RoundsLeft--;
                Competition = CompetitionStatus.round;
                _currentPlayerIndex = CompetitionJumpers.Count;
                HasWind = false;
                EndRound();
                break;
            default:
                break;
        }
    }

    public override void JumpFinish(float distance, float note)
    {
        if (GameManager == null)
            Init();
        CompetitionJumpers[_currentPlayerIndex-1].AddScores(distance, note);
    }

    public override void Init()
    {
        GameManager = GetComponentInParent<GameManager>();
        if (GameManager == null)
            Debug.LogError("Competition Manager cannot acces Game Manager component!");
        if (GameManager != null) CompetitionJumpers = GameManager.HotSeatPlayers.Cast<Jumper>().ToList();
        _currentPlayerIndex = CompetitionJumpers.Count;
    }

    public void ShowName()
    {
        if (GameManager == null)
            Init();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("Hot Seat Competition cannot acces Canvas object!");
        if (canvas != null)
        {
            Transform nameTransform = canvas.transform.FindChild("PlayerName");
            _nameText = nameTransform.GetComponent<Text>();
        }
        _nameText.text = CompetitionJumpers[_currentPlayerIndex - 1].Name;
    }

    public void HideName()
    {
        _nameText.text = "";
    }
}

