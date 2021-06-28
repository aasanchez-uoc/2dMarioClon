using System.Collections;
using UnityEngine;
/// <summary>
/// Clase encargada de manejar al enemigo, de mostrar la animación de morir y destruirse
/// </summary>
public class EnemyController : MonoBehaviour
{

    /// <summary>
    /// Referecnia a la clase Animator del enemigo
    /// </summary>
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Método encargado de matar al enemigo
    /// </summary>
    public void Kill()
    {
            //inciamos la animacion de muerte
            animator.SetBool("dead", true);

            //desactivamos los colliders del enemigo
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach(Collider2D col in colliders)
            {
            col.enabled = false;
            }
            //iniciamos la corrutina que lo destruye
            StartCoroutine(DestroyCoroutine());
    }

    /// <summary>
    /// Corrutina encargada de destruir al objeto tras esperar un segundo
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}
