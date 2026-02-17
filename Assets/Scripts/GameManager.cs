using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject HomeScreenPanel;
    public GameObject CountDownPanel;
    public static GameManager Instance;
    public TMP_Text countdownText;
    public AudioSource CountDownBeepSound;
    public int score = 0;
    public TMP_Text scoreText;
    public GameObject ScorePanel;

    [Header("Cherry Settings")]
    public GameObject cherry;
    public Transform cherrySpawnPoint;
    private bool cherrySpawned = false;

    [Header("Strawberry Settings")]
    public GameObject Strawberry;
    public Transform StrawberrySpawnPoint;
    private bool StrawberrySpawned = false;

    [Header("Dot Settings")]
    private int dotsRemaining;
    public int CoinsRemaining;
    public GameObject GameOverPanel;
    public TMP_Text finalscoreText;

    public GameObject SuccessPanel;
    public GameObject RestartButton;
    public GameObject MainMenuButton;
    public string PlayerName;
    public NameValidation NV;
    public bool IsGameStarted = false;
    public TMP_Text ResultText;

    public AudioSource DotHitSound;
    public AudioSource PowerDotHitSound;
    public AudioSource FruitHitSound;
    public AudioSource EnemyHitSound;
    public AudioSource EnemyEatingHitSound;

    void Awake()
    {
        Instance = this;
        dotsRemaining = GameObject.FindGameObjectsWithTag("Dot").Length;
        CoinsRemaining = GameObject.FindGameObjectsWithTag("Coin").Length;
        UpdateScoreUI();
    }

    void Start()
    {
        PlayerController.canMove = false;
        CountDownPanel.SetActive(false);
        ScorePanel.SetActive(false);
    }

    public void StartGame()
    {
        //Debug.Log("StartGame called");
        HomeScreenPanel.SetActive(false);
        //Debug.Log("HomeScreen active: " + HomeScreenPanel.activeSelf);
        //Debug.Log("Enabling countdown panel...");
        CountDownPanel.SetActive(true);
        //Debug.Log("Countdown panel state: " + CountDownPanel.activeSelf);
        PlayerName = NV.GetComponent<NameValidation>().nameInput.text;
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        CountDownBeepSound.Play();
        PlayerController.canMove = false;

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        CountDownPanel.SetActive(false);
        PlayerController.canMove = true;
        IsGameStarted = true;
        ScorePanel.SetActive(true);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        CheckCherrySpawn();
        CheckStrawberrySpawn();
    }

    void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    void CheckCherrySpawn()
    {
        if (score >= 70 && !cherrySpawned)
        {
            SpawnCherry();
        }
    }

    void CheckStrawberrySpawn()
    {
        if (score >= 170 && !StrawberrySpawned)
        {
            SpawnStrawberry();
        }
    }

    void SpawnCherry()
    {
        cherrySpawned = true;
        cherry.transform.position = cherrySpawnPoint.position;
        cherry.SetActive(true);

        StartCoroutine(CherryTimer());
    }

    void SpawnStrawberry()
    {
        StrawberrySpawned = true;
        Strawberry.transform.position = StrawberrySpawnPoint.position;
        Strawberry.SetActive(true);

        StartCoroutine(StrawberryTimer());
    }

    System.Collections.IEnumerator CherryTimer()
    {
        yield return new WaitForSeconds(10f);

        if (cherry.activeSelf)
        {
            cherry.SetActive(false);
        }
    }

    public void CherryCollected()
    {
        AddScore(100);
        cherry.SetActive(false);
    }

    System.Collections.IEnumerator StrawberryTimer()
    {
        yield return new WaitForSeconds(10f);

        if (Strawberry.activeSelf)
        {
            Strawberry.SetActive(false);
        }
    }
    public void StrawberryCollected()
    {
        AddScore(100);
        Strawberry.SetActive(false);
    }

    public void DotCollected()
    {
        dotsRemaining--;
        if (dotsRemaining <= 0)
        {
            LevelComplete(true);
            IsGameStarted = false;
        }
    }

    public void LevelComplete(bool Iswin)
    {
        PlayerController.canMove = false;
        GameOverPanel.SetActive(true);
        if (Iswin)
        {
            ResultText.text = "You Win!";
        }
        else
        {
            ResultText.text = "Game Over!";
        }
        finalscoreText.text = scoreText.text;

        FirebaseManager.Instance.SaveScore(PlayerName, score);
        FirebaseManager.Instance.PrintLeaderboard();
    }

    public void GoToMainMenu()
    {
        //GameOverPanel.SetActive(false);
        //HomeScreenPanel.SetActive(true);
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}