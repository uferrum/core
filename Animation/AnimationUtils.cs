using System.Collections;
using UnityEngine;

namespace Ferrum.Animation
{

public class AnimationUtils 
{
    public static void PlayIfPossible(GameObject gameObject, string state)
    {
        Animator anim = gameObject.GetComponent<Animator>();
        if (anim == null) return;

        try
        {
            anim.Play(state);
        } catch
        {

        }
    }
}

    public static class AnimatiorExtensionsAU
    {
        public static IEnumerator WaitForIdle(this Animator anim)
        {
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }
    }

}
