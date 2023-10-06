using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;



public struct StyleKey
{
    public ZombieKillbox.faces face;
    public CarManager.CarState carState;

    public StyleKey(ZombieKillbox.faces face, CarManager.CarState carState)
    {
        this.face = face;
        this.carState = carState;
    }
    public StyleKey(string face, string carState)
    {
        ZombieKillbox.faces convFace;
        CarManager.CarState convCarState;

        if (!ZombieKillbox.faces.TryParse(face, out convFace))
        {
            throw new System.ArgumentException("face");
        }
        if (!CarManager.CarState.TryParse(carState, out convCarState))
        {
            throw new System.ArgumentException("state");
        }
        this.face = convFace;
        this.carState = convCarState;
    }
    /*
    public bool Equals(StyleKey x, StyleKey y)
    {
        return x.face == y.face && x.carState == y.carState; 
    }
    public int GetHashCode(StyleKey x)
    {
        return face.GetHashCode() ^ carState.GetHashCode();
    }
    */

}

[System.Serializable]
public struct StyleLevel
{
    public string levelName;
    public float levelTime;
    public int multiplier;
    public int pointsToNextLevel;
    public string styleMusicState;
}


public class StyleSystem : MonoBehaviour
{
    private class StyleStale
    {
        StyleKey style;
        Queue<float> killBuffer;
        float bufferTime = 3f;
        public int decay = 0;
        float decayFraction = .9f;
        int points;
        float time;
        public StyleStale(int points, StyleKey style)
        {
            killBuffer = new Queue<float>();
            this.points = points;
            this.style = style;
        }
        public void EndCombo()
        {
            killBuffer.Clear();
            decay = 0;
        }
        public void Update()
        {
            time = Time.time;
            if(killBuffer.Count > 0 && killBuffer.Peek()+bufferTime < Time.time)
            {
                while(killBuffer.Count > 0)
                {
                    killBuffer.Dequeue();
                }
                instance.StaleMove(style);
            }
        }
        public void AddKill()
        {
            killBuffer.Enqueue(Time.time);
        }
        public int GetPoints()
        {
            int x = (int)(points * Mathf.Pow(decayFraction, (float)decay) / 100) * 100;
            return x;
        }
    }

    public GameObject slider;
    public GameObject levelText;
    public GameObject levelInfo;
    public GameObject multiplierText;
    public GameObject totalPoints;
    public GameObject[] CombosText;
    private Slider sliderObject;
    private TextMeshProUGUI levelTextObject;
    private TextMeshProUGUI multiplierTextObject;
    private TextMeshProUGUI totalPointsTextObject;
    private TextMeshProUGUI levelPointsObject;
    private TextMeshProUGUI[] CombosTextboxes;
    public TextAsset dataFile;
    public StyleLevel[] levels;


    private Dictionary<StyleLevel, string> styleMusicState;
    private Queue<(StyleKey, float)> timedKills;
    private Queue<(StyleKey, float)> staleQueue;
    private Dictionary<StyleKey, int> staleCounter;
    public float staleTimeout = 10f;
    private Dictionary<StyleKey, int> killCounter;
    public float killExpirationTime = 5f;
    private Dictionary<StyleKey, StyleStale> styleStaleMap;
    private float timeLeft = 0;
    private int level = -1;
    private int currLevelTotal = 0;
    private int comboTotal = 0;
    public int staleLength;
    public static StyleSystem instance;
    void Start()
    {
        instance = this;
        sliderObject = slider.GetComponent<Slider>();
        levelTextObject = levelText.GetComponent<TextMeshProUGUI>();
        multiplierTextObject = multiplierText.GetComponent<TextMeshProUGUI>();
        totalPointsTextObject = totalPoints.GetComponent<TextMeshProUGUI>();
        levelPointsObject = levelInfo.GetComponent<TextMeshProUGUI>();
        styleStaleMap = new Dictionary<StyleKey, StyleStale>();
        CombosTextboxes = new TextMeshProUGUI[CombosText.Length];
        for (int i = 0; i < CombosText.Length; i++)
        { 
            CombosTextboxes[i] = CombosText[i].GetComponent<TextMeshProUGUI>();
        }
        timedKills = new Queue<(StyleKey, float)>();
        killCounter = new Dictionary<StyleKey, int>();
        staleQueue = new Queue<(StyleKey, float)>(staleLength);
        staleCounter = new Dictionary<StyleKey, int>();
        ReadData();
        for (int i = 0; i < levels.Length; i++)
        {
            styleMusicState.Add(levels[i], "");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (level >= 0)
        {
            timeLeft -= Time.deltaTime;
        }
        if (timeLeft <= 0 && level <= 0)
        {
            EndCombo();
            
        }
        else if(timeLeft <= 0)
        {
            level--;
            timeLeft = levels[level].levelTime;
            currLevelTotal = 0;
        }
        else
        {
            while (timedKills.Count > 0 && Time.time - killExpirationTime > timedKills.Peek().Item2)
            {
                var kill = timedKills.Dequeue();
                killCounter[kill.Item1]--;
            }
            var sortedKills = killCounter.ToList();
            sortedKills.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            for (int i = 0; i < CombosText.Length; i++)
            {
                if (sortedKills.Count > i && sortedKills[i].Value > 0f)
                {
                    CombosTextboxes[i].text = sortedKills[i].Key.face.ToString() + "," + sortedKills[i].Key.carState.ToString() + " x" + sortedKills[i].Value.ToString();
                }
                else
                {
                    CombosTextboxes[i].text = "";
                }
            }
            StaleCounterTimeout();
            foreach (var staling in styleStaleMap)
            {
                staling.Value.Update();
            }
            sliderObject.value = timeLeft / levels[level].levelTime;
            levelTextObject.text = levels[level].levelName;
            multiplierTextObject.text = "x" + levels[level].multiplier.ToString();
            totalPointsTextObject.text = "Combo Total : " + comboTotal.ToString();
            levelPointsObject.text = currLevelTotal.ToString() + " / " + levels[level].pointsToNextLevel.ToString();
        }

    }

    void ReadData()
    {
        string[] lines = dataFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Trim('\r').Split(',');
            for (int j = 1; j < data.Length; j++)
            {
                StyleKey sk = new StyleKey((ZombieKillbox.faces)(i - 1), (CarManager.CarState)(j - 1));
                styleStaleMap[sk] = new StyleStale(int.Parse(data[j]), sk);
            }

        }
    }
     private void AddPoints(int points)
    {
        if(level == -1)
        {
            level = 0;
            AudioManager.instance.SetState(levels[level].styleMusicState);
            timeLeft = levels[level].levelTime;
            currLevelTotal = levels[level].multiplier * points;
            comboTotal = currLevelTotal;
            gameObject.SetActive(true);
            //AudioManager.instance.SetState(true);
        }
        else
        {
            timeLeft += (float)points / 2000;
            if(timeLeft > levels[level].levelTime)
            {
                timeLeft = levels[level].levelTime;
            }
            currLevelTotal += levels[level].multiplier * points;
            comboTotal += levels[level].multiplier * points;
            if(currLevelTotal >= levels[level].pointsToNextLevel)
            {
                currLevelTotal = 0;
                level = Mathf.Min(level+1, levels.Length-1);
                AudioManager.instance.SetState(levels[level].styleMusicState);
                timeLeft = levels[level].levelTime;
            }
        }
    }
    public void AddPoints(CarManager.CarState carState, ZombieKillbox.faces face)
    {
        StyleKey sk = new StyleKey(face, carState);
        timedKills.Enqueue((sk, Time.time));
        styleStaleMap[sk].AddKill();
        if (killCounter.ContainsKey(sk))
        {
            killCounter[sk]++;
        }
        else
        {
            killCounter[sk] = 1;
        }
        AddPoints(styleStaleMap[sk].GetPoints());
    }
    public void EndCombo()
    {
        CarManager.numPoints += comboTotal;
        comboTotal = 0;
        currLevelTotal = 0;
        level = -1;
        gameObject.SetActive(false);
        foreach(var kvp in styleStaleMap)
        {
            kvp.Value.EndCombo();
        }
        //AudioManager.instance.SetState(false);
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

    public void StaleMove(StyleKey styleKey)
    {
        staleQueue.Enqueue((styleKey,Time.time));
        ResetStaleCounter();
    }
}
