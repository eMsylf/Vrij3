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

    Controls controls;
    Controls Controls
    {
        get
        {
            if (controls == null)
            {
                controls = new Controls();
            }
            return controls;
        }
    }

    public void Pause(bool enabled)
    {
        foreach (PlayerController player in Players)
        {
            player.enabled = !enabled;
        }
        CameraController.Instance.enabled = !enabled;
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
        if (DeathScreen == null)
        {
            Debug.LogError("Death screen not assigned to " + name, this);
            return;
        }
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

    public void RespawnPlayer()
    {
        PlayerInstance.Respawn();
    }

    private void Start()
    {
        Players = FindObjectsOfType<PlayerController>().ToList();
    }

    private void OnEnable()
    {
        SubscribeControls();
    }

    private void OnDisable()
    {
        UnsubControls();
    }

    void SubscribeControls()
    {
        Controls.Game.Enable();
        Controls.Game.Reload.performed += _ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UnsubControls()
    {
        Controls.Game.Disable();
        Controls.Game.Reload.performed -= _ => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
