using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallVignette : MonoBehaviour
{
    private Player_Move player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = FindObjectOfType<Player_Move>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
         if (player.useBall)
         {
            spriteRenderer.enabled = true;
            Vector3 newPosition = player.transform.position;
            newPosition.z += 0.5f;
            transform.position = newPosition;
           /* if (!gameObject.activeSelf)
             {
                 gameObject.SetActive(true);
             }*/
         }

        else
        {
            spriteRenderer.enabled = false;
            /*  if (gameObject.activeSelf)
              {
                  gameObject.SetActive(false);
              }*/
        }

    }
}
