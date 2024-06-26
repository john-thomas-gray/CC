using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreManager", menuName = "ScriptableObjects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    // Change score to a long
    public int score = 0;
    public int startingScore = 0;
    public int enemies_destroyed = 0;

    [System.NonSerialized]
    public UnityEvent<int> scoreChangeEvent;
    public IntEventChannelSO enemyDestroyedECSO;
    public VoidEventChannelSO fleetWipeEC;

    private void OnEnable() {
        score = startingScore;
        enemies_destroyed = 0;
        if (scoreChangeEvent == null)
        {
            scoreChangeEvent = new UnityEvent<int>();
        }
        fleetWipeEC.OnEventRaised += FleetWipe;
    }

    private void OnDisable()
    {
        fleetWipeEC.OnEventRaised -= FleetWipe;
    }
    public void IncreaseScore(int amount) {
        score += amount;
        enemies_destroyed += 1;
        enemyDestroyedECSO.RaiseEvent(enemies_destroyed);
        scoreChangeEvent.Invoke(score);
    }

    public void FleetWipe()
    {
        score += 10000;
        scoreChangeEvent.Invoke(score);
    }
}
