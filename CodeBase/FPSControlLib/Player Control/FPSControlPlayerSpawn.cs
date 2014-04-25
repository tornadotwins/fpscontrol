using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{    
    public class FPSControlPlayerSpawn : MonoBehaviour
    {
        public int spawnID = 0;
        public bool available = false;

        private static FPSControlPlayerSpawn[] _spawnPoints = null;

        void Awake()
        {
            if (_spawnPoints == null)
            {
                _spawnPoints = FindObjectsOfType(typeof(FPSControlPlayerSpawn)) as FPSControlPlayerSpawn[];
            }
        }

        void OnDestroy()
        {
            _spawnPoints = null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.3F);
            Vector3 forward = transform.rotation * new Vector3(0, 0, 1);
            Vector3 pos = transform.position;

            Gizmos.DrawRay(pos, forward);

            Vector3 right = Quaternion.Euler(0, 200, 0) * forward;
            Vector3 left = Quaternion.Euler(0, 160, 0) * forward;
            Gizmos.DrawRay(pos + forward, right * 2);
            Gizmos.DrawRay(pos + forward, left * 2);

        }

        public static FPSControlPlayerSpawn[] GetAllSpawnPoints() { return _spawnPoints; }

        public static void ResetSpawnPoints()
        {
            _spawnPoints = FindObjectsOfType(typeof(FPSControlPlayerSpawn)) as FPSControlPlayerSpawn[];
        }

        public static bool ReSpawn(FPSControlPlayer player)
        {

            FPSControlPlayerSpawn spawn = null;
            float minDist = -1f;
            float d = 0f;

            foreach (FPSControlPlayerSpawn sp in _spawnPoints)
            {
                if (sp.available)
                {
                    d = Vector3.Distance(player.transform.position, sp.transform.position);

                    if (minDist < 0 || d < minDist)
                    {
                        minDist = d;
                        spawn = sp;
                    }
                }
            }

            if (spawn != null)
            {
                player._OnSpawn(spawn);
                FPSControlPlayerEvents.Spawn();
                return true;
            }

            return false;
        }

        public static bool ReSpawnAt(FPSControlPlayer player, int ID)
        {

            FPSControlPlayerSpawn spawn = null;
            float minDist = -1f;
            float d = 0f;

            foreach (FPSControlPlayerSpawn sp in _spawnPoints)
            {
                if (sp.available && sp.spawnID == ID)
                {
                    d = Vector3.Distance(player.transform.position, sp.transform.position);

                    if (minDist < 0 || d < minDist)
                    {
                        minDist = d;
                        spawn = sp;
                    }
                }
            }

            if (spawn != null)
            {
                player._OnSpawn(spawn);
                FPSControlPlayerEvents.Spawn();
                return true;
            }

            return false;
        }
    }
}
