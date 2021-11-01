using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialTimer : MonoBehaviour
{
    float currentTime = 0f;
    float startingTime = 4f;

    //public TextMeshPro countdownText;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        //countdownText.text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            SceneManager.LoadScene("Race");
        }
    }
}