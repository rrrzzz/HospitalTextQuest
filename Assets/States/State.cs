using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(10, 14)]
    public string StoryText;
    
    [TextArea(10, 14)]
    public string StoryTextTagged;

    public string FirstChoice;
    public string SecondChoice;
    public string ThirdChoice;
    public StateType StateType;
    public bool IsNewScreen;
    public State[] NextStates;
    public Sprite BackgroundSprite;
    private readonly List<string> _choices = new List<string>();

    public List<string> GetChoices() => _choices;
}

public enum StateType
{
    Story,
    Choice
}