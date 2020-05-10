using UnityEngine;

public class Hill
{
    public hills HillName { get; private set; }
    public hillType HillType { get; private set; }
    public int KPoint { get; private set; }
    public float HillRecord { get; private set; }
    public Vector2 RecordPoint { get; private set; }

    public Hill(hills hillName, int kPoint)
    {
        HillName = hillName;
        KPoint = kPoint;

        if (kPoint <= 80)
            HillType = hillType.small;
        else if (kPoint < 100)
            HillType = hillType.medium;
        else if (kPoint <= 170)
            HillType = hillType.large;
        else if (kPoint > 170)
            HillType = hillType.mamut;

        HillRecord = PlayerPrefs.GetFloat(HillName.ToString());
        RecordPoint = new Vector2(PlayerPrefs.GetFloat(HillName.ToString() + "X", -400),
            PlayerPrefs.GetFloat(HillName.ToString() + "Y", -400));

    }

    public void SetRecord(float hillRecord, Vector2 recordPoint)
    {
        if (hillRecord > HillRecord)
        {
            HillRecord = hillRecord;
            RecordPoint = recordPoint;

            PlayerPrefs.SetFloat(HillName.ToString(), HillRecord);
            PlayerPrefs.SetFloat(HillName.ToString() + "X", RecordPoint.x);
            PlayerPrefs.SetFloat(HillName.ToString() + "Y", RecordPoint.y);
        }
    }

    public void ResetRecord()
    {
            HillRecord = 0;
            RecordPoint = new Vector2(-400,-400);

            PlayerPrefs.SetFloat(HillName.ToString(), 0);
            PlayerPrefs.SetFloat(HillName.ToString() + "X", -400);
            PlayerPrefs.SetFloat(HillName.ToString() + "Y", -400);
    }

}

