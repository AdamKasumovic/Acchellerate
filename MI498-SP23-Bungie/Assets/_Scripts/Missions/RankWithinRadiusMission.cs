using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankWithinRadiusMission : SingleMission
{
    public GradeLetter CurrentGradeLetter { get; private set; }
    GameObject tempSphere;
    CarManager carManager;

    [Header("Rank Within Radius Mission Settings")]
    public GradeLetter missionGradeLetter = GradeLetter.A;
    public float radius = 10;
    public GameObject sphereRadius;

    protected override void Start()
    {
        base.Start();
        carManager = CarManager.Instance;
        CurrentGradeLetter = (GradeLetter)GameManager.instance.data.GetGradeFromPoints((int)CarManager.numPoints);
    }

    protected override void Update()
    {
        base.Update();
        if(IsActive && tempSphere == null)
        {
            tempSphere = Instantiate<GameObject>(sphereRadius, carManager.transform.position, Quaternion.identity, this.transform);
            tempSphere.transform.localScale = new Vector3(radius, radius, radius);
        }

        
        string tintString = (!IsCompleted && !IsFailed && !IsActive) ? " tint=1" : "";
        string timer = UseTimer ? $"<sprite index=0{tintString}>{Mathf.Max(0, timeRemaining):0.0}s" : "";
        string missionSymbol = $"<sprite index=16>";
        string grade = GetGradeLetter(missionGradeLetter);








        MissionName = $"{SpriteInsideBoxMarkdown} {missionSymbol} Reach {grade} rank inside the zone {timer}";

        
    }

    private string GetGradeLetter(GradeLetter type)
    {
        switch (type)
        {
            case GradeLetter.A:
                return "A";
            case GradeLetter.B:
                return "B";
            case GradeLetter.C:
                return "C";
            case GradeLetter.D:
                return "D";
            case GradeLetter.Z:
                return "Z";
            default:
                return "default";
        }
    }
    public void NewGreadLetter(int greadID)
    {
        CurrentGradeLetter = (GradeLetter)greadID;
    }
    public override void Execute()
    {
        base.Execute();
        if (CurrentGradeLetter >= missionGradeLetter)
        {
            CompleteMission();
        }
    }

    public override void FailMission()
    {
        base.FailMission();  // MUST CALL THIS!
        Destroy(tempSphere);
        tempSphere = null;
        Debug.Log("Rank Within Radius Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Destroy(tempSphere);
        tempSphere = null;
        Debug.Log("Rank Within Radius Mission Completed!");
    }
}