using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EffectConstants
{
    public const float totalEffectDuration = 0.5f;
    public const float minEffectDuration = 0.15f;
    public const int baseSortingOrder = 200;

    public const float forwardTimeRate = 0.15f;
    public const float backwardTimeRate = 0.85f;

    public const float deadAlpha = 0.1f;
    public const float aliveAlpha = 1f;


}

// ==========================
// 统一的动画请求基类
// ==========================
public abstract class AnimationRequest
{
    public Action OnComplete;
    public abstract IEnumerator Execute(EffectManager manager);
}


public class DamageAnimationRequest : AnimationRequest
{
    public BattleCharacterValue Target { get; }
    public BattleCharacterValue Attacker { get; }
    public Skill Skill { get; }
    public List<DamageResult> Results { get; }

    public DamageAnimationRequest(BattleCharacterValue target, BattleCharacterValue attacker, Skill skill, List<DamageResult> results)
    {
        Target = target;
        Attacker = attacker;
        Skill = skill;
        Results = results;
    }

    public override IEnumerator Execute(EffectManager manager)
    {

        if (Results == null || Results.Count == 0)
        {
            Debug.Log($"[DamageAnimationRequest] No results, exiting");
            yield break;
        }

        BattleCharacterInfo characterInfo = Target.isEnemy
            ? manager.enemyBattleCharacter
            : manager.playerBattleCharacter;

        bool shouldWaitForAll = Target == characterInfo.GetBattleCharacterValue();
        if (shouldWaitForAll)
        {
            for (int i = 0; i < Results.Count; i++)
            {
                var result = Results[i];
                bool finished = false;

                characterInfo.PlayHitReaction(result, Attacker, Skill, () => {
                    finished = true;
                });

                yield return new WaitUntil(() => finished);
            }
        }
        else
        {
            foreach (var result in Results)
            {
                Target.positionAtBattle.PlayHitReaction(result);
            }
        }

    }
}


public class DamageAnimationBatchRequest : AnimationRequest
{
    public List<DamageAnimationRequest> DamageRequests = new List<DamageAnimationRequest>();

    public override IEnumerator Execute(EffectManager manager)
    {

        BattleCharacterValue frontTarget = manager.enemyBattleCharacter.GetBattleCharacterValue();
        BattleCharacterValue selfTarget = manager.playerBattleCharacter.GetBattleCharacterValue();

        DamageAnimationRequest priorityRequest = DamageRequests
            .FirstOrDefault(r =>
            {
                var characterInfo = r.Target.isEnemy
                    ? manager.enemyBattleCharacter
                    : manager.playerBattleCharacter;
                return r.Target == characterInfo.GetBattleCharacterValue();
            });

        if (priorityRequest != null)
        {
            yield return manager.StartCoroutine(priorityRequest.Execute(manager));

        }
        


        int otherCount = 0;
        foreach (var req in DamageRequests)
        {
            if (req != priorityRequest)
            {
                otherCount++;

                // ✅ 不要 yield return！直接启动
                manager.StartCoroutine(req.Execute(manager));
            }
        }

    }

}

public class MultiValueChangeAnimationRequest : AnimationRequest
{
    public List<(BattleCharacterValue Target, List<(BattleValue valueIndex, int delta)> Values)> TargetValues;

    public override IEnumerator Execute(EffectManager manager)
    {
        if (TargetValues == null || TargetValues.Count == 0)
        {
            yield break;
        }

        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (var (target, values) in TargetValues)
        {
            if (target == null || values == null || values.Count == 0) continue;

            coroutines.Add(manager.StartCoroutine(target.positionAtBattle.ValueChangeAnimation(values)));
            coroutines.Add(manager.StartCoroutine(manager.playerBattleCharacter.ValueChangeAnimation(target, values)));
            coroutines.Add(manager.StartCoroutine(manager.enemyBattleCharacter.ValueChangeAnimation(target, values)));
        }

        foreach (var co in coroutines)
            yield return co;

    }
}

public class HealAnimationRequest : AnimationRequest
{
    public BattleCharacterValue Target { get; }
    public Skill Skill { get; }
    public List<int> Results { get; }

    public HealAnimationRequest(BattleCharacterValue target, Skill skill, List<int> results)
    {
        Target = target;
        Skill = skill;
        Results = results;
    }

    public override IEnumerator Execute(EffectManager manager)
    {
        if (Results == null || Results.Count == 0) yield break;

        BattleCharacterInfo characterInfo = Target.isEnemy
            ? manager.enemyBattleCharacter
            : manager.playerBattleCharacter;

        bool shouldWaitForAll = Target == characterInfo.GetBattleCharacterValue();
        if (shouldWaitForAll)
        {
            for (int i = 0; i < Results.Count; i++)
            {
                var result = Results[i];
                bool finished = false;

                characterInfo.PlayHealReaction(result, Skill, () => { finished = true; });

                yield return new WaitUntil(() => finished);
            }
        }
        else
        {
            foreach (var result in Results)
            {
                Target.positionAtBattle.PlayHealReaction(result);
            }
        }
    }
}

public class HealAnimationBatchRequest : AnimationRequest
{
    public List<HealAnimationRequest> HealRequests = new List<HealAnimationRequest>();

    public override IEnumerator Execute(EffectManager manager)
    {
        if (HealRequests == null || HealRequests.Count == 0) yield break;

        HealAnimationRequest priorityRequest = HealRequests
            .FirstOrDefault(r =>
            {
                var characterInfo = r.Target.isEnemy
                    ? manager.enemyBattleCharacter
                    : manager.playerBattleCharacter;
                return r.Target == characterInfo.GetBattleCharacterValue();
            });

        if (priorityRequest != null)
        {
            yield return manager.StartCoroutine(priorityRequest.Execute(manager));
        }

        foreach (var req in HealRequests)
        {
            if (req != priorityRequest)
            {
                manager.StartCoroutine(req.Execute(manager));
            }
        }
    }
}





public class CustomAnimationRequest : AnimationRequest
{
    public Func<IEnumerator> AnimationCoroutine;

    public override IEnumerator Execute(EffectManager manager)
    {
        if (AnimationCoroutine != null)
            yield return manager.StartCoroutine(AnimationCoroutine());

    }
}

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    public EffectControl playerCriticalHitEffect;
    public EffectControl enemyCriticalHitEffect;

    public BattleCharacterInfo playerBattleCharacter;
    public BattleCharacterInfo enemyBattleCharacter;

    private bool isPlaying = false;
    private Queue<AnimationRequest> animationQueue = new Queue<AnimationRequest>();
    private Coroutine currentCoroutine;

    private void Awake() => instance = this;

    // ========================== 队列播放系统 ==========================
    public void PlayAnimation(AnimationRequest request, bool useQueue = true)
    {
        if (useQueue)
        {
            animationQueue.Enqueue(request);
            if (!isPlaying)
                currentCoroutine = StartCoroutine(ProcessQueue());
        }
        else
        {
            StartCoroutine(PlaySingle(request));
        }
    }

    private IEnumerator ProcessQueue()
    {
        isPlaying = true;

        while (animationQueue.Count > 0)
        {
            var request = animationQueue.Dequeue();

            // 先执行动画逻辑
            yield return StartCoroutine(request.Execute(this));

            // Execute 完成后才调用 OnComplete
            request.OnComplete?.Invoke();
        }

        isPlaying = false;
    }

    private IEnumerator PlaySingle(AnimationRequest request)
    {
        bool finished = false;
        request.OnComplete += () => finished = true;
        yield return StartCoroutine(request.Execute(this));
        yield return new WaitUntil(() => finished);
    }

    #region Heal Animation

    public void PlayHealBatches(List<HealAnimationBatchRequest> batches, Action onComplete)
    {
        StartCoroutine(PlayHealBatchesRoutine(batches, onComplete));
    }

    private IEnumerator PlayHealBatchesRoutine(List<HealAnimationBatchRequest> batches, Action onComplete)
    {
        foreach (var batch in batches)
        {
            yield return StartCoroutine(batch.Execute(this));
        }
        onComplete?.Invoke();
    }


    #endregion

    #region Damage Animation

    // ========================== 伤害动画 ==========================

    public void PlayDamageBatches(List<DamageAnimationBatchRequest> batches, Action onComplete = null)
    {
        if (batches == null || batches.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        var wrapperRequest = new CustomAnimationRequest
        {
            AnimationCoroutine = () => ExecuteBatchesSequentially(batches),
            OnComplete = onComplete
        };

        PlayAnimation(wrapperRequest);
    }

    private IEnumerator ExecuteBatchesSequentially(List<DamageAnimationBatchRequest> batches)
    {
        DamageAnimationBatchRequest priorityBatch = null;

        foreach (var batch in batches)
        {
            // 检查这个 batch 中是否有主角色请求
            var hasPriority = batch.DamageRequests.Any(req =>
            {
                var characterInfo = req.Target.isEnemy
                    ? enemyBattleCharacter
                    : playerBattleCharacter;
                return req.Target == characterInfo.GetBattleCharacterValue();
            });

            if (hasPriority)
            {
                priorityBatch = batch;
                break;
            }
        }

        if (priorityBatch != null)
        {
            yield return StartCoroutine(priorityBatch.Execute(this));
        }


        int otherCount = 0;
        foreach (var batch in batches)
        {
            if (batch != priorityBatch)
            {
                otherCount++;
                StartCoroutine(batch.Execute(this));
            }
        }
    }
    internal void StartCriticalOrBlockAnimationInternal(DamageAnimationRequest req, Action onComplete)
    {
        if (req.Results == null || req.Results.Count == 0) { onComplete?.Invoke(); return; }
        var result = req.Results[0];

        if (result.IsCritical)
        {
            var effect = req.Attacker.isEnemy ? enemyCriticalHitEffect : playerCriticalHitEffect;
            var blockEffect = req.Attacker.isEnemy ? playerCriticalHitEffect : enemyCriticalHitEffect;
            effect.ShowCriticalAnimation(req.Attacker, req.Target, req.Skill, blockEffect, result.IsBlock, onComplete);
        }
        else if (result.IsBlock)
        {
            var effect = req.Target.isEnemy ? enemyCriticalHitEffect : playerCriticalHitEffect;
            effect.ShowBlockAnimation(req.Target, effect, onComplete);
        }
        else
            onComplete?.Invoke();
    }
    #endregion

    #region ValueChange Animation
    // ========================== 属性动画 ==========================
    public void ValueChangeAnimation(List<BattleCharacterValue> targets, List<List<(BattleValue valueIndex, int delta)>> values, Action onComplete = null)
    {
        var targetPairs = new List<(BattleCharacterValue, List<(BattleValue, int)>)>();
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && values[i] != null && values[i].Count > 0)
                targetPairs.Add((targets[i], values[i]));
        }

        if (targetPairs.Count == 0) { onComplete?.Invoke(); return; }

        var request = new MultiValueChangeAnimationRequest
        {
            TargetValues = targetPairs,
            OnComplete = onComplete
        };

        PlayAnimation(request);
    }
    #endregion


    // ========================== 自定义动画 ==========================
    public void PlayCustomAnimation(Func<IEnumerator> coroutine, Action onComplete = null)
    {
        var request = new CustomAnimationRequest { AnimationCoroutine = coroutine, OnComplete = onComplete };
        PlayAnimation(request);
    }

    // ========================== 状态管理 ==========================
    public bool IsPlaying() => isPlaying;
    public void InterruptCurrentAnimation()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        isPlaying = false;
        animationQueue.Clear();
    }
}