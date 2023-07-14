using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDirector : MonoBehaviour
{
    [SerializeField]
    GameObject drawPanel;

    [SerializeField]
    Text selectedText1, selectedText2, selectedText3, sacrificeNumText;

    [SerializeField]
    Button soldierButton, sacrificeButton, monsterButton, predictionButton, cancelButton;

    // プレイヤーとターン
    int nowPlayer;
    int turnCount;
    bool isCpu;

    // 生贄の数
    int sacrificeNum;

    // モード
    enum Mode
    {
        None,
        Start,
        Select,
        Process,
        End
    }

    Mode nowMode, nextMode;

    public enum CardType
    {
        Soldier,    // 兵士
        Sacrifice,  // 生贄
        Monster,    // 怪物
        Prediction, // 予言
        None,       // 無し
        PSoldier,　 // 兵士を予言
        PSacrifice, // 生贄を予言
        PMonster    // 怪物を予言
    }

    // 選択したカード一覧
    List<CardType>[] selectedCards = new List<CardType>[2];

    // 選択されたカードを表示するための文字列
    List<string>[] selectedCardTexts = new List<string>[2];

    // 予言が選択されているかどうか
    bool isPredictionSelected = false;

    [SerializeField]
    GameSceneDirector gameSceneDirector;



    // CPU
    // Start is called before the first frame update
    void Start()
    {
        selectedCards[0] = new List<CardType>();
        selectedCards[1] = new List<CardType>();

        selectedCardTexts[0] = new List<string>();
        selectedCardTexts[1] = new List<string>();

        nowPlayer = 0;

        // 初回モード
        nowMode = Mode.None;
        nextMode = Mode.Start;

        // ボタンのクリックイベントにメソッドを追加
        soldierButton.onClick.AddListener(addSoldier);
        sacrificeButton.onClick.AddListener(addSacrifice);
        monsterButton.onClick.AddListener(addMonster);
        predictionButton.onClick.AddListener(addPrediction);
        cancelButton.onClick.AddListener(selectCancel);
    }

    // Update is called once per frame
    public void drawMode()
    {
        if (Mode.Start == nowMode)
        {
            startMode();
        }
        else if (Mode.Select == nowMode)
        {
            selectMode();
        }
        else if (Mode.Process == nowMode)
        {
            processMode();
        }
        else if (Mode.End == nowMode)
        {
            endMode();
        }

        // モード変更
        if (Mode.None != nextMode)
        {
            nowMode = nextMode;
            nextMode = Mode.None;
        }

        // 選択されたカードの名前を表示
        selectedText1.text = selectedCardTexts[nowPlayer].Count > 0 ? selectedCardTexts[nowPlayer][0] : "未選択";
        selectedText2.text = selectedCardTexts[nowPlayer].Count > 1 ? selectedCardTexts[nowPlayer][1] : "未選択";
        selectedText3.text = selectedCardTexts[nowPlayer].Count > 2 ? selectedCardTexts[nowPlayer][2] : "未選択";

        // 予言が選択されている場合は予言ボタンを無効化
        predictionButton.interactable = !isPredictionSelected;
    }

    private void startMode()
    {
        drawPanel.SetActive(true);
        nextMode = Mode.Select;
    }

    private void selectMode()
    {
        // 便宜上CPUのみ想定
        // TODO: PvPの機能も入れる
        if (selectedCards[nowPlayer].Count >= 3)
        {
            // 相手の選択リストにランダムな値を代入
            for (int i = 0; i < 3; i++)
            {
                int randNum = UnityEngine.Random.Range(0, 3);
                switch (randNum)
                {
                    case 0:
                        selectedCards[1].Add(CardType.Monster);
                        break;
                    case 1:
                        selectedCards[1].Add(CardType.Soldier);
                        break;
                    case 2:
                        selectedCards[1].Add(CardType.Sacrifice);
                        break;
                }
            }

            nextMode = Mode.Process;
        }
    }

    private void processMode()
    {
        // 相手の選択したカードリストと自分が選択したカードリストを比較する
        for (int i = 0; i < selectedCards[0].Count; i++)
        {
            // 相手の選択した番号が同じカードタイプと自身の予言したカードタイプが同じ場合
            if (((selectedCards[1][i] == CardType.PSoldier && selectedCards[0][i] == CardType.Soldier)
                || (selectedCards[1][i] == CardType.PSacrifice && selectedCards[0][i] == CardType.Sacrifice)
                || (selectedCards[1][i] == CardType.PMonster && selectedCards[0][i] == CardType.Monster)))
            {
                // そのリストの相手と同じ番号の相手のカードタイプをNoneにして自身のカードタイプをMonsterにする
                selectedCards[0][i] = CardType.None;
                selectedCards[1][i] = CardType.Monster;
            }
            else if (((selectedCards[0][i] == CardType.PSoldier && selectedCards[1][i] == CardType.Soldier)
                || (selectedCards[0][i] == CardType.PSacrifice && selectedCards[1][i] == CardType.Sacrifice)
                || (selectedCards[0][i] == CardType.PMonster && selectedCards[1][i] == CardType.Monster)))
            {
                // そのリストの相手と同じ番号の相手のカードタイプをNoneにして自身のカードタイプをMonsterにする
                selectedCards[1][i] = CardType.None;
                selectedCards[0][i] = CardType.Monster;
            }
        }

        // 比較が終わったら、リストからCardTypeを取り出して、それに応じてdrawUnitを3回（リストの要素分）呼び出す
        for (int i =  0; i < selectedCards.Length; i++)
        {
            for (int j = 0; j < selectedCards[nowPlayer].Count; j++)
            {
                CardType cardType = selectedCards[i][j];

                if (cardType == CardType.Monster && sacrificeNum >= 2)
                {
                    sacrificeNum--;
                    sacrificeNumText.text = "生贄の数:" + sacrificeNum;
                }
                else if (cardType == CardType.Monster && sacrificeNum < 2)
                {
                    cardType = CardType.None;
                    Debug.Log("生贄が足りない...");
                }
                else if (cardType == CardType.Sacrifice)
                {
                    sacrificeNum++;
                    sacrificeNumText.text = "生贄の数:" + sacrificeNum;
                }
                gameSceneDirector.drawUnit(i, cardType);
            }
        }

        // 次のモードへ
        nextMode = Mode.End;
    }

    private void endMode()
    {
        // ここに終了時の処理を書く
        drawPanel.SetActive(false);
        gameSceneDirector.turnCount++;
        nowMode = Mode.None;
        nextMode = Mode.Start;

        // リストをクリア
        selectedCards[0].Clear();
        selectedCards[1].Clear();
        selectedCardTexts[0].Clear();
        selectedCardTexts[1].Clear();

        gameSceneDirector.nextMode = GameSceneDirector.Mode.Start;
    }

    private void addSoldier()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            selectedCards[nowPlayer].Add(isPredictionSelected ? CardType.PSoldier : CardType.Soldier);
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "兵士?" : "兵士");
            isPredictionSelected = false;
        }
    }

    private void addSacrifice()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            selectedCards[nowPlayer].Add(isPredictionSelected ? CardType.PSacrifice : CardType.Sacrifice);
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "生贄?" : "生贄");
            isPredictionSelected = false;
        }
    }

    private void addMonster()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            selectedCards[nowPlayer].Add(isPredictionSelected ? CardType.PMonster : CardType.Monster);
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "怪物?" : "怪物");
            isPredictionSelected = false;
        }
    }

    private void addPrediction()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            isPredictionSelected = true;
        }
    }

    private void selectCancel()
    {
        selectedCards[nowPlayer].Clear();
        selectedCardTexts[nowPlayer].Clear();
        isPredictionSelected = false;
    }
}
