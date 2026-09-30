using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<PlayerNetworkData> _playerData;
    [SerializeField] private bool _serverAuth;



    // Start is called before the first frame update
    void Awake()
    {

        var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _playerData = new NetworkVariable<PlayerNetworkData>(writePerm: permission);
    }
    public override void OnNetworkSpawn()
    {//2
        if (!IsOwner)
        {
            //Destroy(this);
        }
        else
        {
            gameObject.GetComponent<PlayerControls>()._rb = GetComponent<Rigidbody>();
            GeneralFunctions.Player = gameObject;
            GeneralFunctions.GameManager.Player = gameObject;
            Debug.Log("ani hai");
            gameObject.SetActive(false);
            //GeneralFunctions.GameManager.PlayerInsertGame();
            //GeneralFunctions.ControlMove.SetActive(true);
            //GeneralFunctions.GameManager.ControlMove.GetComponent<MovementControl>().NewPlayer();
            //GeneralFunctions.Camera.GetComponent<CameraScript>()._target = GeneralFunctions.Player.transform.GetChild(2);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            TransmitDate();
        }
        else
        {
            ReciveData();
        }
    }
    private void TransmitDate()
    {
        var data = new PlayerNetworkData()
        {
            Position = transform.position,
            Rotation = transform.rotation
        };

        if (IsServer || !_serverAuth)
        {
            _playerData.Value = data;
        }
        else
        {
            TransmitDataServerRpc(data);
        }
    }

    [ServerRpc]
    private void TransmitDataServerRpc(PlayerNetworkData data)
    {
        _playerData.Value = data;
    }

    private void ReciveData()
    {
        transform.position = _playerData.Value.Position / 100;
        transform.rotation = _playerData.Value.Rotation;
    }

    struct PlayerNetworkData : INetworkSerializable
    {
        private short _x, _y, _z;
        private short _xRot, _yRot, _zRot, _wRot;
        

        internal Vector3 Position
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                _x = (short)(value.x*100);
                _y = (short)(value.y * 100);
                _z = (short)(value.z * 100);
            }
        }
        internal Quaternion Rotation
        {
            get => new Quaternion((float)_xRot/100, (float)_yRot/100, (float)_zRot/100, (float)_wRot/100);
            set
            {
                _xRot = (short)(value.x * 100);
                _yRot = (short)(value.y * 100);
                _zRot = (short)(value.z * 100);
                _wRot = (short)(value.w * 100);
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _y);
            serializer.SerializeValue(ref _z);

            serializer.SerializeValue(ref _xRot);
            serializer.SerializeValue(ref _yRot);
            serializer.SerializeValue(ref _zRot);
            serializer.SerializeValue(ref _wRot);
        }
    }

    
}
