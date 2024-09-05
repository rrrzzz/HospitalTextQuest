using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextPlayback : MonoBehaviour
{
    public const string GameCloseStateName = "GameCloses";
    public const string TrueEndingStateName = "3dScene";
    private const string DefaultChoicePrefix = "     ";
    private const string CurrentChoicePrefix = "->  ";
    private const int InvisibleCharactersLength = 13;
    private const char TagStartChar = '[';
    private const string DrugState = "B1";
 

    public State StartingState;
    public State CurrentState;
    public bool IsExecuting;
    
    private bool _isAllTextShown;
    private float _previousTime;
    private TextMeshProUGUI _sceneText;
    private int _currentChoiceIndex;
    private bool _shouldPlay = true;
    private int _taggedTextCurrentChar;
    private string _taggedText;
    private bool _isTextTagged;
    private bool _isDrugTaken;
    

    public static event EventHandler NewScreenEncountered;
    public static event EventHandler<string> TagEncountered;
    
    [Range(0.1f, 1)]
    public float ShowCharacterDelay = 0.6f;

    // Use this for initialization
    void Start ()
    {
        _sceneText = GetComponent<TextMeshProUGUI>();
        _sceneText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0);
        CurrentState = StartingState;
        Initialize();
    }

    // Update is called once per frame
	void Update ()
	{
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneManager.LoadScene(1);
        }
#endif
        if (!IsExecuting) return;

        if (ChoiceRecorder.ShouldReplayChoices)
        {
            if (_shouldPlay)
            {
                _shouldPlay = false;
                StartCoroutine(ReplayChoicesIfNeeded());    
            }
            return;
        }
        
	    if (CurrentState.StateType == StateType.Choice && _isAllTextShown)
	    {
	        if (Input.GetKeyDown(KeyCode.UpArrow))
	        {
	            TryChangeChoiceIndex(-1);
	        }
	        else if (Input.GetKeyDown(KeyCode.DownArrow))
	        {
	            TryChangeChoiceIndex(1);
	        }
	    }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && _isAllTextShown) ChangeState();
	    else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) ShowAllText();
	    else if (!_isAllTextShown) ShowNextCharacter();
    }

    public void Initialize()
    {
        InitializeTimeAndText();
        InitializeChoices();
    }

    public IEnumerator PausePlayback(float length)
    {
        IsExecuting = false;
        yield return new WaitForSeconds(length);
        IsExecuting = true;
    }

    private IEnumerator ReplayChoicesIfNeeded()
    {
        ShowAllText();
        yield return new WaitForSeconds(.5f);
        var initIndex = 0;
        if (CurrentState.StateType == StateType.Choice)
        {
            var targetIndex = ChoiceRecorder.Choices[ChoiceRecorder.ChoiceListIndex++];
            while (targetIndex != initIndex)
            {
                TryChangeChoiceIndex(1);
                yield return new WaitForSeconds(.5f);
                initIndex++;
            }
            ChangeShownChoice();
        }
        ChangeState();
    }

    private void InitializeTimeAndText()
    {
        _previousTime = Time.realtimeSinceStartup;
        _sceneText.maxVisibleCharacters = 0;
        _sceneText.text = CurrentState.StoryText;
        // _taggedText = CurrentState.StoryTextTagged;
        // _isTextTagged = _taggedText.Contains(TagStartChar);
        _taggedTextCurrentChar = 0;
        _isAllTextShown = false;
    }

    private void InitializeChoices()
    {
        if (CurrentState.StateType != StateType.Choice) return;

        _currentChoiceIndex = 0;

        if (CurrentState.GetChoices().Count == 0)
        {
            if (CurrentState.FirstChoice.Length > 0) CurrentState.GetChoices().Add(CurrentState.FirstChoice);
            if (CurrentState.SecondChoice.Length > 0 ) CurrentState.GetChoices().Add(CurrentState.SecondChoice);
            if (CurrentState.ThirdChoice.Length > 0) CurrentState.GetChoices().Add(CurrentState.ThirdChoice);
        }

        ChangeShownChoice();
    }

    private void TryChangeChoiceIndex(int i)
    {
        if (_currentChoiceIndex + i < 0 || _currentChoiceIndex + i > CurrentState.GetChoices().Count - 1) return;

        _currentChoiceIndex += i;
        ChangeShownChoice();
    }

    private void ChangeShownChoice()
    {
        _sceneText.text = CurrentState.StoryText;
        _sceneText.text += "\n\n";

        for(var i = 0; i < CurrentState.GetChoices().Count; i++)
        {
            if (i == _currentChoiceIndex)
            {
                _sceneText.text += CurrentChoicePrefix;
                _sceneText.text += "<b><i>" + CurrentState.GetChoices()[i] + "</i></b>";
            }
            else
            {
                _sceneText.text += DefaultChoicePrefix;
                _sceneText.text += CurrentState.GetChoices()[i];
            }

            if (i != CurrentState.GetChoices().Count - 1)
            {
                _sceneText.text += "\n";
            }
        }
    }

    private void ShowNextCharacter()
    {
        var textLength = CurrentState.StateType == StateType.Choice
            ? _sceneText.text.Length - InvisibleCharactersLength
            : _sceneText.text.Length;

        if (Time.realtimeSinceStartup - _previousTime > ShowCharacterDelay / 10)
        {
            _previousTime = Time.realtimeSinceStartup;
            _sceneText.maxVisibleCharacters++;
            CheckTagStart();
            _taggedTextCurrentChar++;
        }

        if (_sceneText.maxVisibleCharacters == textLength)
        {
            _isAllTextShown = true;
        }
    }

    private void CheckTagStart()
    {
        if (!_isTextTagged || _taggedText[_taggedTextCurrentChar] != TagStartChar) return;

        var strBld = new StringBuilder();
        while (_taggedText[++_taggedTextCurrentChar] != ']') 
            strBld.Append(_taggedText[_taggedTextCurrentChar]);
        
        OnTagEncountered(strBld.ToString());
        _taggedTextCurrentChar++;
    }

    private void ChangeState()
    {

        if (CurrentState.StateType == StateType.Choice)
        {
            CurrentState = CurrentState.NextStates[_currentChoiceIndex];
            if (!ChoiceRecorder.ShouldReplayChoices)
            {
                ChoiceRecorder.Choices.Add(_currentChoiceIndex);
            }
        }
        else
        {
            if (CurrentState.NextStates.Length == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("game over");
                Application.Quit();
                return;
            }
            CurrentState = CurrentState.NextStates[0];
        }

        CheckStateDrug();

        if (CheckStateRequiresNewScreen()) return;

        Initialize();
    }

    private void CheckStateDrug()
    {
        if (CurrentState.name == DrugState) _isDrugTaken = true;
    }

    private bool CheckStateRequiresNewScreen()
    {
        _shouldPlay = true;
        if (CurrentState.IsNewScreen) OnNewScreenEncountered();
        return CurrentState.IsNewScreen;
    }

    protected virtual void OnNewScreenEncountered()
    {
        var handler = NewScreenEncountered;
        handler?.Invoke(this, EventArgs.Empty);
    }

    private void ShowAllText()
    {
        _sceneText.maxVisibleCharacters = _sceneText.text.Length;
        _isAllTextShown = true;
    }

    void OnTagEncountered(string fxTag)
    {
        var handler = TagEncountered;
        handler?.Invoke(this, fxTag);
    }
}