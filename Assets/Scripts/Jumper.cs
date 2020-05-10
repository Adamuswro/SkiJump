using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper
{
    public string Name { get; protected set; }
    public float Points { get; private set; }
    public List<float> Distance { get; private set; }
    public Clothes Cloth { get; protected set; }

    public Jumper (string name)
    {
        Name = name;
        Cloth = new Clothes();
        Distance = new List<float>();
        Points = 0;
    }

    public void AddScores(float distance, float note)
    {
        if (Distance.Count < 3)
        {
            distance = (Mathf.Round((distance * 10))) / 10;
            Distance.Add(distance);
        }
        else
        {
            throw new System.ArgumentException(Name + " can't jump 3 times.", "Distance");
        }

        note = (Mathf.Round((note * 10))) / 10;
        Points += note;
    }

    public void ClearScores()
    {
        Distance.Clear();
        Points = 0;
    }


}
