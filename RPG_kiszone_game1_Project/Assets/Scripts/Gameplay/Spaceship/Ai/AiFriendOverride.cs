using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFriendOverride : AiBehaviour
{
    [System.Serializable] class FriendInfo
    {
        public string name;
        [HideInInspector] public AiBehaviour behaviour;

        [System.Serializable] class Mapping
        {
            public string name;
            public string hostFromName;
            public string friendToName;
            [HideInInspector] public int hostFromId;
            [HideInInspector] public int friendToId;
        }
        [SerializeField] List<Mapping> attackMappings;

        public enum State { NotFound, Found, Destroyed}
        [HideInInspector] public State state = State.NotFound;

        public void GenerateMappingIds()
        {
            Spaceship s = behaviour.gameObject.GetComponent<Spaceship>();
            for(int i = 0; i<attackMappings.Count; i++)
            {
                attackMappings[i].hostFromId = s.AttackNameToId(attackMappings[i].hostFromName);
                attackMappings[i].friendToId = s.AttackNameToId(attackMappings[i].friendToName);
            }
        }

        /// <summary>
        /// sets attack of the friend's behaviour according to the attackMap
        /// </summary>
        /// <param name="hostAttackId">map argument</param>
        /// <param name="useMap">weather to use a map</param>
        public void SetAttack(int hostAttackId=0)
        {
            List<int> availableAtacks = new();
            foreach(Mapping m in attackMappings)
            {
                if (m.hostFromId == hostAttackId)
                    availableAtacks.Add(m.friendToId);
            }
            behaviour.attack = availableAtacks[Random.Range(0, availableAtacks.Count)];
        }
    }

    [SerializeField] AiBehaviour behaviour;
    [SerializeField] List<FriendInfo> friends;

    private void Start()
    {
        behaviour.levelOffset = levelOffset;
        behaviour.availableAttacksCount = availableAttacksCount;
    }

    public override void Behave()
    {
        // EVALUATE BASE BEHAVIOUR
        behaviour.Behave();
        moveDirectionX = behaviour.moveDirectionX;
        moveDirectionY = behaviour.moveDirectionY;
        shoot = behaviour.shoot;
        attack = behaviour.attack;


        // OVERRIDE FREINDS
        foreach (FriendInfo friend in friends)
        {
            if (friend.state == FriendInfo.State.NotFound) {
                GameObject fr = GameObject.Find(friend.name);
                if(fr != null)
                {
                    friend.behaviour = fr.GetComponent<EnemyController>().GetBehaviour();
                    friend.state = FriendInfo.State.Found;
#if UNITY_EDITOR
                    Debug.Log("" + name + " found!");
#endif
                    friend.GenerateMappingIds();
                }
            }
            if (friend.state == FriendInfo.State.Found)
            {
                if (friend.behaviour == null)
                    friend.state = FriendInfo.State.Destroyed;
                else
                {
                    friend.SetAttack(attack);
                    friend.behaviour.shoot = shoot;
                }
            }
        }
        attack = behaviour.attack;
        shoot = behaviour.shoot;
    }
}
