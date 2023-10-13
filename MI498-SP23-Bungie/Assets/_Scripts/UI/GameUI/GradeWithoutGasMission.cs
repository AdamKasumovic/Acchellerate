using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GradeLetter
{
    D,
    C,
    B,
    A, 
    Z,
}

public class GradeWithoutGasMission : SingleMission
{
    public GradeLetter CurrentGradeLetter { get; private set; }

    [Header("Grade Without Gas Mission Settings")]
    public GradeLetter missionGradeLetter = GradeLetter.A;

    protected override void Start()
    {
        base.Start();
        CurrentGradeLetter = (GradeLetter)GameManager.instance.data.GetGradeFromPoints((int)CarManager.numPoints);
    }

    protected override void Update()
    {
        base.Update();

        string grade = GetGradeLetter(missionGradeLetter);
        string currentGrade = GetGradeLetter(CurrentGradeLetter);
        string progress = $"Reach {grade} rank without collecting any gas cans";
        string timer = UseTimer ? $" Time left: {Mathf.Max(0, timeRemaining):0.0}s" : "";

        MissionName = $"{progress}.{timer}";
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
        Debug.Log("Grade Without Gas Mission Failed!");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();  // MUST CALL THIS!
        Debug.Log("Grade Without Gas Mission Completed!");
    }
}
