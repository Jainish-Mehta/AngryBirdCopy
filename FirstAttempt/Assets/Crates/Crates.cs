using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private GameObject _crateBreakEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If hit by a FlyingBird
        if (collision.collider.GetComponent<FlyingBird>() != null)
        {
            FlyingBird bird = collision.collider.GetComponent<FlyingBird>();
            if (bird != null)
            {
                Instantiate(_crateBreakEffect, transform.position, Quaternion.identity);
                Debug.Log("Crate hit by bird!");
                Destroy(gameObject); // Destroy the crate
                return;
            }
        }

        // If hit by another crate, do nothing
        Crate otherCrate = collision.collider.GetComponent<Crate>();
        if (otherCrate != null)
        {
            return;
        }

        // If hit from above (e.g., player jumps on it)
      /*  if (collision.contacts[0].normal.y < 0.5)
        {
            Instantiate(_crateBreakEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }*/
    }
}
