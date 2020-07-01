﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLibManager;

public class DeathMatchNetworkGameRule : IONetworkGameRule
{
    public int endMatchCountDown = 10;
    [Tooltip("Rewards for each ranking, sort from high to low (1 - 10)")]
    public MatchReward[] rewards;
    public int EndMatchCountingDown { get; protected set; }
    public override bool HasOptionBotCount { get { return true; } }
    public override bool HasOptionMatchTime { get { return true; } }
    public override bool HasOptionMatchKill { get { return true; } }
    public override bool HasOptionMatchScore { get { return false; } }
    public override bool ShowZeroScoreWhenDead { get { return false; } }
    public override bool ShowZeroKillCountWhenDead { get { return false; } }
    public override bool ShowZeroAssistCountWhenDead { get { return false; } }
    public override bool ShowZeroDieCountWhenDead { get { return false; } }
    
    protected override void EndMatch()
    {
        SetRewards((BaseNetworkGameCharacter.Local as CharacterEntity).rank);
        networkManager.StartCoroutine(EndMatchRoutine());
    }

    public void SetRewards(int rank)
    {
        MatchRewardHandler.SetRewards(rank, rewards);
    }

    IEnumerator EndMatchRoutine()
    {
        EndMatchCountingDown = endMatchCountDown;
        while (EndMatchCountingDown > 0)
        {
            yield return new WaitForSeconds(1);
            --EndMatchCountingDown;
        }
        networkManager.StopHost();
    }

    public override bool RespawnCharacter(BaseNetworkGameCharacter character, params object[] extraParams)
    {
        var targetCharacter = character as CharacterEntity;
        // In death match mode will not reset score, kill, assist, death
        targetCharacter.ResetItemAndStats();
        targetCharacter.watchAdsCount = 0;

        return true;
    }

    public override void InitialClientObjects(LiteNetLibClient client)
    {
        base.InitialClientObjects(client);
        var gameplayManager = FindObjectOfType<GameplayManager>();
        if (gameplayManager != null)
        {
            gameplayManager.killScore = 1;
            gameplayManager.suicideScore = 0;
        }
    }
}
