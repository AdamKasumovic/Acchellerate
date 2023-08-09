using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StyleTextbox : MonoBehaviour
{
    public GameObject[] textboxObjects;
    public GameObject[] backdropObjects;
    public Image[] backdrops;
    private TextMeshProUGUI[] textboxes;
    public List<(string, int, float, int, bool)> moves;
    public List<(string, int, float, int, bool)> movesQueue;
    public static StyleTextbox instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        textboxes = new TextMeshProUGUI[textboxObjects.Length];
        backdrops = new Image[backdropObjects.Length];
        for (int i = 0; i < textboxObjects.Length; i++)
        {
            textboxes[i] = textboxObjects[i].GetComponent<TextMeshProUGUI>();
        }
        for(int i = 0; i < backdropObjects.Length; i++)
        {
            backdrops[i] = backdropObjects[i].GetComponent<Image>();
        }
        moves = new List<(string, int, float, int, bool)>();
        movesQueue = new List<(string, int, float, int, bool)>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < movesQueue.Count; i++)
        {
            movesQueue[i] = (movesQueue[i].Item1, movesQueue[i].Item2, movesQueue[i].Item3 - Time.deltaTime, movesQueue[i].Item4, movesQueue[i].Item5);
        }
        movesQueue.RemoveAll(elem => elem.Item3 <= 0);
        for (int i = 0; i < movesQueue.Count; i++)
        {
            moves.Add(movesQueue[i]);
        }
    }
    private void LateUpdate()
    {
        moves.Sort((x, y) => x.Item3.CompareTo(y.Item3));
        for (int i = 0; i < textboxes.Length; i++)
        {
            textboxes[i].text = " ";
        }
        for(int i = 0; i < backdrops.Length; i++)
        {
            backdrops[i].enabled = false;
        }
        int k = textboxes.Length - 1;
        int l = backdrops.Length - 1;
        for (int j = moves.Count - 1; j >= Mathf.Max(moves.Count - textboxes.Length,0); j--)
        {
            if (!moves[j].Item5) { 
                textboxes[k].text = $"<size=120%>{moves[j].Item2}pts <size=75%>"+moves[j].Item1;
                textboxes[k].color = moves[j].Item4 == 0 ? Color.white : new Color(1f, 1f - .166f * moves[j].Item4, 1f - .166f * moves[j].Item4, 1);
                textboxes[k].alpha = moves[j].Item3 > 1 ? 1 : moves[j].Item3;
                textboxes[k].ForceMeshUpdate();
                if(l >= 0)
                {
                    backdrops[l].enabled = true;
                    backdrops[l].rectTransform.sizeDelta = new Vector2(textboxes[k].preferredWidth + 4*backdrops[l].rectTransform.sizeDelta.y, backdrops[l].rectTransform.sizeDelta.y);
                }
            }
            else
            {
                textboxes[k].text = $"<size=120%>{moves[j].Item2}pts  <size=75%>" + moves[j].Item1;
                textboxes[k].color = new Color(0.831372549f, .68627451f, .215686275f);
                textboxes[k].alpha = moves[j].Item3 > 1 ? 1 : moves[j].Item3;
                textboxes[k].ForceMeshUpdate();
                if (l >= 0)
                {
                    backdrops[l].enabled = true;
                    backdrops[l].rectTransform.sizeDelta = new Vector2(textboxes[k].preferredWidth + 4 * backdrops[l].rectTransform.sizeDelta.y, backdrops[l].rectTransform.sizeDelta.y);
                }
            }
            k--;
            l--;
        }
        moves.Clear();
    }
    public void AddRewardInfo(string text, int points, float time, int decay = 0)
    {
        moves.Add(new(text, points, time, decay, false));
    }
    public void AddRewardInfoAndHandleTimeout(string text, int points, float time, int decay = 0, bool highValue =false)
    {
        movesQueue.Add(new(text, points, time+Random.Range(-.1f,.1f), decay, highValue));
    }
    public void AddRewardInfoAndHandleTimeoutNoRand(string text, int points, float time, int decay = 0, bool highValue = false)
    {
        movesQueue.Add(new(text, points, time, decay, highValue));
    }
}
