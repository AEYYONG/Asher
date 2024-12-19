using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallVignette : MonoBehaviour
{
    private Player_Move player;

    void Start()
    {
        player = FindObjectOfType<Player_Move>();
    }

    void Update()
    {
         if (player.useBall)
         {
            Vector3 newPosition = player.transform.position;
            newPosition.z += 0.5f;
            transform.position = newPosition;
            if (!gameObject.activeSelf)
             {
                 gameObject.SetActive(true);
             }
         }

        else
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
