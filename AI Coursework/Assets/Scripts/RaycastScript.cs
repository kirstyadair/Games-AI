using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider.gameObject.GetComponent<Exit>() != null)
            {
                if (hit.collider.gameObject.GetComponent<Exit>().isBlocked)
                {
                    hit.collider.gameObject.GetComponent<Exit>().isBlocked = false;
                }
                else hit.collider.gameObject.GetComponent<Exit>().isBlocked = true;
            }
        }
    }
}
