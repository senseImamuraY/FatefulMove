using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDirector : MonoBehaviour
{
    // vC[Ζ^[
    int nowPlayer;
    int turnCount;
    bool isCpu;

    // [h
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
        Soldier,    // Ίm
        Sacrifice,  // ΆζΡ
        Monster,    // φ¨
        Prediction, // \Ύ
    }

    // Gwέθ
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
