using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneDirector : MonoBehaviour
{
    [SerializeField]
    Text textTrunInfo, textResultInfo;

    [SerializeField]
    Button buttonTitle, buttonRematch, buttonEvolutionApply, buttonEvolutionCancel;

    // �Q�[���ݒ�
    const int PlayerMax = 2;
    int boardWidth;
    int boardHeight;

    [SerializeField]
    GameObject prefabTile;

    [SerializeField]
    List<GameObject> prefabUnits;

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

    Dictionary<Vector2Int, GameObject> tiles;
    UnitController[,] units;

    UnitController selectUnit;

    // Start is called before the first frame update
    void Start()
    {
        buttonTitle.gameObject.SetActive(false);
        buttonRematch.gameObject.SetActive(false);
        buttonEvolutionApply.gameObject.SetActive(false);
        buttonEvolutionCancel.gameObject.SetActive(false);
        textResultInfo.text = "";

        // �{�[�h�T�C�Y
        boardWidth = boardSetting.GetLength(0);
        boardHeight = boardSetting.GetLength(1);

        //�@�t�B�[���h������
        tiles = new Dictionary<Vector2Int, GameObject>();
        units = new UnitController[boardWidth, boardHeight];

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                float x = i - boardWidth / 2;
                float y = j - boardHeight / 2;

                Vector3 pos = new Vector3(x, 0, y);

                Vector2Int tileindex = new Vector2Int(i, j);

                GameObject tile = Instantiate(prefabTile, pos, Quaternion.identity);
                tiles.Add(tileindex, tile);

                // ���j�b�g�쐬
                int type = boardSetting[i, j] % 10;
                int player = boardSetting[i, j] / 10;

                if (0 == type) continue;

                pos.y = 0.7f;

                GameObject prefab = prefabUnits[type - 1];
                GameObject unit = Instantiate(prefab, pos, Quaternion.Euler(90, player * 180, 0));
                unit.AddComponent<Rigidbody>();

                UnitController unitctrl = unit.AddComponent<UnitController>();
                unitctrl.Init(player, type, tile, tileindex);

                // ���j�b�g�f�[�^�Z�b�g
                units[i, j] = unitctrl;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject tile = null;
        UnitController unit = null;

        // �v���C���[����
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                UnitController hitunit = hit.transform.GetComponent<UnitController>();

                // ������
                if (hitunit && FieldStatus.Captured == hitunit.FieldStatus)
                {
                    unit = hitunit;
                }
                // �^�C���I���Ə�ɏ���Ă��郆�j�b�g
                else if (tiles.ContainsValue(hit.transform.gameObject))
                {
                    tile = hit.transform.gameObject;
                    // �^�C�����烆�j�b�g��T��
                    foreach (var item in tiles)
                    {
                        if (item.Value == tile)
                        {
                            unit = units[item.Key.x, item.Key.y];
                        }
                    }
                    break;
                }
            }
        }

        // �Ȃɂ��I�����Ȃ���Ώ������Ȃ�
        if (tile == null && null == unit) return;

        // ���j�b�g�I��
        if(unit)
        {
            setSelectCursors(unit);
        }
    }

    // �I����
    void setSelectCursors(UnitController unit = null, bool playerunit = true)
    {
        // �I�����j�b�g�̔�I�����
        if (selectUnit)
        {
            selectUnit.Select(false);
            selectUnit = null;
        }
    }
}
