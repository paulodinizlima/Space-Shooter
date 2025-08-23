using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartMenu", 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartMenu()
    {
        SceneManager.LoadScene(0);
    }
}
