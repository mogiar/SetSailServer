using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private Rigidbody _rb;
    private float _waveSinShipRot;
    [SerializeField] private float _upDownAmptittude;
    [SerializeField] private float _upDownSpeed;
    [SerializeField] private float _shipDanglePower;
    [SerializeField] private float _shipDangleSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _rb = gameObject.GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {        
        _waveSinShipRot = Mathf.Sin(Time.time * _shipDangleSpeed);
        float _waveSinUpdown = Mathf.Sin(Time.time * _upDownSpeed);
        //RotateShip(_rb.velocity);
        gameObject.transform.localPosition = new Vector3(0, _waveSinUpdown * _upDownAmptittude * _rb.velocity.magnitude, 0);
    }

    private void RotateShip(Vector3 veclocity)
    {
        gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y, _waveSinShipRot * (_shipDanglePower * veclocity.magnitude));
    }
}
