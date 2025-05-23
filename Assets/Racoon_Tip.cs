using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Racoon_Tip : MonoBehaviour
{
    public GameObject tip_prefab;
    public String tip_to_say;
    private GameObject tip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tip = Instantiate(tip_prefab, transform.position, Quaternion.identity);
            tip.GetComponent<TextMeshPro>().text = tip_to_say;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (tip!=null)
            {
                Destroy(tip);
            }
        }
    }
}
