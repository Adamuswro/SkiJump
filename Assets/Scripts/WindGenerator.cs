using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindGenerator : MonoBehaviour {

    //values
    public float maxWindStrenght =3f;
    public GameObject arrowImage;
    public Text windForceStrenght;
    public bool staticWindGeneration = false;
    bool windGenerated = false;
    //wspolrzedne wektora
    float x; 
    float y;
    public float windAngle;
    public float windStrenght;
    public Vector2 wind;
    public float softening;
    List<float> angleList;
    float[] angleArray;
    List<float> strenghtList;
    float[] strenghtArray;
    // wartosc wiatru wygenerowana przy wywołaniu konstruktora
    float windRef_Angle;
    float windRef_Strenght;

    //constructor
    public WindGenerator(){
        windGenerated = false;

    }

    //methods

    public void GenerateWind(){
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("Wind generator cannot acces Game Manager component!");
        HotSeatCompetition competition = gameManager.ActualCompetition as HotSeatCompetition;
        if (competition != null && competition.HasWind)
        {
            RandomAngle(competition.WindAngle);
            RandomStrenght(competition.WindStrenght);
        }
        else
        {
            RandomAngle();
            RandomStrenght();
        }

        VectorPoint();

        wind = new Vector2(x, y);
        windGenerated = true;
        return;
    }
        

    public void GenerateDynamicWind()
    {
        if (!windGenerated)
        {
            GenerateWind();
        }

       // Randomize_x();
        //Randomize_y();

        Randomize_windAngle();
        Randomize_windStrength();
        VectorPoint();

        wind = new Vector2(x, y);
  
    } 
    public void GenerateStaticWind()
    {
        VectorPoint();
        wind = new Vector2(x, y);

    }
     
     public float RandomAngle(float angle=0)
    {
        if (angle == 0)
            windAngle = Random.Range(90, 360);
        else
            windAngle = angle;
        windRef_Angle = windAngle;
        angleList = new List<float>();
        angleList.Add(windAngle);
        return windAngle;
    }

    public float RandomStrenght(float strenght =0){
        if (strenght == 0)
            windStrenght = Random.Range(0.0f, 3.0f);
        else
            windStrenght = strenght;
        windRef_Strenght = windStrenght;
        strenghtList = new List<float>();
        strenghtList.Add(windStrenght);
        return windStrenght;
    }

    public void VectorPoint()
    {

        x = Mathf.Cos(windAngle*Mathf.PI/180) * windStrenght;
        y = Mathf.Sin(windAngle * Mathf.PI/180) * windStrenght;

       // windRef_X = x;
        //windRef_Y = y;
    }

      public float Randomize_windAngle ()
    {

        //zakres w ktorym wiatr bedzie sie zmienial
        float windAngle1 = windAngle - 1;
        float windAngle2 = windAngle + 1;
        float randomized_windAngle = Random.Range(windAngle1, windAngle2);

        angleList.Add(randomized_windAngle);
        angleArray = angleList.ToArray(); //to jest po to zeby trzymac 2 kolejne wartosci kata do lerp funkcji

        windAngle = Mathf.Lerp(angleArray[0], angleArray[1], Time.deltaTime* softening);
            
        windAngle = Mathf.Clamp(windAngle, windRef_Angle - 20, windRef_Angle + 20);
        if (windAngle < 90)
        {
            windAngle = 90;
        }
        if (windAngle > 360)
        {
            windAngle = 360;
        }
        angleList.RemoveAt(0); //po przepisaniu to dablicy usuwam element zeby lista nie rosla za duza
        return windAngle;   
    } 
     public float Randomize_windStrength ()
    {
        
        //zakres w ktorym wiatr bedzie sie zmienial
        float windStrenght1 = windStrenght - 0.1f;
        float windStrenght2 = windStrenght + 0.1f;
        float randomized_Strenght = Random.Range(windStrenght1, windStrenght2);

        strenghtList.Add(randomized_Strenght);
        strenghtArray= strenghtList.ToArray();

        windStrenght = Mathf.Lerp(strenghtArray[0], strenghtArray[1],Time.deltaTime* softening);

        windStrenght = Mathf.Clamp(windStrenght, windRef_Strenght - 0.5f, windRef_Strenght + 0.5f);
        strenghtList.RemoveAt(0);

        if (windStrenght < 0)
        {
            windStrenght = 0;
        }

        return windStrenght;   
    } 


    public void RefreshImageAndText()
    {
        float windStrenghtDecimal;

        windStrenghtDecimal = Mathf.Round(windStrenght * 10) / 10;
        arrowImage.transform.eulerAngles= new Vector3(0, 0, windAngle-90); //Arrow rotation
        windForceStrenght.text = windStrenghtDecimal.ToString();
    }
	
	// Update is called once per frame
    void LateUpdate(){
        RefreshImageAndText();
    }

    void FixedUpdate()
    {
        if (!staticWindGeneration)
        {
            GenerateDynamicWind();

        }else if(staticWindGeneration)
        {
            GenerateStaticWind();
        }
    }

    public float windFactor()
    /// <summary>
    /// Funkcja określająca jak dobry jest wiatr - używana w klasie skoczka do generowania wyników przeciwników
    /// </summary>
    {
        float windFactor;
        float bestAngle = 155;
        float worstAngle = bestAngle + 180;
        float neutralAngle = (worstAngle - bestAngle) / 2 + bestAngle;

        if (windAngle <= neutralAngle)
        {
            windFactor = 1 - Mathf.Abs(bestAngle - windAngle) / (neutralAngle - bestAngle);
        }
        else
        {
            windFactor = -1 + Mathf.Abs(worstAngle - windAngle) / (worstAngle - neutralAngle);
        }

        windFactor *= windStrenght;

        return windFactor;
    }
}
