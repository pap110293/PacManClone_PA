using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    private static readonly int boarWidth = 28;
    private static readonly int boarHeight = 36;
    public GameObject[,] boarGame = new GameObject[boarWidth, boarHeight];
    public int pacManLife = 3;
    [HideInInspector]
    public int totalPallets;
    public float multibleSpeed = 1.0f;

    public Text readyText;
    public Text playerText;
    public Text scoreText;
    public Text highScoreText;
    public Text roundText;

    public Image pacmanLife2;
    public Image pacmanLife3;

    public AudioClip backgroudAudioNormal;
    public AudioClip backgroudFrightened;
    public AudioClip backgroudPacmanDeath;
    public AudioClip backgroudIntro;

    private int round = 1;
    private int highScore;
    private AudioSource audioSource;
    private PacMan pacMan;
    private Ghost[] ghosts;
    private bool didStartDeath = false;

    private void Awake()
    {
        totalPallets = FindObjectsOfType<Tile>().Count(i => i.isPellet || i.isSuperPellet);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        audioSource = GetComponent<AudioSource>();
        ghosts = FindObjectsOfType<Ghost>();
        pacMan = FindObjectOfType<PacMan>();

        Tile[] objecs = FindObjectsOfType<Tile>();

        foreach (Tile t in objecs)
        {
            if(t.GetComponent<Tile>() != null || t.GetComponent<Node>() != null)
            {
                Vector2 pos = t.transform.position;
                boarGame[(int)pos.x, (int)pos.y] = t.gameObject;
            }

            //if (!o.CompareTag("Ghost") && !o.CompareTag("Player") && !o.CompareTag("GhostHouse"))
            //{
            //    Vector2 pos = o.transform.position;
            //    boarGame[(int)pos.x, (int)pos.y] = o;
            //}
        }
    }

    private void Start()
    {
        highScoreText.text = highScore.ToString();
        StartGame();
        InvokeRepeating("LevelUp", 10.0f, 10.0f);
    }

    void LevelUp()
    {
        
        multibleSpeed += 0.05f;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = pacMan.GetScore()+"";
        if(pacManLife == 3)
        {
            pacmanLife2.enabled = true;
            pacmanLife3.enabled = true;
        }
        if (pacManLife == 2)
        {
            pacmanLife2.enabled = true;
            pacmanLife3.enabled = false;
        }
        if (pacManLife == 1)
        {
            pacmanLife2.enabled = false;
            pacmanLife3.enabled = false;
        }

        roundText.text = round.ToString();
    }

    private void StartGame()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.canMove = false;
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        pacMan.canMove = false;
        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<AudioSource>().Stop();

        StartCoroutine(StartGameAffter(4.3f));
    }

    IEnumerator StartGameAffter(float delay)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(backgroudIntro);
        yield return new WaitForSeconds(delay/2);

        foreach (Ghost ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
        }

        playerText.enabled = false;
        yield return new WaitForSeconds(delay / 2);

        foreach (Ghost ghost in ghosts)
        {
            ghost.canMove = true;
        }

        pacMan.canMove = true;
        pacMan.GetComponent<Animator>().enabled = true;
        PlayBackgoundAudioNormal();
        readyText.enabled = false;
    }

    public GameObject GetTile(Vector2 pos)
    {
        return boarGame[(int)pos.x, (int)pos.y];
    }

    public Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GetTile(transform.position);

        if (tile != null)
            return tile.GetComponent<Node>();

        return null;
    }

    public Transform GetPortal(Vector2 pos)
    {
        var tile = GetTile(pos)?.GetComponent<Tile>();
        if (tile != null && tile.isPortal)
            return tile?.GetComponent<Tile>()?.portalReceiver;
        return null;
    }

    public Tile GetConsumePellet(Vector2 pos)
    {
        Tile tile = GetTile(pos)?.GetComponent<Tile>();
        if (tile != null && !tile.didConsume && (tile.isPellet || tile.isSuperPellet))
        {
            return tile;
        }
        return null;
    }

    public void StartWining()
    {
        StartCoroutine(WiningProcess(2.0f));
    }

    IEnumerator WiningProcess(float delay)
    {
        pacMan.StopAnim();
        pacMan.canMove = false;

        audioSource.Stop();

        foreach (Ghost ghost in ghosts)
        {
            ghost.canMove = false;
            ghost.StopAnim();
        }

        yield return new WaitForSeconds(delay);

        pacMan.GetComponent<SpriteRenderer>().enabled = false;

        foreach (Ghost ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        readyText.text = "Level up!!";
        readyText.color = Color.green;
        InvokeRepeating("BlinkText", 0.0f, 0.5f);

        yield return new WaitForSeconds(4.5f);
        CancelInvoke("BlinkText");
        readyText.enabled = false;

        Tile[] tiles = FindObjectsOfType<Tile>();
        for (int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i].isPellet || tiles[i].isSuperPellet)
            {
                tiles[i].Restart();
            }
        }

        pacMan.consumedPallet = 0;
        round++;
        StartCoroutine(ProcessRestart(4.3f));

        
    }

    void BlinkText()
    {
        readyText.enabled = !readyText.enabled;
    }

    public void Restart()
    {
        pacMan.Restart();

        foreach (var ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = true;
            ghost.Restart();
        }
    }

    public void StartDeath()
    {
        if (!didStartDeath)
        {
            didStartDeath = true;
        }

        foreach (Ghost ghost in ghosts)
        {
            ghost.canMove = false;
        }

        pacMan.isConsumed = true;
        pacMan.canMove = false;
        pacMan.GetComponent<Animator>().enabled = false;
        pacMan.GetComponent<AudioSource>().Stop();
        audioSource.Stop();

        StartCoroutine(ProcessDeathAffter(1.5f));
    }

    IEnumerator ProcessDeathAffter(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var ghost in ghosts)
        {
            ghost.GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(ProcessDeathAnimation(2.5f));
    }

    IEnumerator ProcessDeathAnimation(float delay)
    {
        pacMan.transform.eulerAngles = new Vector3(0, 0, 0);

        var animator = pacMan.GetComponent<Animator>();
        animator.runtimeAnimatorController = pacMan.pacManDeathAnimation;
        animator.enabled = true;

        PlayBackgoundAudioPacmanDeath();

        yield return new WaitForSeconds(delay);
        pacManLife -= 1;

        StartCoroutine(ProcessRestart(4.3f));
    }

    IEnumerator ProcessRestart(float delay)
    {

        if(pacManLife == 0)
        {
            CheckHighScore();
            playerText.enabled = true;
            readyText.enabled = true;
            readyText.text = "GAME OVER";
            readyText.color = Color.red;
            audioSource.Stop();
            yield return new WaitForSeconds(delay / 2);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else
        {
            playerText.enabled = true;
            readyText.text = "Ready!!";
            readyText.enabled = true;
            audioSource.PlayOneShot(backgroudIntro);
            Restart();
            pacMan.GetComponent<Animator>().enabled = false;

            foreach (var ghost in ghosts)
            {
                ghost.GetComponent<SpriteRenderer>().enabled = false;
            }

            yield return new WaitForSeconds(delay / 2);

            playerText.enabled = false;
            foreach (var ghost in ghosts)
            {
                ghost.GetComponent<SpriteRenderer>().enabled = true;
            }

            yield return new WaitForSeconds(delay / 2);

            pacMan.GetComponent<Animator>().enabled = true;
            PlayBackgoundAudioNormal();
            foreach (Ghost ghost in ghosts)
            {
                ghost.canMove = true;
            }
            pacMan.canMove = true;
            readyText.enabled = false;
        }

        
    }

    private void CheckHighScore()
    {
        if(pacMan.GetScore() > highScore)
        {
            PlayerPrefs.SetInt("HighScore", pacMan.GetScore());
        }
    }

    public void PlayBackgoundAudioNormal()
    {
        PlayeAudio(backgroudAudioNormal);
        audioSource.loop = true;
    }

    public void PlayBackgoundAudioFrightened()
    {
        PlayeAudio(backgroudFrightened);
        audioSource.loop = true;
    }

    public void PlayBackgoundAudioPacmanDeath()
    {
        PlayeAudio(backgroudPacmanDeath);
        audioSource.loop = false;
    }

    private void PlayeAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
