using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : BaseManager<CooldownManager>
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _completedCooldownQueue = new();

    protected override void OnInit()
    {

    }
    protected override void OnClear()
    {
        foreach (var cooldown in _cooldowns)
        {
            cooldown.Clear();
        }

        _cooldowns.Clear();
        _completedCooldownQueue.Clear();
    }

    protected override void OnDispose()
    {

    }

    public void UpdateCooldowns()
    {
        foreach (var cooldown in _cooldowns)
        {
            if (cooldown.RemainingTime >= 0f)
            {
                cooldown.RemainingTime -= Time.deltaTime;
            }
            else
            {
                _completedCooldownQueue.Enqueue(cooldown);
            }
        }

        while (_completedCooldownQueue.Count > 0)
        {
            var completedCooldown = _completedCooldownQueue.Dequeue();
            _cooldowns.Remove(completedCooldown);
        }
    }

    public void AddCooldown(Cooldown cooldown)
    {
        if (cooldown == null)
        {
            return;
        }

        if (_cooldowns.Add(cooldown))
        {
            cooldown.Start();
        }
    }
}
