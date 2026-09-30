using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipProceduralAnimation : MonoBehaviour
{
    private float _waveSinShipRot;
    [SerializeField] private float _upDownAmptittude;
    [SerializeField] private float _upDownSpeed;
    [SerializeField] private float _shipDanglePowerZ;
    private float _shipDanglePowerX;
    [SerializeField] private float _shipDangleSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        _waveSinShipRot = Mathf.Sin(Time.time * _shipDangleSpeed);
        float _waveSinUpdown = Mathf.Sin(Time.time * _upDownSpeed);
        RotateShip(Vector3.one);
        transform.position = new Vector3(transform.position.x, _waveSinUpdown * _upDownAmptittude, transform.position.z);
    }

    private void RotateShip(Vector3 veclocity)
    {
        _waveSinShipRot = Mathf.Sin(Time.time * _shipDanglePowerZ);
        _shipDanglePowerX = _shipDanglePowerZ * -1;


        gameObject.transform.localRotation = Quaternion.Euler(_waveSinShipRot * (_shipDanglePowerX * veclocity.magnitude),
                                                              gameObject.transform.rotation.y,
                                                              _waveSinShipRot * (_shipDanglePowerZ * veclocity.magnitude));
    }
}
