using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ImageSelector : MonoBehaviour
{
    public LevelPoints data;
    private Image image;
    private Image exitImage;
    private TextMeshProUGUI text;
    public static ImageSelector instance;
    public Sprite[] sprites;
    public Sprite[] exitSignSprites;
    public GameObject imagego;
    public GameObject exit;
    public GameObject exitSignText;
    public int[] souls;
    // Start is called before the first frame update
    private void Awake()
    {


    }
    void Start()
    {
        instance = this;
        image = imagego.GetComponent<Image>();
        exitImage = exit.GetComponent<Image>();
        text = exitSignText.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetImageOnWin()
    {
        int points = (int)CarManager.numPoints;
        int gradeIdx = data.GetGradeFromPoints(points);
        //string fileName = "highwaySign-WIN-" + grade.name[0].ToString() + ".png";
        image.sprite = sprites[gradeIdx];
        exitImage.sprite = exitSignSprites[gradeIdx];
        text.text = "Souls Gained: " + souls[gradeIdx];
    }
}
