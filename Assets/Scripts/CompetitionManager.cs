using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompetitionManager : MonoBehaviour {
    public GameObject JumperScore;

    protected GameObject ScoreTable;
    protected GameObject GameCanvas;
    protected GameObject ScoreCanvas;
    protected GameManager GameManager;
    protected List<Enemy> Jumpers;
    protected Player Player;
    protected List<Jumper> CompetitionJumpers;
    public WindGenerator Wind  { get; protected set; }
    protected int RoundsLeft;
    protected CompetitionStatus Competition;

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        RoundsLeft = 2;
        Competition = CompetitionStatus.round;
    }
    
    public virtual void JumpFinish(float distance, float note)
    {
        if (GameManager == null)
            Init();
        Player.AddScores(distance, note);
    }

    public virtual void Result()
    {
        if (GameManager == null)
            Init();

        switch (Competition)
        {
            case CompetitionStatus.round:
                Wind = FindObjectOfType<WindGenerator>();
                if (Wind == null)
                    Debug.LogError("Competition Manager cannot acces Wind Generator component!");
                foreach (Jumper jumper in CompetitionJumpers)
                {
                    Enemy enemy = jumper as Enemy;
                    if(enemy!=null)
                        enemy.MakeJump(GameManager.CurrentHill.KPoint, Wind);
                }
                ShowScoreTable();
                Competition = CompetitionStatus.scoreTable;
                break;
            case CompetitionStatus.scoreTable:
                RoundsLeft--;
                Competition = CompetitionStatus.round;
                EndRound();
                break;
            default:
                break;
        }
        
    }

    public virtual void Init()
    {
        GameManager = GetComponentInParent<GameManager>();
        if (GameManager == null)
            Debug.LogError("Competition Manager cannot acces Game Manager component!");
        if (GameManager != null)
        {
            Jumpers = GameManager.TrainingJumpers;
            Player = GameManager.Player;
        }
        CompetitionJumpers = Jumpers.Cast<Jumper>().ToList();
        CompetitionJumpers.Add(Player);
    }

    protected void ShowScoreTable()
    {
        GameObject[] allGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject gameObject in allGameObjects)
        {
            if (gameObject.name == "Canvas")
                GameCanvas = gameObject;
            else if (gameObject.name == "ScoreBoard")
                ScoreCanvas = gameObject;
        }
        if (GameCanvas == null)
            Debug.LogError("Can't find Canvas!!!");
        if (ScoreCanvas == null)
            Debug.LogError("Can't find Score Board!!!");
        if (GameCanvas != null) GameCanvas.SetActive(false);
        if (ScoreCanvas != null)
        {
            ScoreCanvas.SetActive(true);
            ScoreTable = ScoreCanvas.transform.FindChild("ScoreTable").gameObject;
        }
        if (ScoreTable == null)
            Debug.LogError("Can't find Score Table!!!");
        FillScoreTable();
    }

    private void FillScoreTable()
    {
        CompetitionJumpers = CompetitionJumpers.OrderByDescending(c => c.Points).ToList();
        int i = 1;
        foreach (Jumper jumper in CompetitionJumpers)
        {
            GameObject jumperScore = Instantiate(JumperScore);
            jumperScore.transform.SetParent(ScoreTable.transform);
            Text place = jumperScore.transform.FindChild("Place").GetComponent<Text>();
            if (place == null)
                Debug.LogError("Place field in score table didn't find!");
            Text name = jumperScore.transform.FindChild("Name").GetComponent<Text>();
            if (name == null)
                Debug.LogError("Name field in score table didn't find!");
            Text round1 = jumperScore.transform.FindChild("1.Round").GetComponent<Text>();
            if (round1 == null)
                Debug.LogError("1.Round field in score table didn't find!");
            Text round2 = jumperScore.transform.FindChild("2.Round").GetComponent<Text>();
            if (round2 == null)
                Debug.LogError("2.Round field in score table didn't find!");
            Text score = jumperScore.transform.FindChild("Points").GetComponent<Text>();
            if (score == null)
                Debug.LogError("Score field in score table didn't find!");

            place.text = i.ToString() + ".";
            name.text = jumper.Name;
            round1.text = jumper.Distance[0].ToString();
            if (RoundsLeft == 1)
                round2.text = jumper.Distance[1].ToString();
            else
                round2.text = "0";
            score.text = jumper.Points.ToString();
            if (!(GameManager.ActualCompetition is HotSeatCompetition))
            {
                if (jumper.GetType() == Type.GetType("Player"))
                {
                    place.color = Color.green;
                    name.color = Color.green;
                    round1.color = Color.green;
                    round2.color = Color.green;
                    score.color = Color.green;
                }
            }
            i++;
        }
    }

    protected void EndRound()
    {
        if (RoundsLeft <= 0)
        {
            ClearScores();
            RestartCompetition();
        }
        else
            SceneManager.LoadScene(GameManager.CurrentHill.HillName.ToString());
    }

    public void RestartCompetition()
    {
        ClearScores();
        RoundsLeft = 2;
        Competition = CompetitionStatus.round;
        SceneManager.LoadScene(GameManager.CurrentHill.HillName.ToString());
    }

    private void ClearScores()
    {
        foreach (Jumper jumper in CompetitionJumpers)
            jumper.ClearScores();
    }
}
