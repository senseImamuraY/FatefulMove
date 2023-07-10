using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneDirector : MonoBehaviour
{
    // �^�C���̃v���n�u
    [SerializeField] GameObject prefabTile;

    // ���j�b�g�̃v���n�u
    [SerializeField] List<GameObject> prefabUnits;

    // �����z�u
    int[,] boardSetting =
    {
        { 4, 0, 1, 0, 0, 0, 11, 0, 14 },
        { 5, 2, 1, 0, 0, 0, 11,13, 15 },
        { 6, 0, 1, 0, 0, 0, 11, 0, 16 },
        { 7, 0, 1, 0, 0, 0, 11, 0, 17 },
        { 8, 0, 1, 0, 0, 0, 11, 0, 18 },
        { 7, 0, 1, 0, 0, 0, 11, 0, 17 },
        { 6, 0, 1, 0, 0, 0, 11, 0, 16 },
        { 5, 3, 1, 0, 0, 0, 11,12, 15 },
        { 4, 0, 1, 0, 0, 0, 11, 0, 14 },
    };

    // Start is called before the first frame update
    void Start()
    {
        // �{�[�h�T�C�Y
        int boardWidth = boardSetting.GetLength(0);
        int boardHeight = boardSetting.GetLength(1);

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // �^�C���ƃ��j�b�g�̃|�W�V����
                float x = i - boardWidth / 2;
                float y = j - boardHeight / 2;

                // �|�W�V����
                Vector3 pos = new Vector3(x, 0, y);

                // �^�C���̃C���f�b�N�X
                Vector2Int tileindex = new Vector2Int(i, j);

                // �^�C���쐬
                GameObject tile = Instantiate(prefabTile, pos, Quaternion.identity);

                // ���j�b�g�쐬
                int type = boardSetting[i, j] % 10;
                int player = boardSetting[i, j] / 10;

                if (0 == type) continue;

                // ������
                pos.y = 0.7f;

                GameObject prefab = prefabUnits[type - 1];
                GameObject unit = Instantiate(prefab, pos, Quaternion.Euler(90, player * 180, 0));
                unit.AddComponent<Rigidbody>();

                UnitController unitctrl = unit.AddComponent<UnitController>();
                unitctrl.Init(player, type, tile, tileindex);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPvP()
    {
        GameSceneDirector.PlayerCount = 2;
        SceneManager.LoadScene("Demo");
    }
    
    public void OnClickPvE()
    {
        GameSceneDirector.PlayerCount = 1;
        SceneManager.LoadScene("Demo");
    } 

    public void OnClickEvE()
    {
        GameSceneDirector.PlayerCount = 0;
        SceneManager.LoadScene("Demo");
    }
}
