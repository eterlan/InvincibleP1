using System;
using FPS.Scripts.Game.Shared;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FPS.Scripts.Game.Managers
{
    public class PickupSpawner : MonoBehaviour
    {
        [Header("Limit")]
        public int maxSpawnAmountInLevel = 7;


        public float        radius = 15;
        [FormerlySerializedAs("weaponPrefabs")]
        public GameObject[] prefabs;
        public int          tryTime     = 100;
        public float        spawnHeight = 0.6f;
        public MinMaxFloat  spawnIntervalRandomRange;


        public Color debugColor = Color.red;


        private int             m_spawnAmountInLevel;
        private Collider[]      m_overlapped = new Collider[1];
        private GameFlowManager m_gameFlowManager;
        private float           m_lastSpawnTime = Mathf.NegativeInfinity;
        private float           m_spawnInterval;
        
        private void Start()
        {
            m_gameFlowManager = FindObjectOfType<GameFlowManager>();
            EventManager.AddListener<PickupEvent>(OnPickup);
        }

        private void OnPickup(PickupEvent go)
        {
            if (go.Pickup.TryGetComponent<WeaponPickup>(out _))
            {
                m_spawnAmountInLevel--;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void Update()
        {
            if (m_gameFlowManager.GameIsEnding)
            {
                return;
            }

            if (Time.time - m_lastSpawnTime < m_spawnInterval)
            {
                return;
            }

            m_lastSpawnTime = Time.time;
            m_spawnInterval = Random.Range(spawnIntervalRandomRange.Min, spawnIntervalRandomRange.Max);
            Spawn();
        }

        public void Spawn()
        {
            if (m_spawnAmountInLevel > maxSpawnAmountInLevel)
                return;
            
            var pivot      = transform.position;
            var extent     = new Vector3(0.5f, 0.1f, 0.5f);

            var spawnPoint = Vector3.zero;
            for (var i = 0; i < tryTime; i++)
            {
                var localPoint   = Random.insideUnitCircle * radius;
                var point        = new Vector3(localPoint.x + pivot.x, spawnHeight + transform.position.y, localPoint.y + pivot.z);
                
                var overlapCount = Physics.OverlapBoxNonAlloc(point, extent, m_overlapped);
                if (overlapCount == 0)
                {
                    //Debug.Log($"Iterate {i} times to find spawn point");
                    spawnPoint = point; 
                    break;
                }
            }

            if (spawnPoint == Vector3.zero)
            {
                //Debug.Log("Cannot find spawn point");
                return;
            }

            m_spawnAmountInLevel++;
            var weaponPrefab = prefabs[Random.Range(0, prefabs.Length)].gameObject;
            Instantiate(weaponPrefab, spawnPoint, Quaternion.identity, transform);
        }
    }
}