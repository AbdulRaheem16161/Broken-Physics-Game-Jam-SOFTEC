using UnityEngine;

public class DreamFloat : MonoBehaviour
{
    #region Position Settings

    // How much the object floats up and down
    [SerializeField] private float floatHeight = 1.5f;

    // Speed of vertical floating motion
    [SerializeField] private float floatSpeed = 1f;

    // Random offset so multiple cubes don't sync perfectly
    [SerializeField] private float randomPhaseOffset = 10f;

    #endregion

    #region Rotation Settings

    // Rotation speed on each axis
    [SerializeField] private Vector3 rotationSpeed = new Vector3(5f, 10f, 3f);

    // Slight randomness in rotation per object
    [SerializeField] private float rotationRandomness = 1f;

    #endregion

    #region Runtime Values

    private Vector3 startPosition;
    private float noiseSeed;

    #endregion

    private void Start()
    {
        #region Initialize Float State

        startPosition = transform.position;
        noiseSeed = Random.Range(0f, 100f);

        // Add random rotation variation so cubes feel unique
        rotationSpeed += new Vector3(
            Random.Range(-rotationRandomness, rotationRandomness),
            Random.Range(-rotationRandomness, rotationRandomness),
            Random.Range(-rotationRandomness, rotationRandomness)
        );

        #endregion
    }

    private void Update()
    {
        #region Floating Motion

        float time = Time.time * floatSpeed + noiseSeed;

        // Smooth up-down motion (dreamy sine wave)
        float offsetY = Mathf.Sin(time) * floatHeight;

        // Add subtle Perlin noise wobble so it feels less “robotic”
        float noise = Mathf.PerlinNoise(noiseSeed, Time.time * 0.2f);
        float extraWobble = (noise - 0.5f) * 0.3f;

        transform.position = startPosition + new Vector3(0f, offsetY + extraWobble, 0f);

        #endregion

        #region Rotation Motion

        transform.Rotate(rotationSpeed * Time.deltaTime);

        #endregion
    }
}