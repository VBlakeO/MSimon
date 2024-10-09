using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Image[] colorButtons;
    [SerializeField] private AudioClip[] colorSounds;
    [Space]
    
    [SerializeField] private GameObject gameOver = null;
    [Space]

    [SerializeField] private TextMeshProUGUI inputContText = null;
    [SerializeField] private TextMeshProUGUI levelCountText = null;

    private List<int> colorSequence = new List<int>(); 
    private List<int> playerInput = new List<int>();

    private int inputCont = 0;
    private int levelCount = 0;
    private int currentStep = 0; 

    private bool playerTurn = false;

    public void PlayGame()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        AddRandomColor();
        StartCoroutine(PlaySequence());
    }

    void AddRandomColor()
    {
        int randomIndex = Random.Range(0, colorButtons.Length);
        colorSequence.Add(randomIndex);
    }

    IEnumerator PlaySequence()
    {
        playerTurn = false;

        playerInput.Clear();
        currentStep = 0;

        for (int i = 0; i < colorSequence.Count; i++)
        {
            int buttonIndex = colorSequence[i];
            Image button = colorButtons[buttonIndex];

            inputCont = i + 1;
            inputContText.text = inputCont.ToString();

            StartCoroutine(HighlightButton(button, colorSounds[buttonIndex]));
            yield return new WaitForSeconds(1f);
        }

        playerTurn = true;
    }

    IEnumerator HighlightButton(Image button, AudioClip sound)
    {
        button.enabled = true;
        AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);

        yield return new WaitForSeconds(0.5f);

        button.enabled = false;
    }

    public void PlayerSelect(int buttonIndex)
    {
        if (!playerTurn) return;

        playerInput.Add(buttonIndex);

        inputCont = colorSequence.Count - playerInput.Count;
        inputContText.text = inputCont.ToString();

        StartCoroutine(HighlightButton(colorButtons[buttonIndex], colorSounds[buttonIndex]));

        if (playerInput[currentStep] == colorSequence[currentStep])
        {
            currentStep++;

            if (currentStep >= colorSequence.Count)
                StartCoroutine(NextRound());
        }
        else
        {
            gameOver.SetActive(true);
            StartCoroutine(RestartGame());
        }
    }

    IEnumerator NextRound()
    {
        levelCountText.text = "" + levelCount++;
        yield return new WaitForSeconds(2f);
        AddRandomColor();
        StartCoroutine(PlaySequence());
    }

    IEnumerator RestartGame()
    {
        levelCount = 0;
        levelCountText.text = levelCount.ToString();
        yield return new WaitForSeconds(3f);
        gameOver.SetActive(false);

        colorSequence.Clear();
        StartCoroutine(StartGame());
    }
}