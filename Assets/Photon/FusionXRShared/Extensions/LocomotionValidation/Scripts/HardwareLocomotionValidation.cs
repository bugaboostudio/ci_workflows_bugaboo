using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Locomotion
{

    /**
      * Handle locomotion limitations by forwarding the ILocomotionValidator and ILocomotionObserver calls to its children implementing the interface 
      */
    public abstract class ChildrenBasedLocomotionValidation : SimulationBehaviour, ILocomotionValidationHandler
    {

        List<ILocomotionValidator> locomotionValidators = new List<ILocomotionValidator>();
        List<ILocomotionObserver> locomotionObservers = new List<ILocomotionObserver>();

        protected virtual void Awake()
        {
            foreach (var v in GetComponentsInChildren<ILocomotionValidator>())
            {
                if ((object)v != this) locomotionValidators.Add(v);
            }
            foreach (var v in GetComponentsInChildren<ILocomotionObserver>())
            {
                if ((object)v != this) locomotionObservers.Add(v);
            }
        }

        #region ILocomotionObserver
        // Forward the ILocomotionValidator.OnDidMove callbacks to any child ILocomotionValidator
        //  Used by the locomotion systems to warn that an actual move did occur
        public virtual void OnDidMove()
        {
            ChildrenOnDidMove();
        }

        protected void ChildrenOnDidMove()
        {
            foreach (var validator in locomotionObservers)
            {
                validator.OnDidMove();
            }
        }

        public virtual void OnDidMoveFadeFinished()
        {
            ChildrenOnDidMoveFadeFinished();
        }

        protected void ChildrenOnDidMoveFadeFinished()
        { 
            foreach (var validator in locomotionObservers)
            {
                validator.OnDidMoveFadeFinished();
            }
        }
        #endregion

        #region ILocomotionValidator
        // Forward the ILocomotionValidator.CanMove request to any child ILocomotionValidator and return false if any of them returns false
        //  Used by the locomotion systems to validate if an incoming move is valid
        public virtual bool CanMoveHeadset(Vector3 position)
        {
            if (!ChildrenCanMoveHeadset(position)) return false;
            return true;
        }

        public bool ChildrenCanMoveHeadset(Vector3 position)
        {
            foreach (var validator in locomotionValidators)
            {
                if (!validator.CanMoveHeadset(position))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }


    /**
     * Handle locomotion limitations for the HardwareRig: check on HardwareRig children, then on NetworkRig children if it has a NetworkLocomotionValidation component
     */
    [RequireComponent(typeof(HardwareRig))]
    public class HardwareLocomotionValidation : ChildrenBasedLocomotionValidation
    {
        [Header("Headset movement analysis")]
        public bool checkHeadMovements = true;

        Vector3 lastHeadMovement;
        float minHeadMovementDetected = 0.05f;
        float minHeadMovementDetectedSqr;

        HardwareRig hardwareRig;
        RigInfo rigInfo;
        bool didSearchNetworkLocomotionValidation = false;
        NetworkLocomotionValidation _networkLocomotionValidation;
        NetworkLocomotionValidation NetworkLocomotionValidation
        {
            get
            {
                if (rigInfo == null) return null;
                if (rigInfo.localNetworkedRig && !didSearchNetworkLocomotionValidation)
                {
                    _networkLocomotionValidation = rigInfo.localNetworkedRig.GetComponentInChildren<NetworkLocomotionValidation>();
                }
                return _networkLocomotionValidation;

            }
        }

        protected override void Awake()
        {
            base.Awake();
            hardwareRig = GetComponent<HardwareRig>();
            if (hardwareRig) rigInfo = RigInfo.FindRigInfo(hardwareRig.runner);
            minHeadMovementDetectedSqr = minHeadMovementDetected * minHeadMovementDetected;
        }


        #region ILocomotionObserver
        // Forward the ILocomotionValidator.OnDidMove callbacks to any child ILocomotionValidator, and to the NetworkRig ILocomotionValidator childs
        //  Used by the locomotion systems to warn that an actual move did occur
        public override void OnDidMove()
        {
            // We use both hardware rig and local user networked rig as a source of locomotion validation
            base.OnDidMove();
            if (NetworkLocomotionValidation) NetworkLocomotionValidation.OnDidMove();
        }

        public override void OnDidMoveFadeFinished()
        {
            base.OnDidMove();
            if (NetworkLocomotionValidation) NetworkLocomotionValidation.OnDidMoveFadeFinished();
        }
        #endregion

        #region ILocomotionValidator
        // Forward the ILocomotionValidator.CanMove request to any child ILocomotionValidator, and to the NetworkRig ILocomotionValidator childs, and return false if any of them returns false
        //  Used by the locomotion systems to validate if an incoming move is valid
        public override bool CanMoveHeadset(Vector3 position)
        {
            // We use both hardware rig and local user networked rig as a source of locomotion validation
            if (!base.CanMoveHeadset(position)) return false;
            if (NetworkLocomotionValidation && !NetworkLocomotionValidation.CanMoveHeadset(position)) return false;
            return true;
        }
        #endregion

        private void Update()
        {
            if (checkHeadMovements)
            {
                if ((hardwareRig.headset.transform.position - lastHeadMovement).sqrMagnitude > minHeadMovementDetectedSqr)
                {
                    OnDidMove();
                }
            }
        }
    }
}
