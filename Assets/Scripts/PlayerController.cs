using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Referencias a los objetos del juego necesarios para el controlador del jugador
    public GameObject game;
    public GameObject obstacleGenerator;

    // Componentes necesarios del jugador
    private Animator animator;
    private AudioSource audioPlayer;

    // Clips de audio para diferentes eventos del juego
    public AudioClip dieClip;
    public AudioClip jumpClip;
    public AudioClip pointClip;

    // Posición inicial en Y del jugador
    private float startY;

    // Bandera que indica si se ha reproducido el sonido de muerte del jugador
    private bool hasPlayedDieSound = false;

    // Sistema de partículas de polvo
    public ParticleSystem dust;

    void Start()
    {
        // Obtener los componentes del jugador
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();

        // Guardar la posición inicial en Y del jugador
        startY = transform.position.y;
    }

    void Update()
    {
        // Verificar si el jugador está en el suelo, si el juego está en curso y si se ha presionado el botón del mouse
        bool isGrounded = transform.position.y == startY;
        bool gamePlaying = game.GetComponent<GameController>().gameState == GameState.Playing;
        bool userAction = Input.GetMouseButtonDown(0);

        if (gamePlaying && userAction && isGrounded)
        {
            // Si se cumplen las condiciones anteriores, reproducir la animación de salto del jugador y el sonido de salto
            UpdateState("playerjumper");
            audioPlayer.clip = jumpClip;
            audioPlayer.Play();   
        }
    }

    // Método para actualizar el estado de la animación del jugador
    public void UpdateState(string state = null)
    {
        if (state != null)
        {
            animator.Play(state);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el jugador ha chocado con un obstáculo o una bola y no se ha reproducido el sonido de muerte
        if (!hasPlayedDieSound && (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Ball")))
        {
            // Si es así, reproducir la animación de muerte del jugador, cambiar el estado del juego a "Terminado",
            // detener la generación de obstáculos y su movimiento, detener la música de fondo, ajustar el tiempo de escala del juego,
            // reproducir el sonido de muerte del jugador y actualizar la bandera hasPlayedDieSound
            UpdateState("playerdie");
            game.GetComponent<GameController>().gameState = GameState.Ended;
            stopDust();
            obstacleGenerator.SendMessage("StopGenerator");
            obstacleGenerator.SendMessage("StopObstaclesMovement");
            game.GetComponent<AudioSource>().Stop();
            game.SendMessage("ResetTimeScale",0.7f);
            
            if (dieClip != null && audioPlayer != null)
            {
                audioPlayer.clip = dieClip;
                audioPlayer.Play();
                hasPlayedDieSound = true;
            }
        }
        // Verificar si el jugador ha pasado por un punto y el juego está en curso
        else if (game.GetComponent<GameController>().gameState == GameState.Playing && other.gameObject.CompareTag("Point"))
        {
            // Si es así, aumentar los puntos del juego y reproducir el sonido de punto
            game.SendMessage("IncreasePoints");
            audioPlayer.clip = pointClip;
            audioPlayer.Play();
        }
    }

    // Método para iniciar la emisión de partículas de polvo
    public void startDust()
    {
        dust.Play();
    }

    // Método para detener la emisión de partículas de polvo
    public void stopDust()
    {
        dust.Stop();
    }
}
