using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    // Array de prefabs de obstáculos que se generarán
    public GameObject[] obstaclePrefabs;
    
    // Intervalo de tiempo entre la generación de obstáculos
    public float generatorTimer = 2.0f;

    // Bandera que indica si se está generando un obstáculo
    private bool isGenerating = false;

    // Lista que almacena los Rigidbody2D de los obstáculos generados
    private List<Rigidbody2D> obstacleRigidbodies = new List<Rigidbody2D>();

    void Start()
    {
        // Método Start vacío
    }

    // Método para generar un obstáculo
    void CreateObstacle()
    {
        if (!isGenerating)
        {
            isGenerating = true;

            // Seleccionar un índice aleatorio para elegir un prefab de obstáculo
            int randomIndex = Random.Range(0, obstaclePrefabs.Length);

            // Instanciar el prefab de obstáculo en la posición del generador
            GameObject newObstacle = Instantiate(obstaclePrefabs[randomIndex], transform.position, Quaternion.identity);

            // Obtener el componente Rigidbody2D del nuevo obstáculo
            Rigidbody2D obstacleRigidbody = newObstacle.GetComponent<Rigidbody2D>();

            // Si se encuentra el Rigidbody2D, agregarlo a la lista de obstáculos generados
            if (obstacleRigidbody != null)
            {
                obstacleRigidbodies.Add(obstacleRigidbody);
            }

            // Iniciar una corrutina para restablecer la bandera de generación
            StartCoroutine(ResetGenerating());
        }
    }

    // Método para iniciar la generación automática de obstáculos
    public void StartGenerator()
    {
        InvokeRepeating("CreateObstacle", 0f, generatorTimer);
    }

    // Método para detener la generación automática de obstáculos
    public void StopGenerator()
    {
        CancelInvoke("CreateObstacle");
    }

    // Corrutina para restablecer la bandera de generación después de un tiempo
    IEnumerator ResetGenerating()
    {
        yield return new WaitForSeconds(generatorTimer);
        isGenerating = false;
    }

    // Método para detener el movimiento de los obstáculos
    public void StopObstaclesMovement()
    {
        // Lista temporal para almacenar los obstáculos que necesitan ser detenidos
        List<Rigidbody2D> obstaclesToStop = new List<Rigidbody2D>();

        // Iterar a través de los obstáculos generados
        foreach (Rigidbody2D obstacleRigidbody in obstacleRigidbodies)
        {
            // Verificar si el Rigidbody2D o su GameObject son nulos
            if (obstacleRigidbody == null || obstacleRigidbody.gameObject == null)
            {
                continue; // Saltar este obstáculo
            }

            // Verificar si el obstáculo es del tipo "Ball" y omitirlo si es así
            if (obstacleRigidbody.gameObject.CompareTag("Ball"))
            {
                continue; // Saltar este obstáculo
            }

            // Agregar el Rigidbody2D del obstáculo a la lista de obstáculos a detener
            obstaclesToStop.Add(obstacleRigidbody);
        }

        // Iterar a través de los obstáculos que necesitan ser detenidos
        foreach (Rigidbody2D obstacleRigidbody in obstaclesToStop)
        {
            // Detener el movimiento del obstáculo
            obstacleRigidbody.velocity = Vector2.zero;

            // Detener las animaciones si existen
            Animator animator = obstacleRigidbody.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = 0f;
            }
        }
    }
}
