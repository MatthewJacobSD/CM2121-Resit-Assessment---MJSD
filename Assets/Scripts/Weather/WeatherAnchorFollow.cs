using UnityEngine;

public class WeatherAnchorFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float heightOffset = 50f;

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x,
            player.position.y + heightOffset,
            player.position.z
        );
    }
}