using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��̃^�C�v
public enum UnitType
{
    None = -1,
    Hu = 1,
    Kaku,
    Hisha,
    Kyousha,
    Keima,
    Gin,
    Kin,
    Gyoku,
    // ��
    Tokin,
    Uma,
    Ryu,
    NariKyo,
    NariKei,
    NariGin,
}

// ��̏ꏊ
public enum FieldStatus
{
    OnBard,
    Captured,
}

public class UnitController : MonoBehaviour
{
    // ���j�b�g�̃v���C���[�ԍ�
    public int Player;
    // ���j�b�g�̎��
    public UnitType UnitType, OldUnitType;
    // ���j�b�g�̏ꏊ
    public FieldStatus FieldStatus;

    // ���e�[�u��
    Dictionary<UnitType, UnitType> evolutionTable = new Dictionary<UnitType, UnitType>()
    {
        {UnitType.Hu, UnitType.Tokin },
        {UnitType.Kaku, UnitType.Uma },
        {UnitType.Hisha, UnitType.Ryu },
        {UnitType.Kyousha, UnitType.NariKyo },
        {UnitType.Keima, UnitType.NariKei },
        {UnitType.Gin, UnitType.NariGin },
        {UnitType.Kin, UnitType.None },
        {UnitType.Gyoku, UnitType.None },
    };

    // ���ς݂��ǂ���
    public bool isEvolution;

    // ���j�b�g�I��/��I����y���W
    public const float SelectUnitY = 1.5f;
    public const float UnSelectUnitY = 0.7f;

    // �u���Ă���ꏊ
    public Vector2Int Pos;

    // �I�������O��y���W
    float oldPosY;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // �����ݒ�
    public void Init(int player, int unittype, GameObject tile, Vector2Int pos)
    {
        Player = player;
        UnitType = (UnitType)unittype;
        // ���ꂽ�����ɖ߂�悤
        OldUnitType = (UnitType)unittype;
        // �ꏊ�̏����l
        FieldStatus = FieldStatus.OnBard;
        // �p�x�Əꏊ
        transform.eulerAngles = getDefaultAngles(player);
        Move(tile, pos);
    }

    // �w�肳�ꂽ�v���C���[�ԍ��̌�����Ԃ�
    Vector3 getDefaultAngles(int player)
    {
        return new Vector3(90, player * 180, 0);
    }

    // �ړ�����
    public void Move(GameObject tile, Vector2Int tileindex)
    {
        // �V�����ꏊ�ֈړ�����
        Vector3 pos = tile.transform.position;
        pos.y = UnSelectUnitY;
        transform.position = pos;
        // �C���f�b�N�X�X�V
        Pos = tileindex;
    }

    // �I�����̏���
    public void Select(bool select = true)
    {
        Vector3 pos = transform.position;
        bool iskinematic = select;

        if (select)
        {
            oldPosY = pos.y;
            pos.y = SelectUnitY;
        }
        else
        {
            pos.y = UnSelectUnitY;

            // ������̈ʒu�͓���
            if (FieldStatus.Captured == FieldStatus)
            {
                pos.y = oldPosY;
                iskinematic = true;
            }
        }

        GetComponent<Rigidbody>().isKinematic = iskinematic;
        transform.position = pos;
    }

    public List<Vector2Int> GetMovableTiles(UnitController[,] units, bool checkotherunit = true)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        ret = getMovableTiles(units, UnitType.Hu);

        return ret;
    }

    List<Vector2Int> getMovableTiles(UnitController[,] units, UnitType unittype)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        if (unittype == UnitType.Hu)
        {
            int dir = (Player == 0) ? 1 : -1;

            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, 1* dir)
            };

            foreach (var item in vec)
            {
                Vector2Int checkpos = Pos + item;
                if (!isCheckable(units, checkpos) || isFriendlyUnit(units[checkpos.x, checkpos.y]))
                {
                    continue;
                }

                ret.Add(checkpos);
            }
        }

        return ret;

        bool isCheckable(UnitController[,] ary, Vector2Int idx)
        {
            if(idx.x < 0 || ary.GetLength(0) <= idx.x || idx.y < 0 || ary.GetLength(0) <= idx.y)
            {
                return false;
            }
            return true;
        }

        bool isFriendlyUnit(UnitController unit)
        {
            if (unit && Player == unit.Player) return true;
            return false;
        }
    }
}
