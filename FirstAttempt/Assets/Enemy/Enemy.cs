using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _cloudParticlePrefab;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.GetComponent<FlyingBird>() != null)
        {
            FlyingBird bird = collision.collider.GetComponent<FlyingBird>();
            if (bird != null)
            {
                Instantiate(_cloudParticlePrefab,transform.position, Quaternion.identity);
                // Handle the collision with the bird
                Debug.Log("Enemy hit by bird!");
                // You can add more logic here, like destroying the bird or playing a sound
                Destroy(gameObject); // Example: destroy the bird on collision
                return;
            }
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
