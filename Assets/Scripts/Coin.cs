using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip coinSfx;
    [SerializeField] int coinValue = 100;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        FindObjectOfType<GameSession>().AddToScore(coinValue);
        AudioSource.PlayClipAtPoint(coinSfx, Camera.main.transform.position);
        Destroy(gameObject);
    }
}
