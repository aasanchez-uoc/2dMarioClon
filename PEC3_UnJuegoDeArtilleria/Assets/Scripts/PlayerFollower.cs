using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform Player;

    private float offset;

    private void Start()
    {
        offset = Player.transform.position.x - transform.position.x;
    }

    private void Update()
    {
        transform.position = new Vector3(Player.position.x - offset, transform.position.y, transform.position.z);
    }

}
