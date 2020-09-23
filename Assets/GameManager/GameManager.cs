using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using BobJeltes.Menu;
using BobJeltes.StandardUtilities;
using System;

public class GameManager : Singleton<GameManager>
{
    protected GameManager() { }

    public EndGameScreen EndGameScreenPrefab;
    private EndGameScreen EndGameScreenInstance;

    public List<PlayerController> Players = new List<PlayerController>();

    private void Start()
    {
        Players = FindObjectsOfType<PlayerController>().ToList();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quit game");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private PlayerController PlayerInstance;
    public GameObject DeathScreen;

    public void PlayerDeath(PlayerController player)
    {
        //Players.Remove(player);
        //if (Players.Count == 1)
        //{
        //    MatchComplete();
        //}
        PlayerInstance = player;

        DeathScreen.SetActive(true);
    }

    public void MatchComplete()
    {
        if (EndGameScreenInstance == null)
        {
            EndGameScreenInstance = Instantiate(EndGameScreenPrefab);
            EndGameScreenInstance.gameObject.SetActive(false);
        }
        EndGameScreenInstance.IsWinner = true;
        EndGameScreenInstance.gameObject.SetActive(true);
    }

    public void ActivatePlayer()
    {
        PlayerInstance.gameObject.SetActive(true);
    }
}
