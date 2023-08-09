using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrieNode
{
    public bool isEndNode;
    public Dictionary<StyleKey, TrieNode> children;
    public int points;
    public TrieNode(bool end = false)
    {
        children = new Dictionary<StyleKey, TrieNode>();
        isEndNode = end;
    }
}
public class StyleComboTrie : MonoBehaviour
{
    public TrieNode root;
    public TextAsset dataFile;
    // Start is called before the first frame update
    void Start()
    {
        root = new TrieNode();
        ProcessData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCombo(string[] combo)
    {
        TrieNode curr = root;
        int i;
        for(i = 0; i < combo.Length-1; i++)
        { 
            string[] skInfo = combo[i].Split(':');
            StyleKey childKey;
            try
            {
                childKey = new StyleKey(skInfo[0], skInfo[1]);
            }
            catch(System.ArgumentException e)
            {
                Debug.Log(e.Data);
                return;
            }
            if (!curr.children.ContainsKey(childKey))
            {
                curr.children[childKey] = new TrieNode();
            }
            curr = curr.children[childKey];
        }
        curr.isEndNode = true;
        curr.points = int.Parse(combo[i]);
    }
    public void ProcessData()
    {
        string[] lines = dataFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Trim('\r').Split(',');
            AddCombo(data);

        }
    }
}
