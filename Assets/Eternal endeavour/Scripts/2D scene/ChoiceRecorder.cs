using System.Collections.Generic;
using UnityEngine;

public class ChoiceRecorder : MonoBehaviour
{
    private static ChoiceRecorder _instance;

    public static List<int> Choices = new List<int>();
    public static int ChoiceListIndex;

    public static bool ShouldReplayChoices;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
