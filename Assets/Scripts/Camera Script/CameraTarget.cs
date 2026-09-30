using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _distanceMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.localPosition = new Vector3(0, 0, _rb.velocity.magnitude * _distanceMultiplier);
    }
}
