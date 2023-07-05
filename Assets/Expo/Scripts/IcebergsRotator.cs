using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcebergsRotator : MonoBehaviour
{
    [SerializeField] private float speedRotation = 0.4f;

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(transform.up * speedRotation * Time.deltaTime);
    }
}
