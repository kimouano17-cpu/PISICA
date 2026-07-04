using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class BattleRoomScript : NetworkBehaviour
{
    public GameObject backGate, frontGate;
    public List<GameObject> enemiesActive = new List<GameObject>();
    public List<GameObject> playersActive = new List<GameObject>();
    void Start()
    {
        backGate.GetComponent<StoneGateScript>().OpenGate();
        frontGate.GetComponent<StoneGateScript>().OpenGate();
    }


    void Update()
    {
        enemiesActive.RemoveAll(enemy => enemy == null);
        if (enemiesActive.Count == 0 && playersActive.Count != 0)
        {
            frontGate.GetComponent<StoneGateScript>().OpenGate();
       

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playersActive.Contains(other.gameObject))
        {
            playersActive.Add(other.gameObject);
            if (playersActive.Count == GameManager.Instance.maxPlayers)
            {

                backGate.GetComponent<StoneGateScript>().CloseGate();
                frontGate.GetComponent<StoneGateScript>().CloseGate();
            }
          

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !enemiesActive.Contains(other.gameObject))
        {
            var enemy = GetComponent<EnemyStateHandler>();
            enemiesActive.Add(other.gameObject);
            if (enemy.CurrentState == EnemyStateHandler.EnemyState.OnDeath)
            {
                  enemiesActive.Remove(other.gameObject);
            }
          

        }
      
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playersActive.Contains(other.gameObject))
        {
            playersActive.Remove(other.gameObject);
            if (playersActive.Count == 0)
            {
                
               frontGate.GetComponent<StoneGateScript>().CloseGate();
                Debug.Log("Front Gate Closed...");
                this.enabled = false;
            }


        }
    }
}

