using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum ScoreType
{
    None,
    Perfect,
    Good,
    Miss
}

public enum GameState
{
    None,
    Playing,
    End,
}
public struct BeatInfo
{
    public float time;
    public int buttonIndex;
    public BeatInfo(float t, int b) { time = t; buttonIndex = b; }
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;

    [SerializeField] private GameObject NoteSpawner;
    [SerializeField] private NoteSpawner noteSpawner;
    private int nextBeatIndex = 0;
    [SerializeField] private AudioSource audioSource;

    public string csvFileName = "beats"; // Without .csv extension
    public List<BeatInfo> beatTimes = new List<BeatInfo>();
    
    [SerializeField] private float delay; // Delay before the song starts
    [SerializeField] private TMPro.TextMeshProUGUI endText;
    [SerializeField] private UnityEngine.UI.Button endButton;
    private GameState gameState = GameState.None;

    void Start()
    {
        LoadCSV();
        if (audioSource != null)
        {
            audioSource.PlayDelayed(delay); // Only delay audio
        }
        gameState = GameState.Playing;
        if (endText != null) endText.gameObject.SetActive(false);
        if (endButton != null) endButton.gameObject.SetActive(false);
    }

    void LoadCSV()
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError($"CSV file '{csvFileName}.csv' not found in Resources folder.");
            return;
        }

        string[] lines = csvData.text.Split('\n');

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Skip empty lines or headers
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.ToLower().Contains("time"))
                continue;

            string[] parts = trimmedLine.Split(',');
            if (parts.Length >= 2 &&
                float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float beatTime) &&
                int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int buttonIndex))
            {
                beatTimes.Add(new BeatInfo(beatTime, buttonIndex));
            }
            else
            {
                Debug.LogWarning($"Invalid line in CSV: {trimmedLine}");
            }
        }

        Debug.Log($"Loaded {beatTimes.Count} beat times.");
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    private void EndGame()
    {
        gameState = GameState.End;
        if (endText != null) endText.gameObject.SetActive(true);
        if (endButton != null) endButton.gameObject.SetActive(true);
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }

    private void Update()
    {

        Debug.Log("isPlaying: ");
        Debug.Log(audioSource.isPlaying);
        Debug.Log("audiosource.time: ");
        Debug.Log(audioSource.time);
        Debug.Log("audiosource.clip.length: ");
        Debug.Log(audioSource.clip.length);
        // If the audio has stopped or reached the end, end the game
        if ((!audioSource.isPlaying || audioSource.time >= audioSource.clip.length) && gameState == GameState.Playing)
        {
            EndGame();
            return;
        }
        
        if (gameState == GameState.End)
            return;
        if (beatTimes.Count == 0 || noteSpawner == null || audioSource == null || nextBeatIndex >= beatTimes.Count)
            return;

        float songTime = audioSource.time + delay; // Compensate for audio delay
        var beatInfo = beatTimes[nextBeatIndex];
        if (songTime >= beatInfo.time)
        {
            noteSpawner.SpawnNote(beatInfo.buttonIndex);
            nextBeatIndex++;
        }

        if (InputSystem.actions.FindAction("Reset").IsPressed())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if (InputSystem.actions.FindAction("EndSong").IsPressed())
        {
            EndGame();
        }
    }

    public void OnEndButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(ScoreType st)
    {
        switch (st)
        {
            case ScoreType.Perfect:
                score += 100;
                break;
            case ScoreType.Good:
                score += 50;
                break;
            case ScoreType.Miss:
                score -= 20;
                break;
            default:
                break;
        }
    }
}

