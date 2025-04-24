using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnContinueGame()
    {
        GameManager.Instance.TogglePause();
    }

    public void OnQuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
