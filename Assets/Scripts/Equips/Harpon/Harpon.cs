using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

namespace Assets.Scripts.Equips.Harpon
{
    public class Harpon : EquipScript
    {
        private Vector3 TargetStartingPosition;
        [SerializeField] private float StartingCoolDown;
        private float CoolDown;

        //Effect
        [Header ("Effect")]
        [SerializeField] private Transform HarponOBJ;
        [SerializeField] private float RotationSpeed;
        [SerializeField] private Transform LookAtPlayer;
        [SerializeField] private float HarponSpeed;
        [SerializeField] private float Height;
        [SerializeField] private float TargetHeight;
        [SerializeField] private Transform parentTransform;

        private void Start()
        {
            //Player = GeneralFunctions.Player.transform;
            TargetStartingPosition = transform.localPosition;
        }

        private void Update()
        {
            if (HarponOBJ.gameObject.activeSelf == true)
            {
                HarponOBJ.Rotate(Vector3.up * RotationSpeed, Space.Self);
                HarponOBJ.localPosition = Vector3.LerpUnclamped(HarponOBJ.localPosition, new Vector3(0, TargetHeight - 10, 0), Time.deltaTime * HarponSpeed);

                if (HarponOBJ.transform.localPosition.y < TargetHeight)
                {                    
                    HarponOBJ.gameObject.SetActive(false);
                }
            }
            //if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject())
            //{
            //    Debug.Log("touch water 11");
            //    foreach (Touch touch in Input.touches)
            //    {
            //        Ray ray = Camera.main.ScreenPointToRay(touch.position);
            //        RaycastHit hit;
            //        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 4))
            //        {
            //            Debug.Log("touch water");
            //            transform.SetParent(null);
            //            transform.position = hit.point;
            //        }
            //    }
            //}

        }

        //[ServerRpc]
        override public void UseAbilltyServerRp()
        {
            if (CoolDown <= 0)
            {
                ShotHarponClientRpc();
            }
            
        }
        [ClientRpc]
        private void ShotHarponClientRpc()
        {
            Debug.Log("ShootHarpon");
            LookAtPlayer.transform.LookAt(GeneralFunctions.Player.transform);
            transform.parent = null;
            HarponOBJ.gameObject.SetActive(true);
            HarponOBJ.localPosition = new Vector3(0, Height, 0);
            StartCoroutine("StartCooldown");
        }

        IEnumerator StartCooldown()
        {
            CoolDown = StartingCoolDown;
            yield return new WaitForSeconds(CoolDown/2); // ----- test
            GeneralFunctions.Pooler.SpawnFromPool("Splash", new Vector2(transform.position.x, transform.position.z));
            yield return new WaitForSeconds(CoolDown/2);
            CoolDown = 0;
            transform.SetParent(parentTransform);
            transform.localPosition = TargetStartingPosition;
        }
    }
}