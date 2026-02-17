using Firebase;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public TMP_Text LeaderboardText;

    public GameObject leaderboardRowPrefab;
    public Transform leaderboardContent;


    FirebaseFirestore db;

    void Awake()
    {
        Instance = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
            }
            else
            {
                Debug.LogError("Firebase not ready.");
            }
        });
    }

    public void SaveScore(string playerName, int score)
    {
        if (db == null) return;

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "playerName", playerName },
            { "score", score },
            { "date", System.DateTime.Now.ToString("dd-MM-yyyy") }
        };

        db.Collection("scores").AddAsync(data);
    }

    //public void PrintLeaderboard()
    //{
    //    db.Collection("scores")
    //      .OrderByDescending("score")
    //      .GetSnapshotAsync()
    //      .ContinueWith(task =>
    //      {
    //          if (task.IsCompleted && !task.IsFaulted)
    //          {
    //              QuerySnapshot snapshot = task.Result;

    //              foreach (DocumentSnapshot doc in snapshot.Documents)
    //              {
    //                  string name = doc.GetValue<string>("playerName");
    //                  int score = doc.GetValue<int>("score");
    //                  string date = doc.GetValue<string>("date");

    //                  Debug.Log(name + " - " + score + " - " + date);
    //              }
    //          }
    //          else
    //          {
    //              Debug.LogError("Error fetching leaderboard: " + task.Exception);
    //          }
    //      });
    //}

    public async void PrintLeaderboard()
    {
        // Clear old rows
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        QuerySnapshot snapshot = await db.Collection("scores")
                                         .OrderByDescending("score")
                                         .GetSnapshotAsync();

        int rank = 1;

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            string name = doc.GetValue<string>("playerName");
            int score = doc.GetValue<int>("score");
            string date = doc.GetValue<string>("date");

            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContent);

            row.transform.Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();
            row.transform.Find("Name").GetComponent<TMP_Text>().text = name;
            row.transform.Find("Score").GetComponent<TMP_Text>().text = score.ToString();
            row.transform.Find("Date").GetComponent<TMP_Text>().text = date;

            rank++;
        }
    }

}