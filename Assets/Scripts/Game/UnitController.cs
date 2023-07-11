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

    // ���j�b�g�I��/��I����y���W
    public const float SelectUnitY = 1.5f;
    public const float UnSelectUnitY = 0.7f;

    // �u���Ă���ꏊ
    public Vector2Int Pos;

    // �I�������O��y���W
    float oldPosY;


    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

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

    // �ړ��\�͈͎擾
    public List<Vector2Int> GetMovableTiles(UnitController[,] units, bool checkotherunit = true)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        // ��������
        if (FieldStatus.Captured == FieldStatus)
        {
            foreach (var checkpos in getEmptyTiles(units))
            {
                // �ړ��\
                bool ismovable = true;

                // �ړ�������Ԃ����
                Pos = checkpos;
                FieldStatus = FieldStatus.OnBard;

                // �����ȊO���Ȃ��t�B�[���h������āA�u�������ƈړ��ł��Ȃ��Ȃ�ړ��s��
                UnitController[,] exunits = new UnitController[units.GetLength(0), units.GetLength(1)];
                exunits[checkpos.x, checkpos.y] = this;

                if (1 > getMovableTiles(exunits, UnitType).Count)
                {
                    ismovable = false;
                }

                // ��
                if (UnitType.Hu == UnitType)
                {
                    // ���
                    for (int i = 0; i < units.GetLength(1); i++)
                    {
                        if (units[checkpos.x, i]
                            && UnitType.Hu == units[checkpos.x, i].UnitType
                            && Player == units[checkpos.x, i].Player)
                        {
                            ismovable = false;
                            break;
                        }
                    }

                    // �ł����l��
                    int nextplayer = GameSceneDirector.GetNextPlayer(Player);

                    // ����ł������Ƃɂ��āA����ɂȂ�ꍇ
                    UnitController[,] copyunits = GameSceneDirector.GetCopyArray(units);
                    copyunits[checkpos.x, checkpos.y] = this;
                    int outecount = GameSceneDirector.GetOuteUnits(copyunits, nextplayer, false).Count;

                    if (0 < outecount && ismovable)
                    {
                        // �ł����l�ߏ�Ԃɂ��Ă���
                        ismovable = false;
                        // ����̂����ꂩ�̋�����������Ԃ��Č�
                        foreach (var unit in units)
                        {
                            if (!unit || nextplayer != unit.Player) continue;
                            // �ړ��͈͂�checkpos���Ȃ�
                            if (!unit.GetMovableTiles(copyunits).Contains(checkpos)) continue;
                            // ����̋���ړ���������Ԃ����
                            copyunits[checkpos.x, checkpos.y] = unit;
                            // �ēx���肳��Ă��邩�`�F�b�N
                            outecount = GameSceneDirector.GetOuteUnits(copyunits, nextplayer, false).Count;
                            // 1�ł����������ł�������Αł����l�߂���Ȃ�
                            if (1 > outecount)
                            {
                                ismovable = true;
                            }
                        }
                    }

                }

                // �ړ��s�@���̏ꏊ�𒲂ׂ�
                if (!ismovable) continue;

                ret.Add(checkpos);
            }

            // �ړ���Ԃ����Ƃɖ߂�
            Pos = new Vector2Int(-1, -1);
            FieldStatus = FieldStatus.Captured;
        }
        // ��
        else if (UnitType.Gyoku == UnitType)
        {
            ret = getMovableTiles(units, UnitType.Gyoku);

            // ����̈ړ��͈͂��l�����Ȃ�
            if (!checkotherunit) return ret;

            // �폜�Ώۂ̃^�C���i�G�̈ړ��͈́j
            List<Vector2Int> removetiles = new List<Vector2Int>();

            foreach (var item in ret)
            {
                // �ړ�������Ԃ�����ĉ��肳��Ă���Ȃ�폜�Ώ�
                UnitController[,] copyunits = GameSceneDirector.GetCopyArray(units);
                // ������ꏊ����ړ�������Ԃɂ���
                copyunits[Pos.x, Pos.y] = null;
                copyunits[item.x, item.y] = this;

                // ���肵�Ă��郆�j�b�g��
                int outecount = GameSceneDirector.GetOuteUnits(copyunits, Player, false).Count;
                if (0 < outecount) removetiles.Add(item);
            }

            // ���Ŏ擾�����^�C�������O����
            foreach (var item in removetiles)
            {
                ret.Remove(item);
            }
        }
        // ���Ɠ�������
        else if (UnitType.Tokin == UnitType
            || UnitType.NariKyo == UnitType
            || UnitType.NariKei == UnitType
            || UnitType.NariGin == UnitType)
        {
            ret = getMovableTiles(units, UnitType.Kin);
        }
        // �n(��+�p)
        else if (UnitType.Uma == UnitType)
        {
            ret = getMovableTiles(units, UnitType.Gyoku);
            foreach (var item in getMovableTiles(units, UnitType.Kaku))
            {
                if (!ret.Contains(item)) ret.Add(item);
            }
        }
        // ��(��+���)
        else if (UnitType.Ryu == UnitType)
        {
            ret = getMovableTiles(units, UnitType.Gyoku);
            foreach (var item in getMovableTiles(units, UnitType.Hisha))
            {
                if (!ret.Contains(item)) ret.Add(item);
            }
        }
        else
        {
            ret = getMovableTiles(units, UnitType);
        }


        return ret;
    }

    // ���ƂƂȂ�ړ��\�͈͎擾
    List<Vector2Int> getMovableTiles(UnitController[,] units, UnitType unittype)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        // ��
        if (UnitType.Hu == unittype)
        {
            // ����
            int dir = (0 == Player) ? 1 : -1;

            // �O��1�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, 1 * dir)
            };

            // ���ۂ̃t�B�[���h�𒲂ׂ�
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
        // �j�n
        else if (UnitType.Keima == unittype)
        {
            // ����
            int dir = (0 == Player) ? 1 : -1;

            // �O���΂�2�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 1, 2 * dir),
                new Vector2Int(-1, 2 * dir),
            };

            // ���ۂ̃t�B�[���h�𒲂ׂ�
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
        // ��
        else if (UnitType.Gin == unittype)
        {
            // ����
            int dir = (0 == Player) ? 1 : -1;

            // �O���΂�2�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 0, 1 * dir),
                new Vector2Int(-1, 1 * dir),
                new Vector2Int( 1, 1 * dir),
                new Vector2Int(-1,-1 * dir),
                new Vector2Int( 1,-1 * dir),
            };

            // ���ۂ̃t�B�[���h�𒲂ׂ�
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
        // ��
        else if (UnitType.Kin == unittype)
        {
            // ����
            int dir = (0 == Player) ? 1 : -1;

            // �O���΂�2�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1, 1 * dir),
                new Vector2Int( 0, 1 * dir),
                new Vector2Int( 1, 1 * dir),
                new Vector2Int( 1, 0 * dir),
                new Vector2Int( 0,-1 * dir),
                new Vector2Int(-1, 0 * dir),
            };

            // ���ۂ̃t�B�[���h�𒲂ׂ�
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
        // ��
        else if (UnitType.Gyoku == unittype)
        {
            // ����1�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1, 1),
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1),
                new Vector2Int( 1, 0),
                new Vector2Int( 1,-1),
                new Vector2Int( 0,-1),
                new Vector2Int(-1,-1),
                new Vector2Int(-1, 0),
            };

            // ���ۂ̃t�B�[���h�𒲂ׂ�
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
        // �p
        else if (UnitType.Kaku == unittype)
        {
            // �ǂ⑼�̋�ƂԂ���܂łǂ��܂ł��i�߂�
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int( 1,-1),
                new Vector2Int(-1,-1),
            };

            foreach (var item in vec)
            {
                Vector2Int checkpos = Pos + item;
                while (isCheckable(units, checkpos))
                {
                    // ���̋�������ꍇ
                    if (units[checkpos.x, checkpos.y])
                    {
                        // ����̃��j�b�g�̏ꏊ�ւ͈ړ��\
                        if (Player != units[checkpos.x, checkpos.y].Player)
                        {
                            ret.Add(checkpos);
                        }
                        break;
                    }

                    ret.Add(checkpos);
                    checkpos += item;
                }
            }
        }
        // ���
        else if (UnitType.Hisha == unittype)
        {
            // �ǂ⑼�̋�ƂԂ���܂łǂ��܂ł��i�߂�
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int( 0, 1),
                new Vector2Int( 0,-1),
                new Vector2Int( 1, 0),
                new Vector2Int(-1, 0),
            };

            foreach (var item in vec)
            {
                Vector2Int checkpos = Pos + item;
                while (isCheckable(units, checkpos))
                {
                    // ���̋�������ꍇ
                    if (units[checkpos.x, checkpos.y])
                    {
                        // ����̃��j�b�g�̏ꏊ�ւ͈ړ��\
                        if (Player != units[checkpos.x, checkpos.y].Player)
                        {
                            ret.Add(checkpos);
                        }
                        break;
                    }

                    ret.Add(checkpos);
                    checkpos += item;
                }
            }
        }
        // ����
        else if (UnitType.Kyousha == unittype)
        {
            // ����
            int dir = (0 == Player) ? 1 : -1;

            // �O��1�}�X
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, 1 * dir)
            };

            foreach (var item in vec)
            {
                Vector2Int checkpos = Pos + item;
                while (isCheckable(units, checkpos))
                {
                    // ���̋�������ꍇ
                    if (units[checkpos.x, checkpos.y])
                    {
                        // ����̃��j�b�g�̏ꏊ�ւ͈ړ��\
                        if (Player != units[checkpos.x, checkpos.y].Player)
                        {
                            ret.Add(checkpos);
                        }
                        break;
                    }

                    ret.Add(checkpos);
                    checkpos += item;
                }
            }
        }

        return ret;
    }

    // �z��I�[�o�[���ǂ���
    bool isCheckable(UnitController[,] ary, Vector2Int idx)
    {
        // �z��I�[�o�[�̏��
        if (idx.x < 0 || ary.GetLength(0) <= idx.x
            || idx.y < 0 || ary.GetLength(1) <= idx.y)
        {
            return false;
        }
        return true;
    }

    // ���Ԃ̃��j�b�g���ǂ���
    bool isFriendlyUnit(UnitController unit)
    {
        if (unit && Player == unit.Player) return true;
        return false;
    }

    // �L���v�`�����ꂽ��
    public void Capture(int player)
    {
        Player = player;
        FieldStatus = FieldStatus.Captured;
        Evolution(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // ��
    public void Evolution(bool evolution = true)
    {
        Vector3 angle = transform.eulerAngles;

        // ��
        if (evolution && UnitType.None != evolutionTable[UnitType])
        {
            UnitType = evolutionTable[UnitType];
            angle.x = 270;
            angle.y = (0 == Player) ? 180 : 0;
            angle.z = 0;
            transform.eulerAngles = angle;
        }
        else
        {
            UnitType = OldUnitType;
            transform.eulerAngles = getDefaultAngles(Player);
        }
    }

    // �󂢂Ă���^�C����Ԃ�
    List<Vector2Int> getEmptyTiles(UnitController[,] units)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        for (int i = 0; i < units.GetLength(0); i++)
        {
            for (int j = 0; j < units.GetLength(1); j++)
            {
                if (units[i, j]) continue;
                ret.Add(new Vector2Int(i, j));
            }
        }
        return ret;
    }

    // �i���ł��邩�ǂ���
    public bool isEvolution()
    {
        if (!evolutionTable.ContainsKey(UnitType) || UnitType.None == evolutionTable[UnitType])
        {
            return false;
        }
        return true;
    }
}
