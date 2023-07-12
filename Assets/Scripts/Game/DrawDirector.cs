using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDirector : MonoBehaviour
{
    // プレイヤーとターン
    int nowPlayer;
    int turnCount;
    bool isCpu;

    // モード
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
        Soldier,    // 兵士
        Sacrifice,  // 生贄
        Monster,    // 怪物
        Prediction, // 予言
    }

    // 敵陣設定
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
