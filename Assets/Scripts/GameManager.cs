using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] float waitForGameOver = 1.5f;
    //Singleton
    public static GameManager Instance { get; private set; }
    PlayerSettings playerSettings;
    ScoreKeeper scoreKeeper;
    ScoreDisplay scoreDisplay;
    RocketMover rocketMover;
    AudioSource audioSource;

    public PlayerSettings PlayerSettings
    {
        get
        {
            if (playerSettings == null)
            {
                playerSettings = GetComponent<PlayerSettings>();
            }
            return playerSettings;
        }
    }

    public ScoreDisplay ScoreDisplay
    {
        get
        {
            if (scoreDisplay == null)
            {
                scoreDisplay = FindObjectOfType<ScoreDisplay>();
            }
            return scoreDisplay;
        }
    }

    public RocketMover Rocket
    {
        get
        {
            if (rocketMover == null)
            {
                rocketMover = FindObjectOfType<RocketMover>();
            }
            return rocketMover;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            scoreKeeper = GetComponent<ScoreKeeper>();
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance.scoreDisplay = null;
            Instance.rocketMover = null;
            Destroy(gameObject);
        }
    }


    public void LoadGameOver()
    {
        //reload scene after 2 seconds
        StartCoroutine(DelayLoadGameOverScene());
    }

    private IEnumerator DelayLoadGameOverScene()
    {
        yield return new WaitForSeconds(waitForGameOver);
        SceneManager.LoadScene("9-GameOverScreen");
    }

    public void LoadNewGame()
    {
        scoreKeeper.ResetScore();
        SceneManager.LoadScene("1-Level");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("0-StartScreen");
    }

    public void PlayRandomClip(AudioClip[] crashSounds, float volume, Vector3? position = null)
    {
        if (crashSounds.Length == 0)
        {
            return;
        }

        AudioClip clip = crashSounds[UnityEngine.Random.Range(0, crashSounds.Length)];
        PlayClip(clip, volume, position);
    }

    public void PlayClip(AudioClip clip, float volume, Vector3? position)
    {
        if (!position.HasValue)
        {
            position = Camera.main.transform.position;
        }

        //always apply camera y, z position
        position = new Vector3(position.Value.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

        //AudioSource.PlayClipAtPoint(clip, position.Value, volume);
        audioSource.PlayOneShot(clip, volume);
    }

    public void LevelCompleted()
    {
        //load next level or end the game if the last
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            LoadGameOver();
        }
        else
        {
            Rocket.ControlsEnabled = false;
            if (ScoreDisplay)
            {
                ScoreDisplay.ShowLevelCompletedUI();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void LevelFailed()
    {
        if (ScoreDisplay)
        {
            ScoreDisplay.ShowLevelFailedUI();
        }
        Rocket.ControlsEnabled = false;
        //LoadGameOver();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    internal void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public ScoreKeeper ScoreKeeper
    {
        get
        { 
            return scoreKeeper;
        }
    }
}
