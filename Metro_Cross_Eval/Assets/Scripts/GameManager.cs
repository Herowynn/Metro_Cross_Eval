using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMPro.TextMeshProUGUI Timer;
    public GameObject TimerUI;
    public GameObject EndMenu;
    public TMPro.TextMeshProUGUI MyTimer;
    public TMPro.TextMeshProUGUI BestTime;

    public GameObject Player;
    public GameObject GhostPrefab;

    public bool TimeRunning = true;
    public static GameManager Instance;

    List<SaveGhostPositions> _bestPath;

    float _time = 0f;
    List<SaveGhostPositions> _currentPath = new List<SaveGhostPositions>();

    string _path;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerPrefs.DeleteAll();
        _path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";

        if (File.Exists(_path))
            LoadData();

        TimerUI.SetActive(true);
        TimeRunning = true;
        if(_bestPath != null)
            StartCoroutine(ShowGhost());
        StartCoroutine(RecordGhost());
    }

    private void Update()
    {
        if (TimeRunning)
        {
            _time += Time.deltaTime;

            Timer.text = FormatTime(_time);
        }
        else
        {
            SetNewScore();
        }
    }

    string FormatTime(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float miliseconds = time * 1000;
        miliseconds = (miliseconds % 1000);
        string timeText = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, miliseconds);

        return timeText;
    }

    void SetNewScore()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            if (_time < PlayerPrefs.GetFloat("BestTime"))
            {
                PlayerPrefs.SetFloat("BestTime", _time);
                PlayerPrefs.Save();
                _bestPath.Clear();
                _bestPath = new List<SaveGhostPositions>(_currentPath);
            }
        }
        else
        {
            if (_time < 99999f)
            {
                PlayerPrefs.SetFloat("BestTime", _time);
                PlayerPrefs.Save();
                _bestPath = new List<SaveGhostPositions>(_currentPath);
            }
        }

        TimerUI.SetActive(false);
        EndMenu.SetActive(true);

        MyTimer.text = FormatTime(_time);
        BestTime.text = FormatTime(PlayerPrefs.GetFloat("BestTime"));
    }

    IEnumerator RecordGhost()
    {
        while (TimeRunning)
        {
            SaveGhostPositions ghostPosition = new SaveGhostPositions(Player.transform.position);
            _currentPath.Add(ghostPosition);

            yield return new WaitForSeconds(.15f);
        }

        SaveIntoJson(_bestPath);
    }

    IEnumerator ShowGhost()
    {
        while (TimeRunning)
        {
            GameObject GhostGO = Instantiate(GhostPrefab, _bestPath[0].ToVector3(), Quaternion.identity);
            for (int i = 0; i < _bestPath.Count; i++)
            {
                GhostGO.transform.position = _bestPath[i].ToVector3();
                yield return new WaitForSeconds(.15f);
            }
            
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveIntoJson<SaveGhostPositions> (List<SaveGhostPositions> Positions)
    {
        using StreamWriter writer = new StreamWriter(_path);
        foreach (SaveGhostPositions pos in Positions)
        {
            var json = JsonUtility.ToJson(pos);
            writer.Write(json);
            writer.Write("\n");
        }
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(_path);
        string json = reader.ReadToEnd();

        Debug.Log(json);

        string[] values = json.Split("\n");

        _bestPath = new List<SaveGhostPositions>();

        for(int i = 0; i < values.Length - 1; i++)
        {
            SaveGhostPositions position = JsonUtility.FromJson<SaveGhostPositions>(values[i]);
            _bestPath.Add(position);
        }
    }
}
