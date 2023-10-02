using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMultiStage : AiBehaviour
{
    [System.Serializable]
    class Stage
    {
        public AiBehaviour behaviour;
        public List<int> allowedAttacks;
        [Tooltip("value from 0 to 1 represents % of health that boss has to drop below in order to move to next stage")]
        public float hpThreshold;
        [Tooltip("signal is sent after threshold is surpassed")]
        public bool sendWaveSignal;
    }

    [SerializeField] List<Stage> stages;
    int currentStage = 0;
    Spaceship ship;
    private void Awake()
    {
        ship = GetComponent<Spaceship>();
    }


    void Update()
    {
        // SHOOTING
        shoot = false;
        if (ship.canShoot)
        {
            if (stages[currentStage].behaviour.attack < 0) attack = stages[currentStage].allowedAttacks[Random.Range(0, stages[currentStage].allowedAttacks.Count)];
            else attack = stages[currentStage].allowedAttacks[stages[currentStage].behaviour.attack];
            shoot = stages[currentStage].behaviour.shoot;
        }

        // MOVEMENT
        moveDirectionX = stages[currentStage].behaviour.moveDirectionX;
        moveDirectionY = stages[currentStage].behaviour.moveDirectionY;

        // STAGE MORPHING
        if (ship.hp / ship.maxHp < stages[currentStage].hpThreshold)
        {
            if (stages[currentStage].sendWaveSignal) LevelManager.SendSignal();
            currentStage = Mathf.Clamp(currentStage + 1, 0, stages.Count);
            stages[currentStage].behaviour.availableAttacks = stages[currentStage].allowedAttacks.Count;
        }
    }
}