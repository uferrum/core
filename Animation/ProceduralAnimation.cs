using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ferrum.Animation
{
    public class ProceduralAnimatonState
    {
        public GameObject target = null;

        public virtual string Name { get; set; } = "Temporary State";
        public virtual float End { get; set; } = 1f;

        public virtual float Progress { get; set; } = 0f;
    
        public float ProgressDelta { 
            get {
                return Progress / End;
            }
        }

        public bool Started = false;

        public IEnumerator Wait()
        {
            yield return null;
            while (true)
            {
                yield return null;
                if(Progress >= End)
                {
                    break;
                }
            }
        }

        public virtual Func<bool> BeforeStart { get; set; } = () =>
        {
            return true;
        };
        
        public virtual Func<bool> ExecuteTick { get; set; } = () =>
        {
            return true;
        };

        public static ProceduralAnimatonState Create(Func<bool> Execute, float End = 1f, GameObject target = null, Func<bool> BeforeStart = null)
        {
            return new ProceduralAnimatonState { target = target, ExecuteTick = Execute, End = End, BeforeStart = BeforeStart };
        }
    }
    
    public class ProceduralAnimator: MonoBehaviour
    {
        #region "Samples"

        public static ProceduralAnimatonState MoveAlphaUI(float time = 1f, float from = 1f, float to = 0f)
        {
            ProceduralAnimatonState state = new() { End = time };

            state.ExecuteTick = () =>
            {
                if (state.target != null)
                {
                    if(state.target.TryGetComponent(out CanvasGroup group)) group.alpha = state.ProgressDelta;
                    if(state.target.TryGetComponent(out RawImage img)) img.color = new Color (img.color.r, img.color.g, img.color.b, Mathf.Lerp(from, to, state.ProgressDelta));
                    Debug.Log(Mathf.Lerp(from, to, state.ProgressDelta));
                }
                return true;
            };

            return state;
        }

        #endregion

        public static ProceduralAnimator Of(GameObject obj)
        {
            if (!obj) return null;
            return obj.Compo<ProceduralAnimator>();
        }

        [NonSerialized]
        public List<ProceduralAnimatonState> ActiveStates = new();

        private void Update()
        {
            try
            {
                foreach (var state in ActiveStates)
                {
                    if (state.Progress >= state.End)
                    {
                        ActiveStates.Remove(state);
                        continue;
                    }

                    state.Progress += Time.deltaTime;
                    if (state.Started)
                    {
                        state.ExecuteTick?.Invoke();
                    }
                    else
                    {
                        if (state.BeforeStart != null && state.BeforeStart() == false)
                        {

                        }
                        else
                        {
                            state.Started = true;
                        }
                    }
                }
            }
            catch (Exception e)
            { Debug.LogError(e); }
        }

        public ProceduralAnimatonState Do(Func<bool> Execute, float End = 1f, Func<bool> BeforeStart = null)
        {
            return Link(ProceduralAnimatonState.Create(Execute, End, gameObject, BeforeStart));
        }

        public ProceduralAnimatonState Link(ProceduralAnimatonState state)
        {
            state.target = gameObject;
            ActiveStates.Add(state);
            return state;
        }
    }
}