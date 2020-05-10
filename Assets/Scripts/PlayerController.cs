using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerController : MonoBehaviour {
	public float JumpForce, RotationSensitivity, LiftFactor, JumpAngle, WindFactor;
	public Animation Jump;
    public Text Score;
    public GameObject HillEnd, Wind, HighScore;
    public GameObject BodyFront, BodyBack;
    public GameObject SkiBack, Arm;
    public GameObject GoodWindPrefab;
    public GameObject ShadowPrefab;
    public GameObject ShadowPrefab2;
    public GameObject StyleNotesText;
    public GameObject DistancePoint;
    public GameObject GeneralNoteText;
    public float Drag;

    private GameObject _currentShadowPrefab;
    private HighScoreLine _highScoreLine;
    private WindGenerator _wind;
    private CompetitionManager _competitionManager;
    private Quaternion _bestAirResistance;
    private bool _isOnHill;              //Czy znajdujemy się na skoczni? 
    private float _animProgress=0;
    private int _styleTimesCalculate;
    private Rigidbody2D _rb;
    private float _rotationZ;
    private float _starGyroX;
    private float _pointsFlyingStyle;    //Punkty za styl w locie
    private const float MaxAngle = 45f;
    private float _lastPrefab;
    private float _clickTime;
    public playerState State;
    private HotSeatCompetition _competition;

    void Start () {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _wind = Wind.GetComponent<WindGenerator>();
        _bestAirResistance = Quaternion.Euler(new Vector3(0, 0, -21));
        _highScoreLine = HighScore.GetComponent<HighScoreLine>();
        _lastPrefab = 0;
        State = playerState.Wait;
        if (_competitionManager == null)
        {
            _competitionManager = FindObjectOfType<CompetitionManager>();
            if (_competitionManager == null)
                Debug.LogError("There is no _competition manager instatieted!");
            _competition = _competitionManager as HotSeatCompetition;
            if (_competition != null)
                _competition.ShowName();
        }
    }

	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState();
        }
    }

    private float TimeFromLastClick()
    {
        return Time.timeSinceLevelLoad - _clickTime;
    }

    private void FixedUpdate()
    {
        if (State == playerState.Fly)
            Rotation();
    }

    private void ChangeState()
    {
        switch (State)
        {
            case playerState.Wait:
                if (Time.timeSinceLevelLoad < 0.5f)
                    break;
                State = playerState.Ride;
                //TODO: Przenieść do managera animacji
                if (_competition != null)
                    _competition.HideName();
                StartCoroutine(MakeStart());
                break;

            case playerState.Ride:
                if (Jump.isPlaying)
                    break;
                StartCoroutine(MakeJump());
                _starGyroX = Input.acceleration.x;
                break;

            case playerState.Fly:
                StopAllCoroutines();
                StartCoroutine(MakeLanding());
            break;

            case playerState.LandingTwoLegs:
                    StartCoroutine(MakeTelemark());
                break;

            case playerState.Landing:
                break;

            case playerState.DuringTwoLegs:
                    StopAllCoroutines();
                    StartCoroutine(MakeTelemark());
                break;

            case playerState.Telemark:
                break;

            case playerState.Finish:
                if (TimeFromLastClick() >= 0.2f)
                    _competitionManager.Result();
                break;

            case playerState.Fall:
                if (TimeFromLastClick()>=0.2f)
                    _competitionManager.Result();
                break;

            default:
                break;
        }
        _clickTime = Time.timeSinceLevelLoad;
    }

    private void FallDown()
    {
        BodyBack.SetActive(true);
        AnimationState animState;
        StopAllCoroutines();
        State = playerState.Fall;
        //TODO: Przenieść do managera animacji
        if (Random.Range(0f, 4f) > 1)
        {
            Jump.CrossFade("Fall");
            animState = Jump.PlayQueued("AfterFall");
        }
        else
        {
            Jump.CrossFade("Fall2");
            animState = Jump.PlayQueued("AfterFall2");
        }
        _rb.gravityScale = 2f;
        if (_currentShadowPrefab != null)
        {
            Destroy(_currentShadowPrefab);
            _currentShadowPrefab = Instantiate(ShadowPrefab2);
        }
        if (!_isOnHill)
            CalculateNotes(GetDistance());
        else
            CalculateNotes(GetDistance(true));
        StartCoroutine(AnimationCooling(animState));
    }


    private IEnumerator MakeStart()
    {
        _rb.gravityScale = 1;
        Jump.Play("Start");
        while (Jump.isPlaying)
            yield return null;
        _rb.AddForce(new Vector2(65f, 0));
    }

    private IEnumerator AnimationCooling(AnimationState animState)          //TODO: Trzeba odwołać się do _rb PLayera w Animator Controllerze
    {
        while (_rb.velocity.magnitude > 0.1)
        {
            animState.speed = _rb.velocity.magnitude / 15f;
            yield return null;
        }
    }
    
    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private IEnumerator MakeJump()
    {
        float extraForce = 60;      //bonus za idealne wybicie z progu
        _rb.drag =Drag;
        _rb.gravityScale = 1f;
        Jump.Play("Jump");
        while (Jump.isPlaying)
            yield return null;
        if (_isOnHill == true)
        {
            Vector2 player = DistancePoint.transform.position;
            Vector2 hilEnd = HillEnd.transform.position;
            float distanceToEnd = Vector2.Distance(player, hilEnd);
            float maxDistanceToEnd = 1f;
            
            extraForce = (1 - distanceToEnd / maxDistanceToEnd) * extraForce;
            if (extraForce < 0)
                extraForce = 0;
            _rb.AddForce(GenerateVector(JumpAngle, JumpForce + extraForce));
            
            StartCoroutine(ReflexText(extraForce));
        }   
        Jump.Play("JumpToFly");
        while (Jump.isPlaying)
            yield return null;
        _rb.angularVelocity = 0;                 //Póki co obrót w locie wykonuję bezpośrednio za pomocą zmiany kąta, więc prędkość musi być 0
        StartCoroutine(FlyingAnimation());
        State = playerState.Fly;
    }

    private IEnumerator ReflexText(float extraForce)
    {
        Color startColor = Score.color;
        if (extraForce < 15)
        {
            Score.text = "Could be better";
            Score.color = Color.black;
        }
        else if (extraForce < 50)
        {
            Score.text = "Good reflex!";
            Score.color = Color.blue;
        }
        else if (extraForce > 50)
        {
            Score.text = "GREAT REFLEX!!!";
            Score.color = Color.red;
        }
        yield return new WaitForSeconds(1);
        while (Score.color.a > 6f)
        {
            Score.color = new Color(Score.color.r, Score.color.g, Score.color.b, Score.color.a - 2);
            yield return null;
        }
        Score.text = null;
        Score.color = startColor;
    }

    private void Rotation()
    {
            bool couldRotate = true;
            _rotationZ = transform.localEulerAngles.z;
            //Zabepieczenie przed maksymalnym wychyleniem
            if (_rotationZ > MaxAngle & _rotationZ <= 180)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, MaxAngle);
                couldRotate = false;
            }
            else if (_rotationZ > 180 & _rotationZ < 360 - MaxAngle)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 360 - MaxAngle);
                couldRotate = false;
            }

            Vector3 playerRotation = Vector3.forward * (-(Input.acceleration.x - _starGyroX)) * RotationSensitivity;         //obrót generowany przez gracza
            Vector3 windRotation = Vector3.forward * _wind.windStrenght * 0.1f;         //obrót generowany przez wiatr

            transform.Rotate(playerRotation);
            transform.Rotate(windRotation);

            if (couldRotate)
            {
                BodyFront.transform.Rotate(playerRotation);
                BodyFront.transform.Rotate(windRotation);
            }

            _rb.AddForce(CalculateLiftForce());
    }

    private void Style(float liftCoefficient)
    {
        _styleTimesCalculate++;
        _pointsFlyingStyle += liftCoefficient;
    }

    private IEnumerator FlyingAnimation(float inTime = 1.5f)
    {
        var skiBackFrom = SkiBack.transform.rotation;
        var armFrom = Arm.transform.rotation;
        var skiBackTo = Quaternion.Euler(SkiBack.transform.eulerAngles + new Vector3(0, 0, 12f));
        var armTo = Quaternion.Euler(Arm.transform.eulerAngles + new Vector3(0, 0, 8));
        while(Jump.isPlaying)
            yield return null;
        while (State == playerState.Fly)
        {
            var t=0f;
            for (t=0f; t < 1; t += Time.deltaTime / inTime)
            {
                SkiBack.transform.rotation = Quaternion.Lerp(skiBackFrom, skiBackTo, t);
                Arm.transform.rotation = Quaternion.Lerp(armFrom, armTo, t);
                yield return null;
            }
            t = 0f;
            for (t = 0f; t < 1; t += Time.deltaTime / inTime)
            {
                SkiBack.transform.rotation = Quaternion.Lerp(skiBackTo, skiBackFrom, t);
                Arm.transform.rotation = Quaternion.Lerp(armTo, armFrom, t);
                yield return null;
            }
        }
    }
       
    private Vector2 CalculateLiftForce()
    {
        var angleOfAttack = Quaternion.Angle(_bestAirResistance, transform.rotation);
        var liftCoefficient = 1f - angleOfAttack / (MaxAngle);
        liftCoefficient = Mathf.Clamp(liftCoefficient, 0.3f, 1f);
        Style(liftCoefficient);
        #region air resistance prefab
        if (liftCoefficient > 0.95f)
        {
            if ((Time.time - _lastPrefab) > 0.4f)
            {
                _lastPrefab = Time.time;
                Instantiate(GoodWindPrefab, new Vector2(transform.position.x + 0.15f, transform.position.y + 0.55f), transform.rotation);
            }
        }
        #endregion
        liftCoefficient *= LiftFactor;                                  //Współczynnik oporu powietrza pomnożony przez współczynnik LiftFactor

        Vector2 windVector = _wind.wind;         //Prędkość wiatru, pomnożona przez współczynnik WindFactor
        Vector2 speedVector = _rb.velocity;      //Prędkość skoczka

        var resulantVector = windVector - speedVector;
        var trueAirSpeed = resulantVector.magnitude *WindFactor;
        trueAirSpeed *= trueAirSpeed;                   //Kwadrat szybkości, ponieważ tak jest we wzorze na nośność 
        return (Quaternion.AngleAxis(90, Vector3.forward) * _rb.velocity.normalized* (liftCoefficient + trueAirSpeed)) ;        //Siła nośna - skierowana zawsze pod kątem 90 stopni do prękości
    }

    private IEnumerator MakeLanding()
    {
        //TODO: POłączyć z telemarkiem, wybór lądowania uzależniony od miejsca kliknięcia
        BodyBack.SetActive(true);
            State = playerState.DuringTwoLegs;
            Jump.Play("Landing");
        const int rotatingTime = 7; //Czas obrotu w klatkach
        Vector3 rotateValue;
        Quaternion finalAngle = Quaternion.Euler(new Vector3(0, 0, 335));
        if (transform.eulerAngles.z > 180 && transform.eulerAngles.z < finalAngle.eulerAngles.z)
            rotateValue = new Vector3 (0,0,(Quaternion.Angle(finalAngle, transform.rotation))/ rotatingTime);
        else
            rotateValue = new Vector3 (0,0,-(Quaternion.Angle(finalAngle, transform.rotation)) / rotatingTime);
        rotateValue.z = Mathf.Clamp(rotateValue.z, -6, 6);

        while (Jump.isPlaying)
        {
            if(Quaternion.Angle(finalAngle, transform.rotation)>(rotateValue.z+1))
                transform.Rotate(rotateValue);
            _animProgress += 1;
            yield return null;
        }
            State = playerState.LandingTwoLegs;
    }

    private IEnumerator MakeTelemark()
    {
        _animProgress = 0;
        State = playerState.DuringTelemark;
        Jump.Play("Telemark");
        while (Jump.isPlaying)
        {
            _animProgress += 1;
            yield return null;
        }
        State = playerState.Telemark;
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.tag == "Hill")
            _isOnHill = true;
        if (collider.tag == "ShadowInstantiate")
        {
            _currentShadowPrefab = Instantiate(ShadowPrefab);
            Destroy(collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Hill")
        {
            _isOnHill = false;   
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (State != playerState.Finish && State != playerState.Fall)
        {
            if (collision.collider.tag == "HillSide")
            {
                switch (State)
                {
                    case playerState.Ride:
                        FallDown();
                        break;

                    case playerState.Fly:
                        FallDown();
                        break;

                    case playerState.DuringTwoLegs:
                        if (_animProgress > 4)
                            Land();
                        else
                            FallDown();
                        break;

                    case playerState.LandingTwoLegs:
                        Land();
                        break;

                    case playerState.DuringTelemark:
                        if (_animProgress > 3)
                            Land();
                        else
                            FallDown();
                        break;

                    case playerState.Telemark:
                        Land();
                        break;

                }

            }
            else if (collision.collider.tag == "Hill")
                switch (State)
                {
                    case playerState.Ride:
                        break;

                    case playerState.Wait:
                        break;

                    default:
                        FallDown();
                        break;
                }
        }
    }

    private void Land()
    {
        StopAllCoroutines();
        switch(State)
        {
            case playerState.LandingTwoLegs:
                Jump.Play("LandAfterTwoLegs");
            break;

            case playerState.Telemark:
                Jump.Play("LandAfterTelemark");
            break;

            case playerState.DuringTwoLegs:
                Jump.Play("LandDuringTwoLegs");
            break;

            case playerState.DuringTelemark:
                Jump.Play("LandDuringTelemark");
            break;

            default:
            break;
        }
        if (_currentShadowPrefab!=null)
        {
            Destroy(_currentShadowPrefab);
            _currentShadowPrefab = Instantiate(ShadowPrefab2);
        }
        CalculateNotes(GetDistance());
        State = playerState.Finish;
        _rb.gravityScale = 2f;
    }

    public static float DistanceNote(float distance, Hill hill)
    {
        float distanceNote;
        switch (hill.HillType)  //Punkty za odległość - uzależnione od typu skoczni
        {
            case hillType.small:         
                distanceNote = 60 + (distance - hill.KPoint) * 2.2f;
                break;

            case hillType.medium:         
                distanceNote = 60 + (distance - hill.KPoint) * 2f;
                break;

            case hillType.large:      
                distanceNote = 120 + (distance - hill.KPoint) * 1.8f;
                break;

            case hillType.mamut:        
                distanceNote = 120 + (distance - hill.KPoint) * 1.2f;
                break;

            default:
                distanceNote = 0;
                break;
        }
        return distanceNote;
    }

    public float StyleNotes (playerState state)
    {
        float landingNote;                 //Składowa noty za styl, na podstawie lądowania
        float flyingNote;            //Składowa noty za styl, na podstawie zachowania w locie
        float[] judgeNotes = new float[5];    //Noty sędziów: dodane noty za lądowanie i styl w locie + drobna randomizacja

        //Obliczenie noty za lądowanie
        switch (state)
        {
            case playerState.Fall:
                landingNote = 7;
                break;

            case playerState.DuringTwoLegs:
                landingNote = 12.5f;
                break;

            case playerState.DuringTelemark:
                landingNote = 13.5f;
                break;

            case playerState.LandingTwoLegs:
                landingNote = 15.5f;
                break;

            case playerState.Telemark:
                landingNote = 17;
                break;

            default:
                landingNote = 0;
                break;
        }
            float flyingStyle = (_pointsFlyingStyle) / (_styleTimesCalculate);
            if (flyingStyle > 0.98f)
                flyingNote = 3;
            else if (flyingStyle > 0.96f)
                flyingNote = 2.5f;
            else if (flyingStyle > 0.94f)
                flyingNote = 2f;
            else if (flyingStyle > 0.92f)
                flyingNote = 1.5f;
            else if (flyingStyle > 0.91f)
                flyingNote = 1f;
            else if (flyingStyle > 0.8f)
                flyingNote = 0.5f;
            else
                flyingNote = 0;

        //Randomizacja not sędziów, 1 na 100 not mocniej losowa
        for (int i = 0; i < 5; i++)
        {
            if ((int)Random.Range(0, 100) == 13)
            {
                judgeNotes[i] = landingNote + flyingNote + (float)(Random.Range(-5, 6)) / 2;
                judgeNotes[i] = Mathf.Clamp(judgeNotes[i], 0, 20);
            }
            else
            {
                judgeNotes[i] = landingNote + flyingNote + ((float)Random.Range(-1, 2)) / 2;
                judgeNotes[i] = Mathf.Clamp(judgeNotes[i], 0, 20);
            }
        }

        //Sortowanie - odrzucamy najmniejszą i największa wartość
        int downScoreIndex = 0, highScoreIndex = 0, j = 0;
        while (j < 5)
        {
            if (judgeNotes[downScoreIndex] > judgeNotes[j])
                downScoreIndex = j;
            else if (judgeNotes[highScoreIndex] < judgeNotes[j])
                highScoreIndex = j;
            j++;
        }
        if (downScoreIndex == highScoreIndex)
            highScoreIndex = 1;

        //Nota całkowita za styl, odrzucamy 2 skrajne noty sędziów
        float styleNote= judgeNotes.Sum();                    //Ostateczna nota za styl, po odrzuceniu 2 skrajnych not sędziów
        styleNote = styleNote - (judgeNotes[downScoreIndex] + judgeNotes[highScoreIndex]);

        ShowJudgeNotes(judgeNotes, downScoreIndex, highScoreIndex);

        return styleNote;
    }

    private void CalculateNotes(float distance)
    {
        float totalNote = DistanceNote(distance, GameManager.CurrentHill) + StyleNotes(State);

        _competitionManager.JumpFinish(distance, totalNote);
        ShowTotalNote(totalNote);
    }

    private void ShowJudgeNotes(float[] judgeNotes, int downScoreIndex, int highScoreIndex)
    {
        Text[] notes = StyleNotesText.GetComponentsInChildren<Text>();
        
        for(int i=0; i<5; i++)
        {
            notes[i].text = judgeNotes[i].ToString();
            if (i == downScoreIndex || i == highScoreIndex)
                notes[i].color = Color.black;
        }
    }

    private void ShowTotalNote (float totalNote)
    {
        Text generalNote = GeneralNoteText.GetComponent<Text>();
        generalNote.text = totalNote.ToString("F1");
    }

    private float GetDistance(bool isOnHill=false) 
        //Przelicza i wyświetla odległość uzyskaną przez gracza, wywołuje metodę nowego rekordu jeśli takowy istnieje
        //TODO: zintegrować z klasą zarządzającą skoczniami
    {
        float distanceFactor = 2;           //przeliczenie na realny wynik
        float distance;
        if (isOnHill)
            distance = 0;
        else
            distance = Mathf.Round(distanceFactor * Vector3.Distance(HillEnd.transform.position, DistancePoint.transform.position) * 10f) / 10f;
        distance = Mathf.Round(distance * 2) / 2;
        Score.text = distance + "m";
        if (distance > PlayerPrefs.GetFloat("HighScore") && State != playerState.Fall)
        {
            _highScoreLine.SetHighScore(this.transform.position.x, this.transform.position.y, distance);
            Jump.PlayQueued("AfterRecord"); ;
        }
        
        return distance;
    }

    static Vector2 GenerateVector(float vectorAngle, float vectorLenght)
        //Generuje wektor o okreslonym kącie (wg ćwiartek układu) i długości
    {
        float x, y;
        x = Mathf.Cos(vectorAngle*Mathf.PI/180) * vectorLenght;
        y = Mathf.Sin(vectorAngle * Mathf.PI/180) * vectorLenght;

        return new Vector2(x, y);
    }
}

