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
            public string hostFromName;
            public string friendToName;
            [HideInInspector] public int hostFromId;
            [HideInInspector] public int friendToId;
        }
        [SerializeField] List<Mapping> attackMappings;

        public enum State { NotFound, Found, Destroyed}
        [HideInInspector] public State state = State.NotFound;

        public void GenerateMappingIds(Spaceship hostSpaceship)
        {
            Spaceship s = behaviour.gameObject.GetComponent<Spaceship>();
            for(int i = 0; i<attackMappings.Count; i++)
            {
                attackMappings[i].hostFromId = hostSpaceship.AttackNameToId(attackMappings[i].hostFromName);
                attackMappings[i].friendToId = s.AttackNameToId(attackMappings[i].friendToName);
#if UNITY_EDITOR
                Debug.Log("host: " + attackMappings[i].hostFromName + " to " + attackMappings[i].hostFromId);
                Debug.Log("friend: " + attackMappings[i].friendToName + " to " + attackMappings[i].friendToId);
#endif
            }
        }

        /// <summary>
        /// sets attack of the friend's behaviour according to the attackMap
        /// </summary>
        /// <param name="hostAttackId">map argument</param>
        public void SetAttack(int hostAttackId=0)
        {
            List<int> availableAtacks = new();
            foreach(Mapping m in attackMappings)
            {
                if (m.hostFromId == hostAttackId)
                    availableAtacks.Add(m.friendToId);
            }
            if (availableAtacks.Count > 0) behaviour.attack = availableAtacks[Random.Range(0, availableAtacks.Count)];
            else behaviour.attack = -1; // if no mappings available choose random attack
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
                    friend.GenerateMappingIds(GetComponent<Spaceship>());
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
