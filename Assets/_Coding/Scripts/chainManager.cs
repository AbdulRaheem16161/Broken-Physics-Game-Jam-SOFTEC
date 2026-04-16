using UnityEngine;

public class chainManager : MonoBehaviour
{
    [Header("Reference")]
    public Transform target;

    private void Update()
    {
        if (!target) return;

        target.position = transform.position;
    }
}