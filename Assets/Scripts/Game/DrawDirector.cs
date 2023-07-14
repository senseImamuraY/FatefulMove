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

    // �v���C���[�ƃ^�[��
    int nowPlayer;
    int turnCount;
    bool isCpu;

    // ���т̐�
    int sacrificeNum;

    // ���[�h
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
        Soldier,    // ���m
        Sacrifice,  // ����
        Monster,    // ����
        Prediction, // �\��
        None,       // ����
        PSoldier,�@ // ���m��\��
        PSacrifice, // ���т�\��
        PMonster    // ������\��
    }

    // �I�������J�[�h�ꗗ
    List<CardType>[] selectedCards = new List<CardType>[2];

    // �I�����ꂽ�J�[�h��\�����邽�߂̕�����
    List<string>[] selectedCardTexts = new List<string>[2];

    // �\�����I������Ă��邩�ǂ���
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

        // ���񃂁[�h
        nowMode = Mode.None;
        nextMode = Mode.Start;

        // �{�^���̃N���b�N�C�x���g�Ƀ��\�b�h��ǉ�
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

        // ���[�h�ύX
        if (Mode.None != nextMode)
        {
            nowMode = nextMode;
            nextMode = Mode.None;
        }

        // �I�����ꂽ�J�[�h�̖��O��\��
        selectedText1.text = selectedCardTexts[nowPlayer].Count > 0 ? selectedCardTexts[nowPlayer][0] : "���I��";
        selectedText2.text = selectedCardTexts[nowPlayer].Count > 1 ? selectedCardTexts[nowPlayer][1] : "���I��";
        selectedText3.text = selectedCardTexts[nowPlayer].Count > 2 ? selectedCardTexts[nowPlayer][2] : "���I��";

        // �\�����I������Ă���ꍇ�͗\���{�^���𖳌���
        predictionButton.interactable = !isPredictionSelected;
    }

    private void startMode()
    {
        drawPanel.SetActive(true);
        nextMode = Mode.Select;
    }

    private void selectMode()
    {
        // �֋X��CPU�̂ݑz��
        // TODO: PvP�̋@�\�������
        if (selectedCards[nowPlayer].Count >= 3)
        {
            // ����̑I�����X�g�Ƀ����_���Ȓl����
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
        // ����̑I�������J�[�h���X�g�Ǝ������I�������J�[�h���X�g���r����
        for (int i = 0; i < selectedCards[0].Count; i++)
        {
            // ����̑I�������ԍ��������J�[�h�^�C�v�Ǝ��g�̗\�������J�[�h�^�C�v�������ꍇ
            if (((selectedCards[1][i] == CardType.PSoldier && selectedCards[0][i] == CardType.Soldier)
                || (selectedCards[1][i] == CardType.PSacrifice && selectedCards[0][i] == CardType.Sacrifice)
                || (selectedCards[1][i] == CardType.PMonster && selectedCards[0][i] == CardType.Monster)))
            {
                // ���̃��X�g�̑���Ɠ����ԍ��̑���̃J�[�h�^�C�v��None�ɂ��Ď��g�̃J�[�h�^�C�v��Monster�ɂ���
                selectedCards[0][i] = CardType.None;
                selectedCards[1][i] = CardType.Monster;
            }
            else if (((selectedCards[0][i] == CardType.PSoldier && selectedCards[1][i] == CardType.Soldier)
                || (selectedCards[0][i] == CardType.PSacrifice && selectedCards[1][i] == CardType.Sacrifice)
                || (selectedCards[0][i] == CardType.PMonster && selectedCards[1][i] == CardType.Monster)))
            {
                // ���̃��X�g�̑���Ɠ����ԍ��̑���̃J�[�h�^�C�v��None�ɂ��Ď��g�̃J�[�h�^�C�v��Monster�ɂ���
                selectedCards[1][i] = CardType.None;
                selectedCards[0][i] = CardType.Monster;
            }
        }

        // ��r���I�������A���X�g����CardType�����o���āA����ɉ�����drawUnit��3��i���X�g�̗v�f���j�Ăяo��
        for (int i =  0; i < selectedCards.Length; i++)
        {
            for (int j = 0; j < selectedCards[nowPlayer].Count; j++)
            {
                CardType cardType = selectedCards[i][j];

                if (cardType == CardType.Monster && sacrificeNum >= 2)
                {
                    sacrificeNum--;
                    sacrificeNumText.text = "���т̐�:" + sacrificeNum;
                }
                else if (cardType == CardType.Monster && sacrificeNum < 2)
                {
                    cardType = CardType.None;
                    Debug.Log("���т�����Ȃ�...");
                }
                else if (cardType == CardType.Sacrifice)
                {
                    sacrificeNum++;
                    sacrificeNumText.text = "���т̐�:" + sacrificeNum;
                }
                gameSceneDirector.drawUnit(i, cardType);
            }
        }

        // ���̃��[�h��
        nextMode = Mode.End;
    }

    private void endMode()
    {
        // �����ɏI�����̏���������
        drawPanel.SetActive(false);
        gameSceneDirector.turnCount++;
        nowMode = Mode.None;
        nextMode = Mode.Start;

        // ���X�g���N���A
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
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "���m?" : "���m");
            isPredictionSelected = false;
        }
    }

    private void addSacrifice()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            selectedCards[nowPlayer].Add(isPredictionSelected ? CardType.PSacrifice : CardType.Sacrifice);
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "����?" : "����");
            isPredictionSelected = false;
        }
    }

    private void addMonster()
    {
        if (selectedCards[nowPlayer].Count < 3)
        {
            selectedCards[nowPlayer].Add(isPredictionSelected ? CardType.PMonster : CardType.Monster);
            selectedCardTexts[nowPlayer].Add(isPredictionSelected ? "����?" : "����");
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
