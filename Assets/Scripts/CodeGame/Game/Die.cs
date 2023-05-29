using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Diamond"))
        {
            TheGameUI.Instance.ShowLose();
            return;
        }
    }
}
