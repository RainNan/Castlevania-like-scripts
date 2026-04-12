using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerSkillCastType
{
    FrontArea,
    ChaseTarget,
    AroundPlayer,
    ForwardWave,
    GroundSpike,
    ShadowBurst,
    LaserBeam,
    BlinkForward
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class PlayerSkillController : MonoBehaviour
{
    [Serializable]
    public class SkillSlot
    {
        public string displayName = "Power Slash";
        public string binding = "<Keyboard>/q";
        public PlayerSkillCastType castType = PlayerSkillCastType.FrontArea;
        public float cooldown = 2f;
        public float damageMultiplier = 1.5f;
        public float range = 1.4f;
        public float radius = 0.7f;
        public float chaseStopDistance = 0.55f;
        public Color effectColor = new Color(1f, 0.35f, 0.1f, 0.55f);

        [NonSerialized] public float cooldownRemaining;

        public bool IsReady => cooldownRemaining <= 0f;
    }

    [SerializeField]
    private LayerMask enemyLayerMask;
    [SerializeField]
    private Vector2 castOffset = new Vector2(0.7f, 0.25f);
    [SerializeField]
    private SkillSlot[] skills =
    {
        new SkillSlot
        {
            displayName = "Laser Beam",
            binding = "<Keyboard>/q",
            castType = PlayerSkillCastType.LaserBeam,
            cooldown = 2.5f,
            damageMultiplier = 2f,
            range = 6f,
            radius = 0.28f,
            effectColor = new Color(1f, 0.1f, 0.05f, 0.75f)
        },
        new SkillSlot
        {
            displayName = "Blink",
            binding = "<Keyboard>/e",
            castType = PlayerSkillCastType.BlinkForward,
            cooldown = 3f,
            damageMultiplier = 0f,
            range = 3f,
            radius = 0.55f,
            effectColor = new Color(0.15f, 0.85f, 1f, 0.65f)
        },
        new SkillSlot
        {
            displayName = "Explosion",
            binding = "<Keyboard>/r",
            castType = PlayerSkillCastType.ShadowBurst,
            cooldown = 5f,
            damageMultiplier = 2.5f,
            range = 0f,
            radius = 2f,
            effectColor = new Color(1f, 0.55f, 0.08f, 0.62f)
        },
        new SkillSlot
        {
            displayName = "Spirit Wave",
            binding = "<Keyboard>/f",
            castType = PlayerSkillCastType.ForwardWave,
            cooldown = 3.5f,
            damageMultiplier = 1.8f,
            range = 4.2f,
            radius = 0.55f,
            effectColor = new Color(0.95f, 0.9f, 0.18f, 0.5f)
        },
        new SkillSlot
        {
            displayName = "Ground Spike",
            binding = "<Keyboard>/c",
            castType = PlayerSkillCastType.GroundSpike,
            cooldown = 5f,
            damageMultiplier = 2.4f,
            range = 3.2f,
            radius = 0.65f,
            effectColor = new Color(0.55f, 0.95f, 0.45f, 0.55f)
        },
        new SkillSlot
        {
            displayName = "Shadow Burst",
            binding = "<Keyboard>/v",
            castType = PlayerSkillCastType.ShadowBurst,
            cooldown = 7f,
            damageMultiplier = 2.8f,
            range = 0f,
            radius = 2f,
            effectColor = new Color(0.35f, 0.22f, 0.95f, 0.5f)
        }
    };

    private readonly List<InputAction> skillActions = new List<InputAction>();
    private readonly HashSet<Entity> damagedTargets = new HashSet<Entity>();
    private Player player;

    public IReadOnlyList<SkillSlot> Skills => skills;
    public event Action<PlayerSkillController> SkillsChanged;

    private void Awake()
    {
        player = GetComponent<Player>();

        if (enemyLayerMask.value == 0 && player != null)
            enemyLayerMask = player.EnemyLayerMask;

        EnsureDefaultSkills();
    }

    private void OnEnable()
    {
        RebuildActions();
    }

    private void OnDisable()
    {
        ReleaseActions();
    }

    private void Update()
    {
        var changed = TickCooldowns();

        for (var i = 0; i < skillActions.Count && i < skills.Length; i++)
        {
            if (skillActions[i].WasPressedThisFrame())
                changed |= TryCast(i);
        }

        if (changed)
            SkillsChanged?.Invoke(this);
    }

    public bool TryCast(int index)
    {
        if (index < 0 || index >= skills.Length)
            return false;

        var skill = skills[index];
        if (skill == null || !skill.IsReady || player == null || player.IsDead)
            return false;

        var hitCount = skill.castType switch
        {
            PlayerSkillCastType.ChaseTarget => CastChaseSkill(skill),
            PlayerSkillCastType.AroundPlayer => CastAroundPlayer(skill),
            PlayerSkillCastType.ForwardWave => CastForwardWave(skill),
            PlayerSkillCastType.GroundSpike => CastGroundSpike(skill),
            PlayerSkillCastType.ShadowBurst => CastShadowBurst(skill),
            PlayerSkillCastType.LaserBeam => CastLaserBeam(skill),
            PlayerSkillCastType.BlinkForward => CastBlinkForward(skill),
            _ => CastFrontArea(skill)
        };

        skill.cooldownRemaining = Mathf.Max(0.01f, skill.cooldown);
        return hitCount >= 0;
    }

    private bool TickCooldowns()
    {
        var changed = false;

        foreach (var skill in skills)
        {
            if (skill == null || skill.cooldownRemaining <= 0f)
                continue;

            skill.cooldownRemaining = Mathf.Max(0f, skill.cooldownRemaining - Time.deltaTime);
            changed = true;
        }

        return changed;
    }

    private int CastFrontArea(SkillSlot skill)
    {
        var origin = GetCastOrigin(skill.range * 0.5f);
        SkillVisualEffect.Spawn(origin, skill.radius, skill.effectColor);
        return DamageTargets(Physics2D.OverlapCircleAll(origin, skill.radius, enemyLayerMask), skill);
    }

    private int CastAroundPlayer(SkillSlot skill)
    {
        var origin = (Vector2)transform.position + Vector2.up * castOffset.y;
        SkillVisualEffect.Spawn(origin, skill.radius, skill.effectColor);
        return DamageTargets(Physics2D.OverlapCircleAll(origin, skill.radius, enemyLayerMask), skill);
    }

    private int CastChaseSkill(SkillSlot skill)
    {
        var target = FindChaseTarget(skill);
        if (target == null)
        {
            var missOrigin = GetCastOrigin(Mathf.Min(skill.range, 1.2f));
            SkillVisualEffect.Spawn(missOrigin, skill.radius, skill.effectColor);
            return 0;
        }

        var sign = player.GetFaceRightSign;
        var playerPosition = transform.position;
        transform.position = new Vector3(
            target.transform.position.x - sign * skill.chaseStopDistance,
            playerPosition.y,
            playerPosition.z
        );
        player.rb.velocity = new Vector2(sign * player.DashSpeed, player.rb.velocity.y);

        SkillVisualEffect.Spawn(target.transform.position, skill.radius, skill.effectColor);
        if (target is IDamageable damageable)
        {
            damageable.TakeDamage(player.Atk * skill.damageMultiplier, sign);
            return 1;
        }

        return 0;
    }

    private int CastForwardWave(SkillSlot skill)
    {
        var sign = player.GetFaceRightSign;
        var origin = player.AttackDetector != null ? (Vector2)player.AttackDetector.position : (Vector2)transform.position;
        var center = origin + new Vector2(sign * skill.range * 0.5f, castOffset.y);
        var size = new Vector2(skill.range, skill.radius * 2f);

        var effectCount = Mathf.Max(2, Mathf.CeilToInt(skill.range / Mathf.Max(0.5f, skill.radius)));
        for (var i = 0; i < effectCount; i++)
        {
            var t = (i + 0.5f) / effectCount;
            var effectPosition = origin + new Vector2(sign * skill.range * t, castOffset.y);
            SkillVisualEffect.Spawn(effectPosition, skill.radius, skill.effectColor, 0.32f);
        }

        return DamageTargets(Physics2D.OverlapBoxAll(center, size, 0f, enemyLayerMask), skill);
    }

    private int CastLaserBeam(SkillSlot skill)
    {
        var sign = player.GetFaceRightSign;
        var origin = player.AttackDetector != null ? (Vector2)player.AttackDetector.position : (Vector2)transform.position;
        origin += Vector2.up * castOffset.y;

        var center = origin + Vector2.right * sign * skill.range * 0.5f;
        var size = new Vector2(skill.range, skill.radius * 2f);
        SkillVisualEffect.Spawn(center, skill.radius, skill.effectColor, 0.18f);

        var effectCount = Mathf.Max(4, Mathf.CeilToInt(skill.range / 0.55f));
        for (var i = 0; i < effectCount; i++)
        {
            var t = (i + 0.5f) / effectCount;
            var effectPosition = origin + Vector2.right * sign * skill.range * t;
            SkillVisualEffect.Spawn(effectPosition, skill.radius * 1.15f, skill.effectColor, 0.2f);
        }

        return DamageTargets(Physics2D.OverlapBoxAll(center, size, 0f, enemyLayerMask), skill);
    }

    private int CastBlinkForward(SkillSlot skill)
    {
        var sign = player.GetFaceRightSign;
        var startPosition = transform.position;
        var endPosition = startPosition + Vector3.right * sign * skill.range;

        SkillVisualEffect.Spawn(startPosition, skill.radius, skill.effectColor, 0.2f);
        transform.position = endPosition;
        player.rb.velocity = new Vector2(0f, player.rb.velocity.y);
        SkillVisualEffect.Spawn(endPosition, skill.radius, skill.effectColor, 0.2f);

        return 0;
    }


    private int CastGroundSpike(SkillSlot skill)
    {
        var sign = player.GetFaceRightSign;
        var origin = (Vector2)transform.position + new Vector2(sign * skill.range * 0.5f, -0.2f);
        var size = new Vector2(skill.range, skill.radius * 2.2f);

        var spikeCount = Mathf.Max(3, Mathf.CeilToInt(skill.range / 0.7f));
        for (var i = 0; i < spikeCount; i++)
        {
            var t = (i + 0.5f) / spikeCount;
            var spikePosition = (Vector2)transform.position + new Vector2(sign * skill.range * t, -0.2f);
            SkillVisualEffect.Spawn(spikePosition, skill.radius * Mathf.Lerp(0.65f, 1f, t), skill.effectColor, 0.38f);
        }

        return DamageTargets(Physics2D.OverlapBoxAll(origin, size, 0f, enemyLayerMask), skill);
    }

    private int CastShadowBurst(SkillSlot skill)
    {
        var origin = (Vector2)transform.position + Vector2.up * castOffset.y;
        SkillVisualEffect.Spawn(origin, skill.radius, skill.effectColor, 0.42f);
        SkillVisualEffect.Spawn(origin, skill.radius * 0.55f, new Color(1f, 1f, 1f, 0.35f), 0.24f);

        return DamageTargetsRadial(Physics2D.OverlapCircleAll(origin, skill.radius, enemyLayerMask), skill, origin);
    }

    private Entity FindChaseTarget(SkillSlot skill)
    {
        var sign = player.GetFaceRightSign;
        var hits = Physics2D.OverlapCircleAll(transform.position, skill.range, enemyLayerMask);
        Entity bestTarget = null;
        var bestDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            var target = hit.GetComponentInParent<Entity>();
            if (target == null || target == player || target.IsDead)
                continue;

            var xDelta = (target.transform.position.x - transform.position.x) * sign;
            if (xDelta < -0.1f)
                continue;

            var sqrDistance = ((Vector2)target.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDistance >= bestDistance)
                continue;

            bestTarget = target;
            bestDistance = sqrDistance;
        }

        return bestTarget;
    }

    private int DamageTargets(Collider2D[] hits, SkillSlot skill)
    {
        damagedTargets.Clear();

        foreach (var hit in hits)
        {
            var target = hit.GetComponentInParent<Entity>();
            if (target == null || target == player || target.IsDead || !damagedTargets.Add(target))
                continue;

            if (target is IDamageable damageable)
                damageable.TakeDamage(player.Atk * skill.damageMultiplier, player.GetFaceRightSign);
        }

        return damagedTargets.Count;
    }

    private int DamageTargetsRadial(Collider2D[] hits, SkillSlot skill, Vector2 origin)
    {
        damagedTargets.Clear();

        foreach (var hit in hits)
        {
            var target = hit.GetComponentInParent<Entity>();
            if (target == null || target == player || target.IsDead || !damagedTargets.Add(target))
                continue;

            if (target is IDamageable damageable)
            {
                var attackDir = Mathf.Sign(target.transform.position.x - origin.x);
                if (Mathf.Approximately(attackDir, 0f))
                    attackDir = player.GetFaceRightSign;

                damageable.TakeDamage(player.Atk * skill.damageMultiplier, attackDir);
            }
        }

        return damagedTargets.Count;
    }

    private Vector2 GetCastOrigin(float forwardDistance)
    {
        var sign = player.GetFaceRightSign;
        var basePosition = player.AttackDetector != null ? player.AttackDetector.position : transform.position;
        return (Vector2)basePosition + new Vector2(sign * (castOffset.x + forwardDistance), castOffset.y);
    }

    private void RebuildActions()
    {
        ReleaseActions();

        foreach (var skill in skills)
        {
            if (skill == null || string.IsNullOrWhiteSpace(skill.binding))
                continue;

            var action = new InputAction(skill.displayName, InputActionType.Button, skill.binding);
            action.Enable();
            skillActions.Add(action);
        }
    }

    private void ReleaseActions()
    {
        foreach (var action in skillActions)
        {
            action.Disable();
            action.Dispose();
        }

        skillActions.Clear();
    }

    private void EnsureDefaultSkills()
    {
        if (skills != null && skills.Length > 0)
            return;

        skills = new[]
        {
            new SkillSlot
            {
                displayName = "Laser Beam",
                binding = "<Keyboard>/q",
                castType = PlayerSkillCastType.LaserBeam,
                cooldown = 2.5f,
                damageMultiplier = 2f,
                range = 6f,
                radius = 0.28f,
                effectColor = new Color(1f, 0.1f, 0.05f, 0.75f)
            },
            new SkillSlot
            {
                displayName = "Blink",
                binding = "<Keyboard>/e",
                castType = PlayerSkillCastType.BlinkForward,
                cooldown = 3f,
                damageMultiplier = 0f,
                range = 3f,
                radius = 0.55f,
                effectColor = new Color(0.15f, 0.85f, 1f, 0.65f)
            },
            new SkillSlot
            {
                displayName = "Explosion",
                binding = "<Keyboard>/r",
                castType = PlayerSkillCastType.ShadowBurst,
                cooldown = 5f,
                damageMultiplier = 2.5f,
                radius = 2f,
                effectColor = new Color(1f, 0.55f, 0.08f, 0.62f)
            },
            new SkillSlot
            {
                displayName = "Spirit Wave",
                binding = "<Keyboard>/f",
                castType = PlayerSkillCastType.ForwardWave,
                cooldown = 3.5f,
                damageMultiplier = 1.8f,
                range = 4.2f,
                radius = 0.55f,
                effectColor = new Color(0.95f, 0.9f, 0.18f, 0.5f)
            },
            new SkillSlot
            {
                displayName = "Ground Spike",
                binding = "<Keyboard>/c",
                castType = PlayerSkillCastType.GroundSpike,
                cooldown = 5f,
                damageMultiplier = 2.4f,
                range = 3.2f,
                radius = 0.65f,
                effectColor = new Color(0.55f, 0.95f, 0.45f, 0.55f)
            },
            new SkillSlot
            {
                displayName = "Shadow Burst",
                binding = "<Keyboard>/v",
                castType = PlayerSkillCastType.ShadowBurst,
                cooldown = 7f,
                damageMultiplier = 2.8f,
                radius = 2f,
                effectColor = new Color(0.35f, 0.22f, 0.95f, 0.5f)
            }
        };
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (skills == null)
            return;

        var playerSign = player != null ? player.GetFaceRightSign : Mathf.Sign(transform.localScale.x);
        foreach (var skill in skills)
        {
            if (skill == null)
                continue;

            Gizmos.color = skill.effectColor;
            if (skill.castType == PlayerSkillCastType.ForwardWave || skill.castType == PlayerSkillCastType.GroundSpike || skill.castType == PlayerSkillCastType.LaserBeam)
            {
                var yOffset = skill.castType == PlayerSkillCastType.GroundSpike ? -0.2f : castOffset.y;
                var origin = (Vector2)transform.position + new Vector2(playerSign * skill.range * 0.5f, yOffset);
                Gizmos.DrawWireCube(origin, new Vector3(skill.range, skill.radius * 2f, 0f));
            }
            else if (skill.castType == PlayerSkillCastType.BlinkForward)
            {
                var origin = (Vector2)transform.position + Vector2.right * playerSign * skill.range;
                Gizmos.DrawWireSphere(origin, skill.radius);
            }
            else
            {
                var origin = skill.castType == PlayerSkillCastType.AroundPlayer
                    ? (Vector2)transform.position + Vector2.up * castOffset.y
                    : (Vector2)transform.position + new Vector2(playerSign * (castOffset.x + skill.range * 0.5f), castOffset.y);
                Gizmos.DrawWireSphere(origin, skill.radius);
            }
        }
    }
#endif
}
