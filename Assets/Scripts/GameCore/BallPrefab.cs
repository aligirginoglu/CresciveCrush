using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPrefab : MonoBehaviour
{
    public BallType type;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball" || collision.gameObject.tag == "Plane")
        {
            AudioManager.Instance.BallDropSound().Play();
        }
    }
}
