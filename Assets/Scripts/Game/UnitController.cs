using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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
        // ���ꂽ�Ƃ��Ɍ��ɖ߂����߂Ɏg��
        OldUnitType = (UnitType)unittype;
        FieldStatus = FieldStatus.OnBard;

        transform.eulerAngles = getDefaultAngles(player);
        Move(tile, pos);
    }

    Vector3 getDefaultAngles(int player)
    {
        return new Vector3(90, player * 180, 0);
    }

    public void Move(GameObject tile, Vector2Int tileindex)
    {
        // �V�����ꏊ�ֈړ�����
        Vector3 pos = tile.transform.position;
        pos.y = UnSelectUnitY;
        transform.position = pos;

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
}
