using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{

    [SerializeField, Header("�V���b�t���̗�")]
    private float shufflePower = 10.0f;

    [SerializeField, Header("�V���b�t���̑��x")]
    private Vector2 shuffleVelocity = new Vector2(10.0f, 10.0f);

    [SerializeField, Header("�V���b�t���̎���")]
    private float duration = 1.0f;

    private CapsuleCollider2D capsuleCol;      // Collider�̃I��/�I�t����p
    private float shuffleTimer;                // �V���b�t�����Ԃ̃J�E���g�p

    private UIManager uiManager;

    /// <summary>
    /// �V���b�t���̏����ݒ�
    /// </summary>
    /// <param name="uiManager"></param>
    public void SetUpShuffle(UIManager uiManager)
    {
        this.uiManager = uiManager;

        capsuleCol = GetComponent<CapsuleCollider2D>();

        // �V���b�t���p�̃R���C�_�[���I�t�ɂ��Ă���
        capsuleCol.enabled = false;
    }

    /// <summary>
    /// �V���b�t���J�n
    /// </summary>
    public void StartShuffle()
    {
        // �R���C�_�[���I���ɂ��Ċ��x���V���b�t���ł���悤�ɂ���
        capsuleCol.enabled = true;

        // �V���b�t�����Ԃ�ݒ�
        shuffleTimer = duration;

        // �V���b�t���̕����������_���Ŏ擾
        int value = Random.Range(0, 2);

        // �V���b�t���̕������V���b�t�����x��x�ɐݒ�(-1 = �������A 1 = �E����)
        shuffleVelocity.x = value == 0 ? shuffleVelocity.x *= -1 : shuffleVelocity.x *= 1;
    }

    private void StopShuffle()
    {
        shuffleTimer = 0;

        // �R���C�_�[���I�t�ɂ��Ċ��x�ւ̉e�����Ȃ���
        capsuleCol.enabled = false;

        // �ēx�V���b�t���{�^����������悤�ɂ���
        uiManager.ActivateShuffleButton(true);
    }

    /// <summary>
    /// �V���b�t���̎�����
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.TryGetComponent(out Rigidbody2D rb))
        {
            // �R���C�_�[���ɂ��銱�x�ɑ΂��ĕ������Z���s��

            // ���x�ɑ΂���V���b�t���̑��Α��x���v�Z
            Vector2 relativeVelocity = shuffleVelocity - rb.velocity;

            // ���x�ɗ͂�������(�V���b�t��)
            rb.AddForce(shufflePower * relativeVelocity);
        }
    }

    void Update()
    {
        // �V���b�t�����̂݁AshuffleTimer���J�E���g
        shuffleTimer -= Time.deltaTime;

        // shuffleTimer��0�ɂȂ�A���R���C�_�[���I���̏ꍇ�ɂ�
        if (shuffleTimer <= 0 && capsuleCol.enabled)
        {
            // �V���b�t����~
            StopShuffle();
        }
    }
}
