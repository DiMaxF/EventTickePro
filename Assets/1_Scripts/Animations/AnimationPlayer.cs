using UnityEngine;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public static class AnimationPlayer
{
    /// <summary>
    /// Plays animations attached to the target GameObject.
    /// </summary>
    /// <param name="target">The GameObject with IViewAnimation components.</param>
    /// <param name="show">True for show animations, false for hide animations.</param>
    /// <returns>UniTask for async completion.</returns>
    public static async UniTask PlayAnimationsAsync(GameObject target, bool show = true, Action onComplete = null)
    {
        if (target == null)
        {
            Logger.LogWarning("AnimationPlayer: Target GameObject is null.", "AnimationPlayer");
            return;
        }

        var animations = target.GetComponents<IViewAnimation>()
            .OrderBy(a => a.Order)
            .GroupBy(a => a.Order) 
            .ToList();

        if (animations.Count == 0)
        {
            return; 
        }

        var controller = new DOTweenAnimationController();
        controller.StartAnimation();
        var sequence = controller.GetSequence();

        foreach (var group in animations)
        {
            bool isFirstInGroup = true;
            foreach (var anim in group)
            {
                try
                {
                    if (isFirstInGroup || !anim.IsParallel)
                    {
                        if (show)
                            sequence.Append(anim.AnimateShow(onComplete));
                        else
                            sequence.Append(anim.AnimateHide());
                        isFirstInGroup = false;
                    }
                    else
                    {
                        sequence.Join(show
                            ? anim.AnimateShow(null)
                            : anim.AnimateHide());
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.LogError($"AnimationPlayer: Error in {anim.GetType().Name} on {target.name}: {ex.Message}", "AnimationPlayer");
                }
            }
        }

        await sequence.AsyncWaitForCompletion();
        controller.StopAnimation();
    }


}