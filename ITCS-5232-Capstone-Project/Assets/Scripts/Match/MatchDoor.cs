using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchDoor : MonoBehaviour
{
    public int index;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            TriggerDoor();
        }
    }

    public void TriggerDoor()
    {
        GameManager.instance.TriggerWave(index + 1);
        Destroy(gameObject);
    }
}
