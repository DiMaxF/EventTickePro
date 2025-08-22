using DG.Tweening;
using System;
using UnityEngine;

public interface IViewAnimation
{
    // <summary>
    /// Adds the show animation to the sequence.
    /// </summary>
    /// <param name="target">The target GameObject or Transform for the animation.</param>
    Tween AnimateShow();

    /// <summary>
    /// Adds the hide animation to the sequence.
    /// </summary>
    /// <param name="target">The target GameObject.</param>
    Tween AnimateHide();

    /// <summary>
    /// Execution order (lower = earlier). For sorting animations.
    /// </summary>
    int Order { get; } 

    /// <summary>
    /// Determines if the animation should join the previous one (true) or append (false).
    /// </summary>
    bool IsParallel { get; }
}