using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float totalTime = 1f;
    private float timeRemaining = 1f;
    [Tooltip("Whether the timer should use scaled or unscaled time. Unscaled time is unaffected by slowmotion.")]
    public bool ScaledTime = true;
    public bool DisplayTimeInObjectName = false;

    public bool RestartOnEnable = true;

    private string OriginalName;
    public void OnEnable()
    {
        if (RestartOnEnable)
        {
            Restart();
        }
    }

    void Update()
    {
        if (ScaledTime) timeRemaining -= Time.deltaTime;
        else timeRemaining -= Time.unscaledDeltaTime;
        if (DisplayTimeInObjectName) 
            name = OriginalName + " (" + timeRemaining.ToString(StringFormats.TwoDecimals) + ")";

        if (timeRemaining <= 0f) OnTimeOver();
    }

    public UnityEvent OnStart;
    public UnityEvent OnEnd;

    public virtual void OnTimeOver()
    {
        //Debug.Log("Timer ran out for " + name, this);
        name = OriginalName;
        OnEnd.Invoke();
    }

    public void Restart()
    {
        if (string.IsNullOrEmpty(OriginalName))
            OriginalName = name;
        timeRemaining = totalTime;
        OnStart.Invoke();
    }

    public void Restart(float _time)
    {
        totalTime = _time;
        Restart();
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class Indicator
    {
        public bool enabled;
        public float IndicatorWidth = .3f;
        public float IndicatorHeight = 1f;
        public Color IndicatorColor = Color.white;
        public enum Direction
        {
            Up,
            Right,
            Forward
        }
        public Direction direction = Direction.Up;
        public bool invertDirection;

        public void Draw(Transform transform, float timeRemaining, float totalTime)
        {
            Gizmos.color = IndicatorColor;
            Vector3 dir = new Vector3();
            Vector3 dirPerp = new Vector3();
            switch (direction)
            {
                case Direction.Up:
                    dir = transform.up;
                    dirPerp = transform.right;
                    break;
                case Direction.Right:
                    dir = transform.right;
                    dirPerp = transform.up;
                    break;
                case Direction.Forward:
                    dir = transform.forward;
                    dirPerp = transform.right;
                    break;
            }

            dir *= IndicatorHeight;
            if (invertDirection) dir *= -1f;

            Gizmos.DrawLine(transform.position, transform.position + dir * (timeRemaining / totalTime));
            Gizmos.DrawLine(
                transform.position + dirPerp * -IndicatorWidth,
                transform.position + dirPerp * IndicatorWidth);
            Gizmos.DrawLine(
                transform.position + dirPerp * -IndicatorWidth + dir,
                transform.position + dirPerp * IndicatorWidth + dir);
        }
    }
    public Indicator indicator = new Indicator();
    private void OnDrawGizmosSelected()
    {
        indicator.Draw(transform, timeRemaining, totalTime);
    }
#endif
}
