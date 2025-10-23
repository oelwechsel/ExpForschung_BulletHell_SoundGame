using UnityEngine;
using TMPro; 

public class PlayerLivesUI : MonoBehaviour
{
    public PlayerLives playerLives; 
    public TextMeshProUGUI livesText;

    private void Update()
    {
        livesText.text = playerLives.CurrentLives.ToString();
    }
}
