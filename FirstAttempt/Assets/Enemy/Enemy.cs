using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        FlyingBird bird = collision.collider.GetComponent<FlyingBird>();
        if (bird != null)
        {
            Instantiate(_cloudParticlePrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy hit by bird!");
            Destroy(gameObject);
            return;
        }

        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if(enemy != null)
        {
            return;
        }
        if(collision.contacts[0].normal.y<0.5)
        {
            Instantiate(_cloudParticlePrefab,transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }
    }
}
