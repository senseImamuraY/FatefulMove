using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDirector : MonoBehaviour
{
    // �v���C���[�ƃ^�[��
    int nowPlayer;
    int turnCount;
    bool isCpu;

    // ���[�h
    enum Mode
    {
        None,
        Start,
        Select,
        Process,
    }

    Mode nowMode, nextMode;

    public enum CardType
    {
        Soldier,    // ���m
        Sacrifice,  // ����
        Monster,    // ����
        Prediction, // �\��
    }

    // �G�w�ݒ�
    const int EnemyLine = 3;
    List<int>[] enemyLines;

    // CPU
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
