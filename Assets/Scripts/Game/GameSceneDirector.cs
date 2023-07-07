using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneDirector : MonoBehaviour
{
    // UI�֘A
    [SerializeField] Text textTurnInfo;
    [SerializeField] Text textResultInfo;
    [SerializeField] Button buttonTitle;
    [SerializeField] Button buttonRematch;
    [SerializeField] Button buttonEvolutionApply;
    [SerializeField] Button buttonEvolutionCancel;

    // �Q�[���ݒ�
    const int PlayerMax = 2;
    int boardWidth;
    int boardHeight;

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

    // �t�B�[���h�f�[�^
    Dictionary<Vector2Int, GameObject> tiles;
    UnitController[,] units;

    // ���ݑI�𒆂̃��j�b�g
    UnitController selectUnit;

    // �ړ��\�͈�
    Dictionary<GameObject, Vector2Int> movableTiles;

    // Start is called before the first frame update
    void Start()
    {
        // UI�֘A�����ݒ�
        buttonTitle.gameObject.SetActive(false);
        buttonRematch.gameObject.SetActive(false);
        buttonEvolutionApply.gameObject.SetActive(false);
        buttonEvolutionCancel.gameObject.SetActive(false);
        textResultInfo.text = "";

        // �{�[�h�T�C�Y
        boardWidth = boardSetting.GetLength(0);
        boardHeight = boardSetting.GetLength(1);

        // �t�B�[���h������
        tiles = new Dictionary<Vector2Int, GameObject>();
        units = new UnitController[boardWidth, boardHeight];

        // �ړ��\�͈�
        movableTiles = new Dictionary<GameObject, Vector2Int>();

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
                tiles.Add(tileindex, tile);

                //TODO// �ړ��\�͈͂����Őݒ�
                //movableTiles.Add(tile, tileindex);

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
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // ��O�̃��j�b�g�ɂ������蔻�肪����̂Ńq�b�g�����S�ẴI�u�W�F�N�g�����擾
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

        // �Ȃɂ��I������Ă��Ȃ���Ώ��������Ȃ�
        if (null == tile && null == unit) return;

        // �ړ���I��
        if (tile && selectUnit && movableTiles.ContainsKey(tile))
        {
            moveUnit(selectUnit, movableTiles[tile]);
            selectUnit = null;
        }
        // ���j�b�g�I��
        else if (unit)
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

        List<Vector2Int> movabletiles = getMovableTiles(unit);
        movableTiles.Clear();

        foreach (var item in movabletiles)
        {
            movableTiles.Add(tiles[item], item);
        }


        // TODO: �ړ��͈͂̕\��

        // �I�����
        if (playerunit)
        {
            unit.Select();
            selectUnit = unit;
        }
    }

    // ���j�b�g�ړ�
    void moveUnit(UnitController unit, Vector2Int tileindex)
    {
        // ���ݒn
        Vector2Int oldpos = unit.Pos;

        // ���j�b�g�ړ�
        unit.Move(tiles[tileindex], tileindex);

        // �����f�[�^�X�V(�V�����ꏊ)
        units[tileindex.x, tileindex.y] = unit;

        // �{�[�h��̋���X�V
        if (FieldStatus.OnBard == unit.FieldStatus)
        {
            // �����f�[�^�X�V
            units[oldpos.x, oldpos.y] = null;
        }
        // ������̍X�V
        else
        {
            // TODO ������̍X�V
        }

        // ���j�b�g�̏�Ԃ��X�V
        unit.FieldStatus = FieldStatus.OnBard;
    }

    // �ړ��\�͈͎擾
    List<Vector2Int> getMovableTiles(UnitController unit)
    {
        List<Vector2Int> ret = unit.GetMovableTiles(units);

        return ret;
    }
}
