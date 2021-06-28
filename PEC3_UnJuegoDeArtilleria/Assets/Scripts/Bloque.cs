using UnityEngine;
/// <summary>
/// Esta clase implementa la funcionalidad de los distintos bloques del juego
/// </summary>
public class Bloque : MonoBehaviour
{

    #region Propiedades
    /// <summary>
    /// El prefab que instanciarán cuando el jugador golpee el bloque
    /// (puede ser null, en el caso de los bloques que no contienen nada)
    /// </summary>
    public Transform ItemPrefab;

    /// <summary>
    /// Booleano que indica si el bloque es destructible(ladrillos sin objeto) o no (bloques interrogación o de ladrillo con objetos)
    /// </summary>
    public bool isBreakable;
    #endregion

    #region Atributos privados
    /// <summary>
    /// Referencia al componente Animator de este GameObject
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Campo privado en el que guardamos si el bloque ha sido usado
    /// </summary>
    private bool isUsed = false;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //comprobamos que el bloque no haya sido usado ya y que la colisión es con el jugador
        if(!isUsed && collision.gameObject.CompareTag("Player"))
        {
            //iniciamos la animación del golpe
            animator.SetTrigger("hit");

            //si el bloque no es destructible...
            if (!isBreakable)
            {
                //...entonces lo marcamos como usado y cambiamos el sprite a usado
                animator.SetBool("used", true);
                isUsed = true;
            }
            else //en caso contrario...
            {
                //si el jugador está en estado "Super", indicamos al Animator que deberá destruirlo
                if (collision.gameObject.GetComponent<CharacterController>().IsBig) animator.SetBool("break", true);

            }

            //si el bloque contenía un objeto
            if (ItemPrefab != null)
            {
                //calculamos la posición en la que crearemos el objeto
                var sprite_extent = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
                Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + sprite_extent, transform.position.z);

                //lo instanciamos
                Instantiate(ItemPrefab, newPosition, Quaternion.identity);

                if (ItemPrefab.gameObject.CompareTag("Coin"))
                {
                    //si se trata de una moneda, aumentamos las monedas del jugador
                    collision.gameObject.GetComponent<CharacterController>().AddCoin();
                }
            }
        }
    }
}