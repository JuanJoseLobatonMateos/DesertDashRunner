using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleController : MonoBehaviour
{
    // Velocidad del obstáculo
    public float velocity = 2f;

    private Rigidbody2D rigid2d; // Referencia al componente Rigidbody2D del obstáculo

    void Start()
    {
        rigid2d = GetComponent<Rigidbody2D>(); // Obtener el componente Rigidbody2D del obstáculo
        rigid2d.velocity = Vector2.left * velocity; // Establecer la velocidad inicial del obstáculo hacia la izquierda
    }

    void Update()
    {
        // Método vacío, no se utiliza en este script
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si el obstáculo entra en contacto con el objeto etiquetado como "Destroyer", se destruye
        if (other.gameObject.tag == "Destroyer")
        {
             Destroy(gameObject);
        }
    }
}
