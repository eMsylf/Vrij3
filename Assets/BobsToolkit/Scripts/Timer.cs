using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float time = 1f;
    private float currentTime = 1f;
    [Tooltip("Whether the timer should use scaled or unscaled time. Unscaled time is unaffected by slowmotion.")]
    public bool ScaledTime = true;
    public bool DisplayTimeInObjectName = false;

    private string OriginalName;
    public void OnEnable()
    {
        Restart();
    }

    void Update()
    {
        if (ScaledTime) currentTime -= Time.deltaTime;
        else currentTime -= Time.unscaledDeltaTime;
        if (DisplayTimeInObjectName) 
            name = OriginalName + " (" + currentTime.ToString(StringFormats.TwoDecimals) + ")";

        if (currentTime <= 0f) OnTimeOver();
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
        currentTime = time;
        OnStart.Invoke();
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class Indicator
    {
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
    }
    public Indicator indicator;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = indicator.IndicatorColor;
        Vector3 dir = new Vector3();
        Vector3 dirPerp = new Vector3();
        switch (indicator.direction)
        {
            case Timer.Indicator.Direction.Up:
                dir = transform.up;
                dirPerp = transform.right;
                break;
            case Timer.Indicator.Direction.Right:
                dir = transform.right;
                dirPerp = transform.up;
                break;
            case Timer.Indicator.Direction.Forward:
                dir = transform.forward;
                dirPerp = transform.right;
                break;
        }
    
        dir *= indicator.IndicatorHeight;
        if (indicator.invertDirection) dir *= -1f;

        Gizmos.DrawLine(transform.position, transform.position + dir * (currentTime/time));
        Gizmos.DrawLine(
            transform.position + dirPerp * -indicator.IndicatorWidth, 
            transform.position + dirPerp * indicator.IndicatorWidth);
        Gizmos.DrawLine(
            transform.position + dirPerp * -indicator.IndicatorWidth + dir, 
            transform.position + dirPerp * indicator.IndicatorWidth + dir);
    }
#endif
}
