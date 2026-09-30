using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] public Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _camSpeed;

    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        //_target = GeneralFunctions.Player.transform;
        startPos = transform.position;
        _offset = startPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_target != null)
        {
            transform.position = Vector3.Lerp(transform.position,
            _target.position + _offset,
            Time.deltaTime * _camSpeed);
        }
        

        //Debug.Log("local: " + transform.localPosition + "world: " + transform.position);
    }

    public void TelescopeButton(int mode)
    {
        if (mode == 0)
        {
            _offset = new Vector3(0, 50, -50);
        }
        else
        {
            _offset = startPos;
        }
    }
}
