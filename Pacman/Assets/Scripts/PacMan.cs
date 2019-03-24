using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public Node startNode;
    public float speed = 4.0f;
    public GameBoard gameBoard;
    public Sprite idleSprite;
    [HideInInspector]
    public Node currentNode;
    public bool canMove = true;
    public bool isConsumed = false;
    public AudioClip chop1;
    public AudioClip chop2;

    public RuntimeAnimatorController pacManDeathAnimation;
    public RuntimeAnimatorController pacManNormalAnimation;

    [HideInInspector]
    public int consumedPallet = 0;
    private AudioSource audioSource;
    private bool isPlayChop1 = true;
    private int score = 0;
    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    private Node previousNode, targetNode;
    private Animator anim;
    private Ghost[] ghosts;
    private bool isPowerUp = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        canMove = true;
        ghosts = FindObjectsOfType<Ghost>();
        anim = GetComponent<Animator>();
        ResetPositionAndDirection();
    }

    private void ResetPositionAndDirection()
    {
        currentNode = startNode;
        transform.position = startNode.transform.position;
        ChangePosition(Vector2.right);
    }

    void PlayChopChop()
    {
        if (isPlayChop1)
        {
            audioSource.PlayOneShot(chop1);
            isPlayChop1 = false;
        }
        else
        {
            audioSource.PlayOneShot(chop2);
            isPlayChop1 = true;
        }
    }

    public void Restart()
    {
        ResetPositionAndDirection();
        anim.enabled = true;
        anim.runtimeAnimatorController = pacManNormalAnimation;
        GetComponent<SpriteRenderer>().enabled = true;
        isConsumed = false;
    }

    private void Update()
    {
        if (canMove)
        {
            CheckInput();

            Move();

            UpdateOrientation();

            UpdateAnimationState();

            ConsumePallet();
        }

        if(isPowerUp)
        {
            bool allGhostIsNormal = true;
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i].currentMode == Ghost.Mode.Frightended)
                {
                    allGhostIsNormal = false;
                    break;
                }
            }
            if(allGhostIsNormal)
            {
                isPowerUp = false;
                gameBoard.PlayBackgoundAudioNormal();
            }
        }

    }

    private void CheckInput()
    {
        // Up
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ChangePosition(Vector2.up);
        }

        // Down
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ChangePosition(Vector2.down);
        }

        // Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangePosition(Vector2.left);
        }

        // Right
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangePosition(Vector2.right);
        }
    }

    void UpdateAnimationState()
    {
        if (direction == Vector2.zero)
        {
            anim.enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
        {
            anim.enabled = true;
        }
    }

    void ChangePosition(Vector2 dir)
    {
        if (dir != direction)
        {
            if (dir == -direction)
            {
                direction = dir;
                var temp = previousNode;
                previousNode = targetNode;
                targetNode = temp;
            }
            nextDirection = dir;
        }

        if (currentNode != null)
        {
            Node moveToNode = CanMove(dir);

            if (moveToNode != null)
            {
                direction = dir;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    void ConsumePallet()
    {
        Tile pallet = gameBoard.GetConsumePellet(transform.position);
        if (pallet != null && !pallet.didConsume)
        {
            AddScore(pallet.GetScore());
            pallet.Disable();
            consumedPallet++;
            PlayChopChop();

            if (gameBoard.totalPallets == consumedPallet)
            {
                gameBoard.StartWining();
                return;
            }

            if (pallet.isSuperPellet)
            {
                gameBoard.PlayBackgoundAudioFrightened();
                isPowerUp = true;
                for (int i = 0; i < ghosts.Length; i++)
                {
                    ghosts[i].StartFrightended();
                }
            }
        }

    }

    private void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;
                CheckPortal();
                CalculateNextMove();
            }
            else
            {
                transform.localPosition += (Vector3)direction * speed * gameBoard.multibleSpeed * Time.deltaTime;
            }
        }
    }

    private void CalculateNextMove()
    {
        Node moveToNode = CanMove(nextDirection);

        if (moveToNode != null)
        {
            direction = nextDirection;
        }

        if (moveToNode == null)
        {
            moveToNode = CanMove(direction);
        }

        if (moveToNode != null)
        {
            targetNode = moveToNode;
            previousNode = currentNode;
            currentNode = null;
        }
        else
        {
            direction = Vector2.zero;
        }
    }

    private void CheckPortal()
    {
        Transform otherPortal = gameBoard.GetPortal(transform.position);

        if (otherPortal != null)
        {
            transform.position = otherPortal.position;
            currentNode = otherPortal.GetComponent<Node>();
        }
    }

    private void UpdateOrientation()
    {
        // Up
        if (direction == Vector2.up)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }

        // Down
        if (direction == Vector2.down)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, -90.0f);
        }

        // Left
        if (direction == Vector2.left)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        }

        // Right
        if (direction == Vector2.right)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    Node CanMove(Vector2 dir)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirection[i] == dir && !currentNode.neighbors[i].isGhostCase)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }
        return moveToNode;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);
        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPostion)
    {
        Vector2 vec = targetPostion - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    public Vector2 GetCurrentDirection()
    {
        return direction;
    }

    public Node GetTargetNode()
    {
        return targetNode;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore(int point)
    {
        score += point;
    }

    public void StopAnim()
    {
        anim.enabled = false;
    }

    public void StartAnim()
    {
        anim.enabled = true;
    }
}
