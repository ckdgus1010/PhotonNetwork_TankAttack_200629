using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public int actNumber;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
