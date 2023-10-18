using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour 
    {
        public GameObject friendlyObj;
        public GameObject enemyObj;
        public Material enemyMaterial;
        public Material closestEnemyMaterial;
        public Transform enemyParent;
        public Transform friendlyParent;
        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();
        List<Soldier> closestEnemies = new List<Soldier>();
        float mapWidth = 50f;
        int cellSize = 10;
        int numberOfSoldiers = 100;
        public bool spatialPartition = false;
        Grid grid;
        public TextMeshProUGUI updateText;
        private float startTime;
        Toggle spToggle;
        void Start() 
        {
            //Create a new grid
            grid = new Grid((int)mapWidth, cellSize);
            
            //Add random enemies and friendly and store them in a list
            for (int i = 0; i < numberOfSoldiers; i++)
            {
                //Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                //Create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the enemy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                //Parent it
                newEnemy.transform.parent = enemyParent;


                //Give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                //Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                //Parent it 
                newFriendly.transform.parent = friendlyParent;
            }
        }
	
	
        void Update() 
        {
            //Move the enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                enemySoldiers[i].Move();
            }

            //Reset material of the closest enemies
            for (int i = 0; i < closestEnemies.Count; i++)
            {
                if (closestEnemies[i] != null && closestEnemies[i].soldierMeshRenderer != null)
                {
                    closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
                }
            }

            //Reset the list with closest enemies
            closestEnemies.Clear();

            //For each friendly, find the closest enemy and change its color and chase it
            for (int i = 0; i < friendlySoldiers.Count; i++)
            {

                //The fast version with spatial partition
                Soldier closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]);

                //If we found an enemy
                if (closestEnemy != null)
                {
                    //Change material
                    closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;

                    closestEnemies.Add(closestEnemy);

                    //Move the friendly in the direction of the enemy
                    friendlySoldiers[i].Move(closestEnemy);
                }
            }
            float endTime = Time.realtimeSinceStartup;
            float updateTime = endTime - startTime;
             Debug.Log("Update time: " + updateTime + " seconds");
            updateText.text = "Update time: " + updateTime + " seconds";
        }
        Soldier FindClosestEnemySlow(Soldier soldier)
        {
            Soldier closestEnemy = null;

            float bestDistSqr = Mathf.Infinity;

            //Loop thorugh all enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                //The distance sqr between the soldier and this enemy
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

                //If this distance is better than the previous best distance, then we have found an enemy that's closer
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestEnemy = enemySoldiers[i];
                }
            }

            return closestEnemy;
        }
        public void SPChange(Toggle change)
        {
            if (change.isOn)
                spatialPartition = true;
            else
                spatialPartition = false;
        }
        public void ButtonCount()
        {
            if(numberOfSoldiers != 100)
                numberOfSoldiers = 100;
            UpdateCount();
        }
        public void UpdateCount()
        {
            GameObject[] soldier = GameObject.FindGameObjectsWithTag("Soldier");
            foreach(GameObject child in soldier)
                Destroy(child);

            enemySoldiers.Clear();
            friendlySoldiers.Clear();
            Start();
        }
    }
}