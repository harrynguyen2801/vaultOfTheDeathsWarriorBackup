using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance => _instance;
    private static MainSceneManager _instance;
    public GameObject profile;

    public GameObject[] levelList;
    public GameObject player;
    public GameObject enemySpawn;
    public EndScreenManager endingScreen;

    public bool winOrLose;

    private void Awake()
    {
        ShowCurrentLevel();
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
        profile.SetActive(true);
    }

    private void ShowCurrentLevel()
    {
        int level = DataManager.Instance.GetDataInt(DataManager.EDataPrefName.Level);
        Debug.Log(level + " level");
        if (PlayerPrefs.HasKey("Level"))
        {
            levelList[level-1].gameObject.SetActive(true);
            player.GetComponent<Transform>().position = levelList[level - 1].GetComponent<GameLevelManager>().playerStartPosition.position;
        }
        else
        {
            levelList[0].gameObject.SetActive(true);
            player.GetComponent<Transform>().position = levelList[0].GetComponent<GameLevelManager>().playerStartPosition.position;
            DataManager.Instance.SaveData(DataManager.EDataPrefName.Level,1);
        }
    }

    public void ShowNextLevel(int level)
    {
        int levelSave = DataManager.Instance.GetDataInt(DataManager.EDataPrefName.Level);
        levelList[levelSave-1].gameObject.SetActive(false);
        levelList[level-1].gameObject.SetActive(true);
        player.GetComponent<Transform>().position = levelList[level-1].GetComponent<GameLevelManager>().playerStartPosition.position;
        StartCoroutine(waitSecond(3f));
        player.GetComponent<Player>().AppearPlayerInGame();
    }
    
    IEnumerator waitSecond(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
}
