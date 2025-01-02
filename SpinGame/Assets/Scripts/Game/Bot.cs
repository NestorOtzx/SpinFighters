using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public enum BotMode{
    Static,
    Attack
}

public class Bot : ControllableCharacter
{
    public BotMode mode;
    const float err = 0.1f;
    bool isFacingEnemy = true;

    [SerializeField] float isPointingTolerance = 0.6f;

    int currentEnemyId = -1;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        info = GetComponent<PlayerInfo>();
        StartCoroutine(InitEnemy());
    }

    IEnumerator InitEnemy()
    {
        yield return new WaitForSeconds(0.1f);
        
        while(gameObject && gameObject.activeSelf)
        {
            SetNewEnemyId();
            yield return new WaitForSeconds(Random.Range(5, 12));
        }
    }

    private void SetNewEnemyId()
    {
        Debug.Log(gameObject.name +" Set new enemy ID");
        List<ulong> l = GameManager.instance.remainingPlayersSingle;
        if (l.Count > 0)
        {
            int newEnemyIndex = Random.Range(0, l.Count);
            int newID = (int) l[newEnemyIndex];
            if (l[newEnemyIndex] == info.playerID)
            {
                if (newEnemyIndex -1 >= 0)
                {
                    newEnemyIndex--;
                }else{
                    newEnemyIndex++;
                }
                
                if (newEnemyIndex > -1 && newEnemyIndex < l.Count)
                {
                    newID = (int) l[newEnemyIndex];
                }else{
                    newID = -1;
                }
            }
            currentEnemyId = newID;
        }else{
            currentEnemyId = -1;
        }
    }

    private void Update()
    {
        if (mode==BotMode.Attack)
        {
            if (currentEnemyId > -1)
            {
                List<ulong> remainingPlayers = GameManager.instance.remainingPlayersSingle;
                if (remainingPlayers.Contains((ulong)currentEnemyId))
                {
                    isFacingEnemy = FacingObjective(GameManager.instance.players[(ulong)currentEnemyId].gameObject.transform);
                    BotBehaviour();
                }else{
                    SetNewEnemyId();
                }
            }
        }
    }

    private void BotBehaviour()
    {
        if (isGroundedSingle)
        {
            if (!isCharging)
            {
                StartCharging();
            }
            else if (currentJumpForce < maxJumpForce-err){
                ChargeJumpForce();
            }else if (isFacingEnemy){
                StartCoroutine(RandomJump());
            }
        }
    }

    private IEnumerator RandomJump()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.4f));
        ReleaseJumpForce();
    }

    private bool FacingObjective(Transform objective)
    {
        Vector3 directionToEnemy = (objective.position - transform.position).normalized;
        directionToEnemy.y = 0;
        Vector3 currentDir = headTransform.forward;
        currentDir.y = 0;
        
        float dotProduct = Vector3.Dot(currentDir, directionToEnemy);

        return 1-dotProduct < isPointingTolerance;
    }
}
