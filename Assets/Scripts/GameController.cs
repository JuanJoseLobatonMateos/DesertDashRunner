using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { StandBy, Playing, Ended };

public class GameController : MonoBehaviour
{
    // Velocidad del efecto de parallax
    public float parallaxSpeed = 0.04f;

    // Imágenes de fondo y suelo
    public RawImage background_0;
    public RawImage background_1;
    public RawImage background_2;
    public RawImage background_3;
    public RawImage ground;

    // Objetos de interfaz de usuario
    public GameObject uiStandBy;
    public GameObject uiScore;
    public GameObject uiEndgame;
    public Text pointsText;
    public Text recordText;

    // Clips de audio para la música de espera y de juego
    public AudioClip standbyMusic;
    public AudioClip gameMusic;

    // Jugador y generador de obstáculos
    public GameObject player;
    public GameObject obstacleGenerator;

    // Estado del juego
    public GameState gameState = GameState.StandBy;

    // Variables de ajuste de tiempo
    public float scaleTime = 8f;
    public float scaleInc = .20f;

    private AudioSource musicPlayer; // Reproductor de música
    private int points = 0; // Puntuación del jugador

    void Start()
    {
        // Configuración inicial del reproductor de música
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.clip = standbyMusic;
        musicPlayer.Play();
    }

    void Update()
    {
        bool userAction = Input.GetMouseButtonDown(0);

        if (gameState == GameState.StandBy)
        {
            // Si estamos en espera y no hay música reproduciéndose
            if (!musicPlayer.isPlaying)
            {
                musicPlayer.clip = standbyMusic;
                musicPlayer.Play();
            }

            // Si el usuario realiza una acción, comenzamos el juego
            if (userAction)
            {
                StartPlaying();
            }
        }
        else if (gameState == GameState.Playing)
        {
            // Si estamos jugando, aplicamos el efecto de parallax
            Parallax();
        }
        else if (gameState == GameState.Ended)
        {
            // Si el juego ha terminado, mostramos la pantalla de fin de juego
            uiEndgame.SetActive(true);

            // Si no hay música reproduciéndose, reproducimos la música de espera
            if (!musicPlayer.isPlaying)
            {
                musicPlayer.clip = standbyMusic;
                musicPlayer.Play();
            }

            // Si el usuario realiza una acción, reiniciamos el juego
            if (userAction)
            {
                RestartGame();
            }
        }
    }

    // Método para comenzar el juego
    void StartPlaying()
    {
        gameState = GameState.Playing;
        uiStandBy.SetActive(false);
        uiScore.SetActive(true);
        ShowTextScore();
        player.SendMessage("UpdateState", "playerrun");
        player.SendMessage("startDust");
        obstacleGenerator.SendMessage("StartGenerator");

        musicPlayer.clip = gameMusic;
        musicPlayer.Play();

        InvokeRepeating("GameTimeScale", scaleTime, scaleTime);
    }

    // Efecto de parallax
    void Parallax()
    {
        float finalSpeed = parallaxSpeed * Time.deltaTime;
        background_0.uvRect = new Rect(background_0.uvRect.x + finalSpeed * 0.1f, 0f, 1f, 1f);
        background_1.uvRect = new Rect(background_1.uvRect.x + finalSpeed * 0.5f, 0f, 1f, 1f);
        background_2.uvRect = new Rect(background_2.uvRect.x + finalSpeed, 0f, 1f, 1f);
        background_3.uvRect = new Rect(background_3.uvRect.x + finalSpeed * 1.5f, 0f, 1f, 1f);
        ground.uvRect = new Rect(ground.uvRect.x + finalSpeed * 4f, 0f, 1f, 1f);
    }

    // Método para reiniciar el juego
    public void RestartGame()
    {
        uiEndgame.SetActive(false);
        ResetTimeScale();
        musicPlayer.clip = standbyMusic;
        musicPlayer.Play();
        SceneManager.LoadScene("DesertDashRunner");
    }

    // Método para ajustar el tiempo de juego
    void GameTimeScale()
    {
        Time.timeScale += scaleInc;
    }

    // Método para reiniciar el tiempo de juego
    void ResetTimeScale(float newTimeScale = 1f)
    {
        CancelInvoke("GameTimeScale");
        Time.timeScale = newTimeScale;
    }

    // Método para incrementar la puntuación del jugador
    public void IncreasePoints()
    {
        points++;
        ShowTextScore();
        if (points >= GetMaxScore())
        {
            recordText.text = "RECORD : " + points.ToString();
            SetMaxScore(points);
        }
    }

    // Método para mostrar la puntuación en la interfaz de usuario
    public void ShowTextScore()
    {
        pointsText.text = "PUNTUACIÓN : " + points.ToString();
        recordText.text = "RECORD : " + GetMaxScore().ToString();
    }

    // Método para obtener la puntuación máxima
    public int GetMaxScore()
    {
        return PlayerPrefs.GetInt("Max Points", 0);
    }

    // Método para establecer la puntuación máxima
    public void SetMaxScore(int currentPoints)
    {
        PlayerPrefs.SetInt("Max Points", currentPoints);
    }
}
