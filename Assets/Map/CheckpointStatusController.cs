using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointStatusController : MonoBehaviour
{
    void SetRunes(bool status)
    {
        Transform runes = transform.GetChild(0);

        runes.gameObject.SetActive(status);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SetRunes(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SetRunes(false);
        }
    }
}
