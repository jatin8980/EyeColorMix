using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PatternData", order = 1)]
public class PatternData : ScriptableObject
{
    public List<Sprite> patternSprites;
    public List<Vector2> patternSize;
    public List<float> patternBottom;
    public int minSliderValue;
    public int maxSliderValue;
    public int defaultValue;
    public bool stretchWidth;
}