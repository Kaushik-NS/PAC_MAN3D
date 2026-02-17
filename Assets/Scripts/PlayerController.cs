using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float rayDistance = 0.6f;
    public LayerMask wallLayer;
    public static bool canMove = false;


    private Vector3 moveDirection;
    private Vector3 desiredDirection;

    public TMP_Text PowerModeStatusText;

    public bool CanEatGhost= false;
    public int GhostEatenCount = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (!canMove)
            return;

        if (IsTyping())
            return;

        GetInput();
        HandleMovement();
        HandleRotation();
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            desiredDirection = Vector3.forward;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            desiredDirection = Vector3.back;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            desiredDirection = Vector3.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            desiredDirection = Vector3.right;
    }

    void HandleMovement()
    {
        // Try changing direction if possible
        if (CanMove(desiredDirection))
        {
            moveDirection = desiredDirection;
        }

        // Stop if blocked
        if (!CanMove(moveDirection))
            return;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (moveDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    bool CanMove(Vector3 dir)
    {
        Ray ray = new Ray(transform.position, dir);
        return !Physics.Raycast(ray, rayDistance, wallLayer);
    }

    bool IsTyping()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return false;

        return EventSystem.current.currentSelectedGameObject
               .GetComponent<TMP_InputField>() != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dot"))
        {
            GameManager.Instance.DotHitSound.Play();
            GameManager.Instance.AddScore(10);
            GameManager.Instance.DotCollected();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("PowerDot"))
        {
            GameManager.Instance.PowerDotHitSound.Play();
            Debug.Log("PowerDot collected");
            StartCoroutine(PowerMode());
            GameManager.Instance.AddScore(50);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Cherry"))
        {
            GameManager.Instance.FruitHitSound.Play();
            GameManager.Instance.CherryCollected();
        }

        if (other.CompareTag("SB"))
        {
            GameManager.Instance.FruitHitSound.Play();
            GameManager.Instance.StrawberryCollected();
        }

        if(other.CompareTag("Coin"))
        {
            GameManager.Instance.FruitHitSound.Play();
            Destroy(other.gameObject);
            GameManager.Instance.CoinsRemaining--;
        }

        if (other.CompareTag("Enemy"))
        {
            if (CanEatGhost)
            { 
                GameManager.Instance.EnemyEatingHitSound.Play();
                Debug.Log("Ghost eaten!");
                GhostEatenCount++;  
                if(GhostEatenCount == 1)
                    GameManager.Instance.AddScore(200);
                else if (GhostEatenCount == 2)
                    GameManager.Instance.AddScore(400);
                else if (GhostEatenCount == 3)
                    GameManager.Instance.AddScore(800);
                else if (GhostEatenCount >= 4)
                    GameManager.Instance.AddScore(1600);
                Destroy(other.gameObject);
                return;
            }
            else
            {
                GameManager.Instance.EnemyHitSound.Play();
                GameManager.Instance.LevelComplete(false);
                canMove = false;
                GameManager.Instance.IsGameStarted = false;
            }
        }
    }
    IEnumerator PowerMode()
    {
        CanEatGhost = true;
        PowerModeStatusText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        PowerModeStatusText.gameObject.SetActive(false);
        CanEatGhost = false;

    }
}
