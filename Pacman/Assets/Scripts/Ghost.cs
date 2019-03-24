using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public bool canMove = true;

    public Mode currentMode = Mode.Chase;

    public Node startNode;

    public float timeToRelease = 10.0f;

    public float normalMoveSpeed = 3.9f;
    public float frightendedMoveSpeed = 2.5f;
    public float consumedMoveSpeed = 40.0f;

    public int socre = 200;

    public int frightendedTime = 7;
    public int startBlinkingAt = 4;

    public int scatterTimer1 = 7;
    public int chaseTimer1 = 20;
    public int scatterTimer2 = 7;
    public int chaseTimer2 = 20;
    public int scatterTimer3 = 7;
    public int chaseTimer3 = 20;
    public int scatterTimer4 = 7;
    public int chaseTimer4 = 20;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostWhite;
    public RuntimeAnimatorController ghostBlue;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public Transform ghostHouse;

    protected PacMan pacMan;
    protected GameBoard gameBoard;
    protected Vector2 targetPostion;  // for test only 

    private Mode previousMode;
    private float currentSpeed = 0.0f;
    private int modeChangeInteration = 1;
    private float modeChangeTimer = 0;
    private float frightendedModeTimer = 0;
    private float blinkTimer = 0;
    private bool isWhite = false;
    private float releaseTimer = 0.0f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightended,
        Consumed
    }

    protected Node currentNode, targetNode;
    protected Vector2 direction, nextDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameBoard = FindObjectOfType<GameBoard>();
        pacMan = FindObjectOfType<PacMan>();
        ResetPositionAndDirection();

    }

    // Start is called before the first frame update
    void Start()
    {
        animator.runtimeAnimatorController = ghostUp;
        InvokeRepeating("UpdateAnimation", 0.0f, 0.1f);
        //targetNode = ChoseNextNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            ModeUpdate();
            if (releaseTimer > timeToRelease)
            {
                MovementUpdate();
            }
            else if (releaseTimer < timeToRelease)
            {
                releaseTimer += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void OnDrawGizmos()
    {
        if (targetPostion != null)
            Debug.DrawLine(transform.position, targetPostion);
    }

    private void MovementUpdate()
    {
        if (currentNode != targetNode && targetNode != null)
        {
            if (OverShotTarget())
            {
                transform.position = targetNode.transform.position;
                currentNode = targetNode;
                targetNode = ChoseNextNode();
                CheckPortal();
            }
            else if (canMove)
            {
                transform.localPosition += (Vector3)direction * currentSpeed * gameBoard.multibleSpeed * Time.deltaTime;
            }
        }
        else if ((targetNode == null || direction == Vector2.zero) && releaseTimer > timeToRelease)
        {
            targetNode = ChoseNextNode();
        }

    }

    private void UpdateAnimation()
    {
        switch (currentMode)
        {
            case Mode.Chase:
            case Mode.Scatter:
                NormalMovementAnimeUpdate();
                break;
            case Mode.Frightended:
                FrightendedAnimeUpdate();
                break;
            case Mode.Consumed:
                ConsumedAnimeUpdate();
                break;
            default:
                break;
        }
    }

    private void NormalMovementAnimeUpdate()
    {
        if (direction == Vector2.up)
        {
            animator.runtimeAnimatorController = ghostUp;
        }
        else if (direction == Vector2.down)
        {
            animator.runtimeAnimatorController = ghostDown;
        }
        else if (direction == Vector2.left)
        {
            animator.runtimeAnimatorController = ghostLeft;
        }
        else if (direction == Vector2.right)
        {
            animator.runtimeAnimatorController = ghostRight;
        }
        else
        {
            animator.runtimeAnimatorController = ghostUp;
        }
    }

    private void FrightendedAnimeUpdate()
    {
        if (isWhite)
            animator.runtimeAnimatorController = ghostWhite;
        else
            animator.runtimeAnimatorController = ghostBlue;
    }

    private void ConsumedAnimeUpdate()
    {
        animator.runtimeAnimatorController = null;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (direction == Vector2.up)
        {
            spriteRenderer.sprite = eyesUp;
        }
        else if (direction == Vector2.down)
        {
            spriteRenderer.sprite = eyesDown;
        }
        else if (direction == Vector2.left)
        {
            spriteRenderer.sprite = eyesLeft;
        }
        else if (direction == Vector2.right)
        {
            spriteRenderer.sprite = eyesRight;
        }
    }

    void Consumed()
    {
        ChangeMode(Mode.Consumed);
        currentSpeed = consumedMoveSpeed;
        pacMan.AddScore(socre);
    }

    void ModeUpdate()
    {
        if (currentMode == Mode.Chase || currentMode == Mode.Scatter)
        {
            modeChangeTimer += Time.deltaTime;

            if (modeChangeInteration == 1)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseTimer1)
                {
                    modeChangeInteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChangeInteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseTimer2)
                {
                    modeChangeInteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChangeInteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseTimer3)
                {
                    modeChangeInteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }

            if (modeChangeInteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseTimer4)
                {
                    modeChangeInteration = 1;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
        }
        else if (currentMode == Mode.Frightended)
        {
            frightendedModeTimer += Time.deltaTime;

            if (frightendedModeTimer > frightendedTime)
            {
                frightendedModeTimer = 0.0f;
                ChangeMode(previousMode);
            }

            if (frightendedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= 0.1f)
                {
                    if (isWhite)
                    {
                        isWhite = false;
                    }
                    else
                    {
                        isWhite = true;
                    }
                    blinkTimer = 0.0f;
                }
            }
        }
        else if (currentMode == Mode.Consumed)
        {
            Vector2 startNodePosition = startNode.transform.position;
            float distanceToGhostHouse = Vector2.Distance(startNodePosition, transform.position);
            if (distanceToGhostHouse < 0.1f)
            {
                transform.position = startNodePosition;
                currentNode = startNode;
                releaseTimer = 0.0f;
                ChangeMode(previousMode);
                direction = Vector2.zero;
            }
        }
    }

    private void CheckCollision()
    {
        float dis = Vector3.Distance(GetPostition(), pacMan.GetPosition());
        if (dis < 1.0f)
        {
            if (IsCanBeConsumed())
                Consumed();

            if (IsCanConsumePacman())
                gameBoard.StartDeath();
        }
    }

    private bool IsCanBeConsumed()
    {
        return currentMode == Mode.Frightended;
    }

    private bool IsCanConsumePacman()
    {
        return currentMode != Mode.Consumed && currentMode != Mode.Frightended && !pacMan.isConsumed;
    }

    private void CheckPortal()
    {
        Transform otherPortal = gameBoard.GetPortal(transform.position);

        if (otherPortal != null)
        {
            transform.position = otherPortal.position;
            currentNode = otherPortal.GetComponent<Node>();
            targetNode = ChoseNextNode();
        }
    }

    private Vector2? GetTarget(Mode mode)
    {
        switch (mode)
        {
            case Mode.Chase:
                currentSpeed = normalMoveSpeed;
                return OnChaseModeNextTarget();
            case Mode.Scatter:
                currentSpeed = normalMoveSpeed;
                return OnScatterModeNextTarget();
            case Mode.Frightended:
                currentSpeed = frightendedMoveSpeed;
                return OnFrightendedNextTarget();
            case Mode.Consumed:
                currentSpeed = consumedMoveSpeed;
                return OnConsumedNextTarget();
            default:
                return pacMan.transform.position;
        }
    }

    private Vector2? OnConsumedNextTarget()
    {
        return startNode.transform.position;
    }

    private Vector2? OnFrightendedNextTarget()
    {
        Node[] neighbors = currentNode.neighbors;
        float maxDis = 0.0f;
        Vector2 target = ghostHouse.position;
        for (int i = 0; i < neighbors.Length; i++)
        {
            float dis = Vector2.Distance(currentNode.transform.position, neighbors[i].transform.position);
            if (dis > maxDis)
            {
                maxDis = dis;
                target = neighbors[i].transform.position;
            }
        }

        return target;
    }

    private Vector2? OnScatterModeNextTarget()
    {
        return ghostHouse.position;
    }

    public virtual Vector2? OnChaseModeNextTarget()
    {
        throw new NotImplementedException();
    }

    private Node ChoseNextNode()
    {
        Vector2? target = GetTarget(currentMode);

        if (target.HasValue == false) return null;

        targetPostion = target.Value; // for test only 

        Node moveToNode = null;

        Node[] foundNode = new Node[currentNode.neighbors.Length];
        Vector2[] foundNodeDirection = new Vector2[currentNode.validDirection.Length];

        int nodeCounter = 0;

        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if ((currentMode == Mode.Consumed || !currentNode.neighbors[i].isGhostCase) && currentNode.validDirection[i] != -direction)
            {
                foundNode[nodeCounter] = currentNode.neighbors[i];
                foundNodeDirection[nodeCounter] = currentNode.validDirection[i];
                nodeCounter++;
            }
        }

        if (foundNode.Length > 0)
        {
            float leastDistance = 10000.0f;
            for (int i = 0; i < foundNode.Length; i++)
            {
                if (foundNode[i] != null)
                {
                    float distance = Vector2.Distance(foundNode[i].transform.position, target.Value);
                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNode[i];
                        direction = foundNodeDirection[i];
                    }
                }
            }
        }

        return moveToNode;
    }

    void ChangeMode(Mode mode)
    {
        if (currentMode == mode) return;
        if (currentMode == Mode.Chase || currentMode == Mode.Scatter)
            previousMode = currentMode;
        currentMode = mode;
    }

    Node CanMove(Vector2 dir)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirection[i] == dir)
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
        Vector2 vec = targetPostion - (Vector2)currentNode.transform.position;
        return vec.sqrMagnitude;
    }

    public Vector2 GetPostition()
    {
        return new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public void StartFrightended()
    {
        if (currentMode != Mode.Consumed)
        {
            frightendedModeTimer = 0.0f;
            currentSpeed = frightendedMoveSpeed;
            isWhite = false;
            ChangeMode(Mode.Frightended);
        }
    }

    private void ResetPositionAndDirection()
    {
        currentNode = startNode;
        targetNode = startNode;
        direction = Vector2.zero;
        transform.position = startNode.transform.position;

    }

    public void Restart()
    {
        ResetPositionAndDirection();
        ResetTimer();
        ResetMode();
        animator.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void ResetMode()
    {
        currentMode = Mode.Chase;
    }

    private void ResetTimer()
    {
        blinkTimer = 0.0f;
        frightendedModeTimer = 0.0f;
        modeChangeTimer = 0.0f;
        releaseTimer = 0.0f;
    }

    public void StopAnim()
    {
        animator.enabled = false;
    }

    public void StartAnim()
    {
        animator.enabled = true;
    }
}
