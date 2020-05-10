using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Enemy> TrainingJumpers { get; private set; }
    public Player Player { get; private set; }
    public List<Enemy> CareerJumpers { get; private set; }
    public GameObject TrainingCompetitionPrefab;
    public GameObject HotSeatCompetitionPrefab;
    public bool IsCompetition;
    public List<Hill> Hills { get; private set; }
    public static Hill CurrentHill { get; private set; }
    public CompetitionManager ActualCompetition { get; private set; }
    public List<Player> HotSeatPlayers { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        HillsConstructor();
        //TODO: Należy zaadoptować klasę Serializer do zapisywania i wczytywania listy skoczków
        TrainingJumpers = new List<Enemy>
        {
            new Enemy("Adam Małysz", Skill.good, Skill.veryGood),
            new Enemy("Jane Ahonen", Skill.average, Skill.average),
            new Enemy("Noriaki Kasai", Skill.good, Skill.weak),
            new Enemy("Jakub Janda", Skill.good, Skill.veryWeak),
            new Enemy("Kamil Stoch", Skill.good, Skill.veryGood)
        };
        Player = new Player("Player");
        HotSeatPlayers = new List<Player> {new Player("Adam"), new Player("Siwy")};
    }

    private void HillsConstructor()
    {
        Hills = new List<Hill> {new Hill(hills.Prototype, 100)};
    }

    private void Start()
    {
        PlayerModify();
    }

    private void SaveJumpers()
    {

    }

    private void LoadJumpers() 
    {

    }

    public void StartTraining(hills hill)
    {
        if (ActualCompetition!=null)
        {
            Debug.Log("Competition is already started!");
            return;
        }
        ActualCompetition = Instantiate(TrainingCompetitionPrefab).GetComponent<CompetitionManager>();
        if (ActualCompetition == null)
            Debug.LogError("GameManager cannot acces Competition Manager in Training Competition Prefab!");
        if (ActualCompetition != null) ActualCompetition.transform.parent = this.transform;
        CurrentHill = Hills.Find(_hill => _hill.HillName.Equals(hill));
        SceneManager.LoadScene(CurrentHill.HillName.ToString());
    }

    public void StartHotSeat(hills hill)
    {
        if (ActualCompetition != null)
        {
            Debug.Log("Competition is already started!");
            return;
        }
        ActualCompetition = Instantiate(HotSeatCompetitionPrefab).GetComponent<CompetitionManager>();
        if (ActualCompetition == null)
            Debug.LogError("GameManager cannot acces Competition manager in Hot Seat Competition Prefab!");
        if (ActualCompetition != null) ActualCompetition.transform.parent = this.transform;
        CurrentHill = Hills.Find(_hill => _hill.HillName.Equals(hill));
        SceneManager.LoadScene(CurrentHill.HillName.ToString());
    }

    public void PlayerModify()
    {
        SceneManager.LoadScene("PlayerModify");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void EndCompetition()
    {
        SceneManager.LoadScene("Menu");
        CurrentHill = null;
        IsCompetition = false;
        Destroy(ActualCompetition.gameObject);
    }


}