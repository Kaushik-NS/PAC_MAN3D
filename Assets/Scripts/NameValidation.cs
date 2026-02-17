using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameValidation : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button startButton;
    public TMP_Text errorText;
    public GameManager GM;

    void Start()
    {
        startButton.interactable = false;
        errorText.gameObject.SetActive(false);

        nameInput.onValueChanged.AddListener(OnNameChanged);
        startButton.onClick.AddListener(OnStartClicked);
    }

    void OnNameChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            startButton.interactable = false;
            errorText.gameObject.SetActive(false);
        }
        else
        {
            startButton.interactable = true;
            errorText.gameObject.SetActive(false);
        }
    }

    void OnStartClicked()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            errorText.gameObject.SetActive(true);
            startButton.interactable = false;
            return;
        }

        errorText.gameObject.SetActive(false);

        //Debug.Log("Starting game with player name: " + nameInput.text);

        GM.StartGame();

        // Load your game scene here
        // SceneManager.LoadScene("GameScene");
    }
}
