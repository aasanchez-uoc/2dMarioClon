using UnityEngine;

/// <summary>
/// Clase encargada de instanciar el power-up que corresponda (seta o flor) en función del estado del jugador
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    /// <summary>
    /// Prefab con el objeto de las setas
    /// </summary>
    public GameObject MushroomPrefab;

    /// <summary>
    /// Prefab con el objeto de las flores
    /// </summary>
    public GameObject FlowerPrefab;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //si el jugador es grande instanciamos la flor, en caso contrario instanciamoa la seta
        GameObject prefab = (player.GetComponent<CharacterController>().IsBig) ? FlowerPrefab : MushroomPrefab;
        Instantiate(prefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
