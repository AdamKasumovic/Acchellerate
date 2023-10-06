using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public struct KillInfo
{
    public int frontFlipKillPoints;
    public int tiltKillPoints;
    public int groundPoundKillPoints;
    public int strafeKillPoints;
    public int driftKillPoints;
    public int drivingKillPoints;
    public int burnoutKillPoints;
    public int swoleKillPoints;

    public int frontFlipBonusPoints;
    public int tiltBonusPoints;
    public int groundPoundBonusPoints;
    public int strafeBonusPoints;
    public int driftBonusPoints;
    public int drivingBonusPoints;
    public int burnoutBonusPoints;
    public int swoleBonusPoints;

    public int frontFlipStalePoints;
    public int tiltStalePoints;
    public int groundPoundStalePoints;
    public int strafeStalePoints;
    public int driftStalePoints;
    public int drivingStalePoints;
    public int burnoutStalePoints;
    public int swoleStalePoints;

    public float frontFlipKillTime;
    public float tiltKillTime;
    public float groundPoundKillTime;
    public float strafeKillTime;
    public float driftKillTime;
    public float drivingKillTime;
    public float burnoutKillTime;
    public float swoleKillTime;

    public List<(int, int, int, float)> GetPointsAsArray()
    {
        return new List<(int, int, int, float)>{(frontFlipKillPoints, frontFlipBonusPoints, frontFlipStalePoints, frontFlipKillTime),
                                            (tiltKillPoints, tiltBonusPoints, tiltStalePoints, tiltKillTime),
                                            (groundPoundKillPoints, groundPoundBonusPoints, groundPoundStalePoints, groundPoundKillTime),
                                            (strafeKillPoints, strafeBonusPoints, strafeStalePoints, strafeKillTime),
                                            (driftKillPoints, driftBonusPoints, driftStalePoints, driftKillTime),
                                            (drivingKillPoints, drivingBonusPoints, drivingStalePoints, drivingKillTime),
                                            (burnoutKillPoints, burnoutBonusPoints, burnoutStalePoints, burnoutKillTime),
                                            (swoleKillPoints, swoleBonusPoints, swoleStalePoints, swoleKillTime)};

    }
}

public class NewStyleKey
{
    public bool frontFlipKill;
    public bool tiltKill;
    public bool groundPoundKill;
    public bool strafeKill;
    public bool driftKill;
    public bool burnoutKill;
    public bool swoleKill;
    public NewStyleKey()
    {
        frontFlipKill = CarManager.Instance.frontFlipKillIndicator;
        tiltKill = CarManager.Instance.tiltKillIndicator;
        groundPoundKill = CarManager.Instance.groundPoundKillIndicator;
        strafeKill = CarManager.Instance.horBoostKillIndicator;
        burnoutKill = CarManager.Instance.isSpinning;
        swoleKill = CarManager.Instance.swoleKill;
        if (!(frontFlipKill || tiltKill || groundPoundKill || strafeKill || burnoutKill || swoleKill) && (CarManager.currentState == CarManager.CarState.DriftingLeft || CarManager.currentState == CarManager.CarState.DriftingRight))
        {
            driftKill = true;
        }
        else
        {
            driftKill = false;
        }
    }
    public bool[] GetKeyAsArray()
    {
        return new bool[] { frontFlipKill, tiltKill, groundPoundKill, strafeKill, driftKill, !(frontFlipKill || tiltKill || groundPoundKill || strafeKill || burnoutKill || swoleKill || driftKill), burnoutKill, swoleKill };
    }
    public override string ToString()
    {
        if (!(frontFlipKill || tiltKill || groundPoundKill || strafeKill || burnoutKill || swoleKill))
        {
            if (driftKill)
            {
                return "Drifting Kill";
            }
            else
            {
                return "Driving Kill";
            }
        }
        int count = (tiltKill ? 1 : 0) +
                    (strafeKill ? 1 : 0) +
                    (groundPoundKill ? 1 : 0) +
                    (frontFlipKill ? 1 : 0) +
                    (burnoutKill ? 1 : 0) + 
                    (swoleKill ? 1 : 0);
        string[] attrs = new string[count];
        int idx = 0;
        if (swoleKill)
        {
            attrs[idx] = "Swole";
            idx++;
        }
        if (tiltKill)
        {
            attrs[idx] = "Tilt";
            idx++;
        }
        if (strafeKill)
        {
            attrs[idx] = "Strafe";
            idx++;
        }
        if(groundPoundKill)
        {
            attrs[idx] = "Ground Pound";
            idx++;
        }
        if(frontFlipKill)
        {
            attrs[idx] = "Flip";
            idx++;
        }
        if (burnoutKill)
        {
            attrs[idx] = "Burnout";
        }

        return string.Join(" + ", attrs) + " Kill";
    }
    public override int GetHashCode()
    {
        return  (swoleKill ? 64 : 0) +
                (burnoutKill ? 32 : 0) +
                (tiltKill ? 16 : 0) +
                (strafeKill ? 8 : 0) +
                (groundPoundKill ? 4 : 0) +
                (frontFlipKill ? 2 : 0)+
                (strafeKill ? 1 : 0);
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            NewStyleKey nsk = (NewStyleKey)obj;
            return frontFlipKill == nsk.frontFlipKill &&
                    tiltKill == nsk.tiltKill &&
                    groundPoundKill == nsk.groundPoundKill &&
                    strafeKill == nsk.strafeKill &&
                    driftKill == nsk.driftKill &&
                    burnoutKill == nsk.burnoutKill &&
                    swoleKill == nsk.swoleKill;
        }
    }
}
public class NewStyleSystem : MonoBehaviour
{
    private class StyleStale
    {
        NewStyleKey styleKey;
        Queue<float> killBuffer;
        float bufferTime = 3f;
        public int decay = 0;
        float decayFraction = .9f;
        int points;
        int totalPoints;
        int comboBonus = 25;
        int decayAmount = 25;
        int maxDecay = 3;
        float timer = 0;
        public StyleStale(NewStyleKey nsk, KillInfo killInfo)
        {
            styleKey = nsk;
            killBuffer = new Queue<float>();
            var killInfoArray = killInfo.GetPointsAsArray();
            var boolArray = nsk.GetKeyAsArray();
            for (int i = 0; i < killInfoArray.Count; i++)
            {
                if (!boolArray[i])
                {
                    killInfoArray[i] = (0, 0, 0, 0f);
                }
            }
            killInfoArray.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            points = killInfoArray.Select(x => x.Item1).Take(2).Sum();
            points = (points == 0) ? killInfo.drivingKillPoints : points;
            bufferTime = killInfoArray.Select(x => x.Item4).Max();
            comboBonus = killInfoArray.Select(x => x.Item2).Max();
            comboBonus = !(nsk.tiltKill || nsk.strafeKill || nsk.groundPoundKill || nsk.burnoutKill || nsk.swoleKill) && nsk.frontFlipKill ? killInfo.frontFlipBonusPoints : comboBonus;
            decayAmount = killInfoArray.Select(x => x.Item3).Max();
            decayAmount = !(nsk.tiltKill || nsk.strafeKill || nsk.groundPoundKill || nsk.burnoutKill) && nsk.frontFlipKill ? killInfo.frontFlipStalePoints : decayAmount;
            bufferTime = (bufferTime == 0) ? killInfo.drivingKillTime : bufferTime;
        }
        public void EndCombo()
        {
            killBuffer.Clear();
            decay = 0;
        }
        public void Update()
        {
            timer -= Time.deltaTime;
            if(timer < 0  && killBuffer.Count > 0)
            {
                var temp = GetInfo();
                string info = temp.Item1 + " x" + temp.Item2;
                StyleTextbox.instance.AddRewardInfoAndHandleTimeoutNoRand(info, temp.Item4, 1, decay);
                while (killBuffer.Count > 0)
                {
                    killBuffer.Dequeue();
                }
                totalPoints = 0;
                instance.StaleMove(styleKey);

            }
        }
        public void AddKill()
        {
            timer = bufferTime;
            killBuffer.Enqueue(Time.time);
        }
        public int GetPoints()
        {
            int currDecay = Mathf.Clamp(decay, 0, maxDecay);
            int p = Mathf.Max((points)-currDecay*decayAmount,100);
            return p;
        }
        public int GetBonusPoints()
        {
            return Mathf.Min(killBuffer.Count, 5) * comboBonus;
        }
        public void AddToTotalPoints(int p)
        {
            totalPoints += (int)(p * CarManager.pointMultiplier);
        }
        public float GetTimeLeft()
        {
            if (killBuffer.Count > 0)
            {
                return timer + 1;
            }
            return 1;
        }
        public (string, int, float, int, int) GetInfo()
        {
            return (styleKey.ToString(), killBuffer.Count, GetTimeLeft(), totalPoints, decay);
        }

    }

    public KillInfo info;
    public GameObject[] CombosText;
    private TextMeshProUGUI[] CombosTextboxes;
    public TextAsset dataFile;
    public StyleLevel[] levels;

    private Dictionary<NewStyleKey, string> styleMusicState;
    private Queue<(NewStyleKey, float)> staleQueue;

    private Queue<(int, float)> slomoQueue;
    public float slomoTimeWindow;
    private float pointsInQueue;
    private float lastStopped;
    public float minTimebetweenStops;
    public float pointsToStop;
    
    private Dictionary<NewStyleKey, int> staleCounter;
    public float staleTimeout = 10f;
    private Dictionary<NewStyleKey, StyleStale> styleStaleMap;

    public int multiplier = 1;
    public int realMultiplier = 1;
    public int maxMultiplier = 8;
    public float multiplierTime = 1f;
    public float timer = 0;
    public int staleLength = 8;

    private float gibTimer = 0;
    public float gibTime = 1f;

    public bool reduceZombieNoises;
    public int numToReduceNoises = 3;
    public static NewStyleSystem instance;
    void Start()
    {
        staleCounter = new Dictionary<NewStyleKey, int>();
        instance = this;
        styleStaleMap = new Dictionary<NewStyleKey, StyleStale>();
        staleQueue = new Queue<(NewStyleKey, float)>(staleLength);
        slomoQueue = new Queue<(int, float)>();
        for (int i = 0; i < levels.Length; i++)
        {
            //styleMusicState.Add(levels[i], "");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMultiplier();
        StaleCounterTimeout();
        SlomoQueueUpdate();
        foreach (var staling in styleStaleMap)
        {
            staling.Value.Update();
        }
        UpdateComboTexts();
        UpdateGibTimer();
    }

    private void UpdateGibTimer()
    {
        if (gibTimer > 0)
        {
            gibTimer -= Time.deltaTime;
        }
        else
        {
            CarManager.Instance.shouldGib = false;
        }
    }

    void UpdateComboTexts()
    {
        foreach (var kvp in styleStaleMap)
        {
            var temp = kvp.Value.GetInfo();
            if (temp.Item2 == 0)
            {
                continue;
            }
            string info = temp.Item1 + " x" + temp.Item2;
            StyleTextbox.instance.AddRewardInfo(info, temp.Item4, temp.Item3, temp.Item5);
        }
    }
    void UpdateMultiplier()
    {
        if (multiplier == 1)
        {
            timer = 0;
        }
        else if(CarManager.Instance.carController.isGrounded)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                EndCombo();
            }
        }
        else
        {
            timer -= Time.deltaTime/4;
            if (timer < 0)
            {
                EndCombo();
            }
        }
    }
    void SlomoQueueUpdate()
    {
        reduceZombieNoises = slomoQueue.Count >= numToReduceNoises;
        while(slomoQueue.Count > 0 && Time.time > slomoQueue.Peek().Item2 + slomoTimeWindow)
        {
            pointsInQueue -= slomoQueue.Peek().Item1;
            slomoQueue.Dequeue();
        }
    }


    public bool AddCarKill()
    {
        NewStyleKey newStyleKey = new NewStyleKey();
        CarManager.Instance.swoleKill = false;
        if (!styleStaleMap.ContainsKey(newStyleKey))
        {
            styleStaleMap[newStyleKey] = new StyleStale(newStyleKey, info);
        }
        styleStaleMap[newStyleKey].AddKill();
        int rawPoints = styleStaleMap[newStyleKey].GetPoints();
        int bonusPoints = styleStaleMap[newStyleKey].GetBonusPoints();
        slomoQueue.Enqueue((rawPoints, Time.time));
        pointsInQueue += rawPoints;
        bool shouldStop = false;
        if(pointsInQueue >= pointsToStop && Time.time > lastStopped + minTimebetweenStops)
        {
            lastStopped = Time.time;
            shouldStop = true;
            CarManager.Instance.shouldGib = true;
            gibTimer = gibTime;
        }
        styleStaleMap[newStyleKey].AddToTotalPoints(rawPoints * realMultiplier + bonusPoints * (int)realMultiplier/2);
        AddPoints(rawPoints * realMultiplier + bonusPoints * (int)realMultiplier / 2);
        //StaleMove(newStyleKey);
        return shouldStop;
    }
    public void AddPoints(int points)
    {
        multiplier += 1;
        realMultiplier = Mathf.FloorToInt(Mathf.Pow(8 * multiplier, .3333f));

        //realMultiplier = Mathf.Min(multiplier, maxMultiplier);
        timer = multiplierTime;
        CarManager.numPoints += points * CarManager.pointMultiplier;
    }
    public int AddPointsWithMultiplier(int points)
    {
        points *= realMultiplier;
        multiplier += 1;
        realMultiplier = Mathf.FloorToInt(Mathf.Pow(8*multiplier, .3333f));
        //realMultiplier = Mathf.Min(multiplier, maxMultiplier);
        timer = multiplierTime;
        CarManager.numPoints += points * CarManager.pointMultiplier;
        return points;
    }
    public void EndCombo()
    {
        if (!CarManager.Instance.inTutorial)
            AudioManager.instance.PlayComboDroppedVoiceLine(multiplier);
        multiplier = 1;
        realMultiplier = multiplier;
        timer = 0;
    } 
    public void PauseCombo()
    {
        gameObject.SetActive(false);
    }
    public void ResumeCombo()
    {
        gameObject.SetActive(true);
    }

    public void StaleCounterTimeout()
    {
        bool changed = false;
        while(staleQueue.Count > 0 && staleQueue.Peek().Item2 + staleTimeout < Time.time)
        {
            staleQueue.Dequeue();
            changed = true;
        }
        if (changed)
        {
            ResetStaleCounter();
        }
    }
    public void ResetStaleCounter()
    {
        foreach (var kvp in staleCounter)
        {
            styleStaleMap[kvp.Key].decay = 0;
        }
        staleCounter.Clear();
        foreach (var (key,time) in staleQueue)
        {
            if (staleCounter.ContainsKey(key))
            {
                staleCounter[key]++;
            }
            else
            {
                staleCounter[key] = 1;
            }
        }
        foreach(var kvp in staleCounter)
        {
            styleStaleMap[kvp.Key].decay = kvp.Value;
        }
    }

    public void StaleMove(NewStyleKey styleKey)
    {
        staleQueue.Enqueue((styleKey,Time.time));
        if(staleQueue.Count > staleLength)
        {
            staleQueue.Dequeue();
        }
        ResetStaleCounter();
    }
}

