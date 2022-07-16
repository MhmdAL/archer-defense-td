public class CooldownTimer
{
    public float startTime;
    public float duration;
    public float currentTime;
    public bool isRunning = false;

    private float elapsed;

    public float Elapsed
    {
        get
        {
            if (isRunning)
            { // If timer is running return elapsed time before the last stop + elapsed time after starting
                return elapsed + (ValueStore.CurrentTime - startTime);
            }
            else
            { // If timer is NOT running return elapsed time before the last stop
                return elapsed;
            }
        }
        set
        {
            elapsed = value;
        }
    }

    public CooldownTimer(float duration)
    {
        this.duration = duration;

        Start();
    }

    public void Start()
    {
        if (!isRunning)
        { // If timer is NOT running then set it to running and set the start time to current time
            isRunning = true;
            startTime = ValueStore.CurrentTime;
        }
    }

    public void Stop()
    { // If timer is running disable it and adjust elapsed time
        if (isRunning)
        {
            isRunning = false;
            elapsed += ValueStore.CurrentTime - startTime;
        }
    }

    public void ResetTimer(float duration)
    {
        // Reset Elapsed time to 0
        elapsed = 0;
        // Set duration to new duration
        this.duration = duration;
        // Reset starttime to current time
        startTime = ValueStore.CurrentTime;
    }

    public float GetCooldownRemaining()
    {
        return duration - Elapsed;
    }
}