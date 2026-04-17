// using UnityEngine;
// using System.Collections.Generic;

// public class PowerUp_EnemyScaleIncreaser : MonoBehaviour, IPowerUp
// {
//     #region References

//     [SerializeField] private string displayName = "Giant Enemies";

//     public string DisplayName => displayName;

//     #endregion

//     #region Settings

//     [Header("Behavior")]
//     public bool dontResetSize = false;

//     [Header("Position Adjustment")]
//     [SerializeField] private float yOffset = 2f;

//     #endregion

//     #region Runtime

//     public List<SizeMultiplierHelper> affectedEnemies = new List<SizeMultiplierHelper>();
//     private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

//     #endregion

//     #region IPowerUp

//     public void ActivatePowerUp()
//     {
//         #region Activate

//         if (scoreManager == null)
//         {
//             Debug.LogWarning("[EnemyScaleIncreaser] ScoreManager missing!");
//             return;
//         }

//         affectedEnemies.Clear();
//         originalPositions.Clear();

//         foreach (GameObject enemy in scoreManager.enemies)
//         {
//             if (enemy == null) continue;

//             SizeMultiplierHelper helper = enemy.GetComponent<SizeMultiplierHelper>();

//             if (helper != null)
//             {
//                 // Store original position
//                 Transform t = enemy.transform;

//                 if (!originalPositions.ContainsKey(t))
//                 {
//                     originalPositions.Add(t, t.position);
//                 }

//                 // Apply Y offset
//                 t.position += new Vector3(0f, yOffset, 0f);

//                 // Activate scaling
//                 helper.increaseSize = true;

//                 affectedEnemies.Add(helper);
//             }
//         }

//         Debug.Log("[EnemyScaleIncreaser] ACTIVATED + Y Offset Applied");

//         #endregion
//     }

//     public void DeactivatePowerUp()
//     {
//         #region Deactivate

//         if (dontResetSize)
//         {
//             Debug.Log("[EnemyScaleIncreaser] Deactivate skipped (dontResetSize ON)");
//             return;
//         }

//         foreach (var helper in affectedEnemies)
//         {
//             if (helper != null)
//             {
//                 helper.increaseSize = false;
//             }
//         }

//         foreach (var pair in originalPositions)
//         {
//             if (pair.Key != null)
//             {
//                 pair.Key.position = pair.Value;
//             }
//         }

//         Debug.Log("[EnemyScaleIncreaser] DEACTIVATED + Positions Restored");

//         #endregion
//     }

//     #endregion
// }