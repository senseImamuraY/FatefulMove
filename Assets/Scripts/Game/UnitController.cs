using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 駒のタイプ
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
    // 成
    Tokin,
    Uma,
    Ryu,
    NariKyo,
    NariKei,
    NariGin,
}

// 駒の場所
public enum FieldStatus
{
    OnBard,
    Captured,
}

public class UnitController : MonoBehaviour
{
    // ユニットのプレイヤー番号
    public int Player;
    // ユニットの種類
    public UnitType UnitType, OldUnitType;
    // ユニットの場所
    public FieldStatus FieldStatus;

    // 成テーブル
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

    // 成済みかどうか
    public bool isEvolution;

    // ユニット選択/非選択のy座標
    public const float SelectUnitY = 1.5f;
    public const float UnSelectUnitY = 0.7f;

    // 置いている場所
    public Vector2Int Pos;

    // 選択される前のy座標
    float oldPosY;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初期設定
    public void Init(int player, int unittype, GameObject tile, Vector2Int pos)
    {
        Player = player;
        UnitType = (UnitType)unittype;
        // 取られた時元に戻るよう
        OldUnitType = (UnitType)unittype;
        // 場所の初期値
        FieldStatus = FieldStatus.OnBard;
        // 角度と場所
        transform.eulerAngles = getDefaultAngles(player);
        Move(tile, pos);
    }

    // 指定されたプレイヤー番号の向きを返す
    Vector3 getDefaultAngles(int player)
    {
        return new Vector3(90, player * 180, 0);
    }

    // 移動処理
    public void Move(GameObject tile, Vector2Int tileindex)
    {
        // 新しい場所へ移動する
        Vector3 pos = tile.transform.position;
        pos.y = UnSelectUnitY;
        transform.position = pos;
        // インデックス更新
        Pos = tileindex;
    }

    // 選択時の処理
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

            // 持ち駒の位置は特別
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
