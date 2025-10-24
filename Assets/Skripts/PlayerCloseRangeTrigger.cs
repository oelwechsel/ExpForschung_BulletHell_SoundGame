using UnityEngine;

public class PlayerCloseRangeTrigger : MonoBehaviour
{
    private string enemyTag;

    public void Initialize(string enemyTagName)
    {
        enemyTag = enemyTagName;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            if(LevelManager.Instance.isLevelActive && !PlayerLives.Instance.IsRespawning && !PlayerLives.Instance.isInvulnerable)
            PlayerController.Instance.EnemyEnteredRange();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            PlayerController.Instance.EnemyLeftRange();
        }
    }
}
