using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGame()
    {
        GameManager.Instance.LoadNewGame();
    }

    public void OnQuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
