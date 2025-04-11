using UnityEngine;

public class TP_Box : MonoBehaviour
{
    [SerializeField] private Transform ExitPoint;
    [SerializeField] private TP_Box ExitBOX;
    bool isPlayerOnTop = false;
    private player_movement playerMovement;
    void Update()
    {
        isPlayerOnTop = CheckForPlayer();
        if (Input.GetKeyDown(KeyCode.S)&&isPlayerOnTop)
        {
            playerMovement.transform.position = ExitBOX.ExitPoint.transform.position;
        }
    }

    private bool CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 2);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player")
            {
                playerMovement = hit.collider.gameObject.GetComponent<player_movement>();
                return true;
            }
        }
        return false;
    }
}
