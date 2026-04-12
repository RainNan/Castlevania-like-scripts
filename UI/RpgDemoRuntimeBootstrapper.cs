using System.Collections.Generic;
using UnityEngine;

public class RpgDemoRuntimeBootstrapper : MonoBehaviour
{
    private readonly HashSet<int> installedHealthBars = new HashSet<int>();
    private float nextScanTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InstallAfterSceneLoad()
    {
        var go = new GameObject("RPG Demo Runtime Bootstrapper");
        go.AddComponent<RpgDemoRuntimeBootstrapper>();
    }

    private void Start()
    {
        InstallPlayerSystems();
        InstallEnemyHealthBars();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextScanTime)
            return;

        nextScanTime = Time.unscaledTime + 0.5f;
        InstallPlayerSystems();
        InstallEnemyHealthBars();
    }

    private static void InstallPlayerSystems()
    {
        var player = FindObjectOfType<Player>();
        if (player == null)
            return;

        var skillController = player.GetComponent<PlayerSkillController>();
        if (skillController == null)
            skillController = player.gameObject.AddComponent<PlayerSkillController>();

        if (FindObjectOfType<PlayerStatsPanel>() == null)
            PlayerStatsPanel.CreateRuntime(player, skillController);

        if (FindObjectOfType<SkillCooldownBar>() == null)
            SkillCooldownBar.CreateRuntime(skillController);
    }

    private void InstallEnemyHealthBars()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.IsDead)
                continue;

            var id = enemy.GetInstanceID();
            if (!installedHealthBars.Add(id))
                continue;

            EntityHealthBar.Create(enemy);
        }
    }
}
