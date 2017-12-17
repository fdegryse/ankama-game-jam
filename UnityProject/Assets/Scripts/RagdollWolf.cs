using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class RagdollWolf : MonoBehaviour
{
    [UsedImplicitly] public Collider2D headCollider;

    [UsedImplicitly] public Collider2D legCollider;

    [UsedImplicitly] public Collider2D tailCollider;

    [UsedImplicitly] public float washerActivationDelay = 1f;

    private struct TrapData
    {
        private readonly PlayerController m_playerController;
        private Joint2D m_activeJoint;
        private readonly int m_activationFrame;

        public enum ForceReleaseResult
        {
            None,
            Acquired,
            Challenged
        }

        public TrapData(PlayerController player, Joint2D joint, int frame)
        {
            m_playerController = player;
            m_activeJoint = joint;
            m_activationFrame = frame;
        }

        public void Release()
        {
            if (null == m_activeJoint)
            {
                return;
            }

            Destroy(m_activeJoint);
            m_activeJoint = null;
        }

        public ForceReleaseResult ForceRelease(int frame = -1)
        {
            if (null == m_activeJoint)
            {
                return ForceReleaseResult.None;
            }

            m_playerController.ReleaseTrappedWolf();

            Destroy(m_activeJoint);
            m_activeJoint = null;

            return frame == m_activationFrame ? ForceReleaseResult.Challenged : ForceReleaseResult.Acquired;
        }
    }

    private readonly TrapData[] m_trapDatas = new TrapData[2];

    private bool m_dead;
    public bool isDead
    {
        get { return m_dead; }
    }

    public bool AttachToTrap(PlayerController player, Collider2D hit)
    {
        Rigidbody2D hitRigidbody = hit.attachedRigidbody;
        Rigidbody2D trapRigidbody = player.trapCollider.attachedRigidbody;
        int playerIndex = player.playerId;
        int otherPlayerIndex = 1 - playerIndex;
        int frame = Time.frameCount;

        // Release current joint from other player

        Debug.Log(frame);
        TrapData.ForceReleaseResult forceReleaseResult = m_trapDatas[otherPlayerIndex].ForceRelease(frame);
        if (forceReleaseResult == TrapData.ForceReleaseResult.Challenged)
        {
            return false;
        }
        
        // Add new joint from current player

        var fixedJoint = hitRigidbody.gameObject.AddComponent<FixedJoint2D>();
        fixedJoint.connectedBody = trapRigidbody;

        m_trapDatas[playerIndex] = new TrapData(player, fixedJoint, frame);

        return true;
    }

    public void ReleaseFromTrap(PlayerController player)
    {
        int playerIndex = player.playerId;
        m_trapDatas[playerIndex].Release();
    }

    public void PutInWasher(WolfWasher wolfWasher)
    {
        if (m_dead)
        {
            return;
        }

        m_dead = true;

        m_trapDatas[0].ForceRelease();
        m_trapDatas[1].ForceRelease();

        wolfWasher.StartCoroutine(PutInWasherRoutine(wolfWasher));
    }

    public void RemoveHingeJointsLimits()
    {
        var hingeJoints = GetComponentsInChildren<HingeJoint2D>();
        foreach (HingeJoint2D hingeJoint2D in hingeJoints)
        {
            hingeJoint2D.useLimits = false;
        }
    }

    private IEnumerator PutInWasherRoutine(WolfWasher wolfWasher)
    {
        yield return new WaitForSeconds(washerActivationDelay);

        Destroy(gameObject);

        wolfWasher.enabled = true;
    }
}