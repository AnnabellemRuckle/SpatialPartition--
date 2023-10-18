using UnityEngine;
using System.Collections;

namespace SpatialPartitionPattern
{
    
    public class Soldier
    {
        public MeshRenderer soldierMeshRenderer;
        //To move the soldier
        public Transform soldierTrans;
        //The speed the soldier is walking with
        protected float walkSpeed;
        public Soldier previousSoldier;
        public Soldier nextSoldier;
        public virtual void Move()
        { }
        public virtual void Move(Soldier soldier)
        { }
    }
}