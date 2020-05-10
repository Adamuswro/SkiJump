using UnityEngine;

public class Enemy:Jumper
    {
    public Skill StyleSkill { get; protected set; }
    public Skill JumpSkill { get; protected set; }

    public Enemy(string name, Skill styleSkill, Skill jumpSkill):base(name)
    {
        StyleSkill = styleSkill;
        JumpSkill = jumpSkill;
    }

    public void MakeJump(int kPoint, WindGenerator wind)
    {
        float randomizationRange = kPoint * 0.05f;        //+- of the distance
        float distanceFactor = 0;                        //Dependes of the JumpSkill property 
        float windImpact = wind.windFactor() * kPoint * 0.06f;   //The Wind impact

        switch (JumpSkill)
        {
            case Skill.veryWeak:
                distanceFactor += 0.975f;
                break;
            case Skill.weak:
                distanceFactor += 0.995f;
                break;
            case Skill.average:
                distanceFactor += 1.015f;
                break;
            case Skill.good:
                distanceFactor += 1.035f;
                break;
            case Skill.veryGood:
                distanceFactor += 1.065f;
                break;
            default:
                Debug.Log("Jump Skill of the " + Name + " is out of range!");
                break;
        }
        float distance = kPoint * distanceFactor + windImpact;

        int randomization = (int)Random.Range(0, 50);
        if (randomization == 13)
            distance = (Random.Range(distance - 3 * randomizationRange, distance + randomizationRange));
        else if (randomization == 12)
            distance = (Random.Range(distance - randomizationRange, distance + 2 * randomizationRange));
        else
            distance = (Random.Range(distance - randomizationRange, distance + randomizationRange));

        distance = Mathf.Round(distance * 2) / 2;
        AddScores(distance, CalculatePoints(distance));
    }
    
    private float CalculatePoints(float distance)
    {
        float baseStyleNote;
        float[] styleNotes = new float[3];
        float totalPoints=0;

        switch (StyleSkill)
        {
            case Skill.veryWeak:
                baseStyleNote = 15.5f;
                break;
            case Skill.weak:
                baseStyleNote = 16.5f;
                break;
            case Skill.average:
                baseStyleNote = 17.5f;
                break;
            case Skill.good:
                baseStyleNote = 18.5f;
                break;
            case Skill.veryGood:
                baseStyleNote = 19.5f;
                break;
            default:
                baseStyleNote = 0;
                break;
        }

        const int randomRange = 2;
        for (int i = 0; i < 3; i++)
        {
            styleNotes[i] = baseStyleNote + 0.5f * Random.Range(-randomRange, randomRange);
            Mathf.Clamp(styleNotes[i], 0, 20);
        }

        foreach (int note in styleNotes)
            totalPoints += note;
        totalPoints += PlayerController.DistanceNote(distance, GameManager.CurrentHill);
        return totalPoints;
    }
}

