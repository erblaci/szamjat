//https://www.youtube.com/watch?v=tBj-FWcIwYw
using UnityEngine;
using UnityEngine.SceneManagement;
public class Enemy_kill : MonoBehaviour
{
    public GameObject player;
    public Transform respawnPoint;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name); //Ha az enemybe belebotlik a játékos akkor a scene újraindul
            //player.transform.position = respawnPoint.position; //Ha ezt használod a felette lévő két sort kommentezd ki ez egy pontra rakja vissza a karaktert a scene újraindítása nélkül.
        }
    }
}