using UnityEngine;

/// <summary>
/// Keeps the weather VFX anchor positioned above the player so effects like
/// clouds and rain always surround the camera.
/// </summary>
public class WeatherAnchorFollow : MonoBehaviour
{
    #region Serialized Fields

    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [Tooltip("How far above the player the anchor is placed.")]
    [SerializeField] private float heightOffset = 50f;

    #endregion

    #region Unity Lifecycle

    private void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x,
            player.position.y + heightOffset,
            player.position.z
        );
    }

    #endregion
}
