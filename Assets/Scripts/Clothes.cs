using UnityEngine;

public class Clothes
{
    private Color _head;
    private Color _body;
    private Color _upperArm;
    private Color _lowerArm;
    private Color _hand;
    private Color _upperLeg;
    private Color _lowerLeg;
    private Color _foot;
    private Color _ski;

    public Clothes()
    {
        RandomClothes();
    }

    public void RandomClothes ()
    {
        _head = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _body = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _upperArm = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _lowerArm = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _hand = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _upperLeg = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _lowerLeg = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _foot = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        _ski = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    public void SetColor(jumperBody bodyPart, Color newColor)
    {
        switch(bodyPart)
        {
            case jumperBody.LowerArm:
                _lowerArm = newColor;
            break;
            case jumperBody.Head:
                _head = newColor;
                break;
            case jumperBody.Body:
                _body = newColor;
                break;
            case jumperBody.UpperArm:
                _upperArm = newColor;
                break;
            case jumperBody.Hand:
                _hand = newColor;
                break;
            case jumperBody.UpperLeg:
                _upperLeg = newColor;
                break;
            case jumperBody.LowerLeg:
                _lowerLeg = newColor;
                break;
            case jumperBody.Foot:
                _foot = newColor;
                break;
            case jumperBody.Ski:
                _ski = newColor;
                break;
            default:
                break;
        }
    }

    public Color GetColor(jumperBody bodyPart)
    {
        switch (bodyPart)
        {
            case jumperBody.LowerArm:
                return _lowerArm;
            case jumperBody.Head:
                return _head;
            case jumperBody.Body:
                return _body;
            case jumperBody.UpperArm:
                return _upperArm;
            case jumperBody.Hand:
                return _hand;
            case jumperBody.UpperLeg:
                return _upperLeg;
            case jumperBody.LowerLeg:
                return _lowerLeg;
            case jumperBody.Foot:
                return _foot;
            case jumperBody.Ski:
                return _ski;
            default:
                return Color.black;
        }
    }
}