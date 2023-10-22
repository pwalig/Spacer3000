using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMultiStage : AiBehaviour
{
    [System.Serializable]
    class Stage
    {
        public AiBehaviour behaviour;
        [Tooltip("Names of the attacks allowed in this stage")]
        public List<string> allowedAttackNames;
        [HideInInspector] public List<int> allowedAttackIds;
        [Tooltip("value from 0 to 1 represents % of health that boss has to drop below in order to move to next stage")]
        public float hpThreshold;
        [Tooltip("signal is sent after threshold is surpassed")]
        public bool sendWaveSignal;
    }

    [SerializeField] List<Stage> stages;
    int currentStageId = 0;
    Stage cStage;
    Spaceship ship;
    private void Awake()
    {
        ship = GetComponent<Spaceship>();
        cStage = stages[currentStageId];
    }

    /// <summary>
    /// Shorthand for stages[currentStageId]
    /// </summary>
    /// <returns>stages[currentStageId]</returns>
    Stage CStage()
    {
        return stages[currentStageId];
    }

    void GenerateAllowedAttackIds(Stage stage)
    {
        stage.allowedAttackIds.Clear();
        foreach (string aa in stage.allowedAttackNames)
        {
            stage.allowedAttackIds.Add(ship.AttackNameToId(aa));
        }
    }


    private void Start()
    {
        foreach(Stage s in stages)
            if (s.allowedAttackNames.Count > 0)
                GenerateAllowedAttackIds(s);

        cStage.behaviour.levelOffset = levelOffset;
        cStage.behaviour.availableAttacksCount = cStage.allowedAttackNames.Count;
    }

    public override void Behave()
    {
        // EVALUATE CURRENT BEHAVIOUR
        cStage.behaviour.Behave();

        // SHOOTING
        shoot = false;
        if (ship.canShoot)
        {
            if (cStage.behaviour.attack < 0) attack = cStage.allowedAttackIds[Random.Range(0, cStage.allowedAttackNames.Count)]; // if current stage behaviour choses random attack pick a random attack
            else attack = cStage.allowedAttackIds[cStage.behaviour.attack]; // map attacks from stage behaviour's choice to list of available attacks in this stage
            shoot = cStage.behaviour.shoot; //copy stage's behaviour's shooting choice
        }

        // MOVEMENT
        moveDirectionX = cStage.behaviour.moveDirectionX;
        moveDirectionY = cStage.behaviour.moveDirectionY;

        // STAGE MORPHING
        if (ship.hp / ship.maxHp < cStage.hpThreshold)
        {
            if (cStage.sendWaveSignal) LevelManager.SendSignal();
            currentStageId = Mathf.Clamp(currentStageId + 1, 0, stages.Count);
            cStage = stages[currentStageId];
            cStage.behaviour.levelOffset = levelOffset; //inform new behaviour of level offset
            cStage.behaviour.availableAttacksCount = cStage.allowedAttackNames.Count; //inform new behaviour of the ammount of attacks available
        }
    }
}
