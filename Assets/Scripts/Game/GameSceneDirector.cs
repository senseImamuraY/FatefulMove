using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneDirector : MonoBehaviour
{
    // UI関連
    [SerializeField] Text textTurnInfo;
    [SerializeField] Text textResultInfo;
    [SerializeField] Button buttonTitle;
    [SerializeField] Button buttonRematch;
    [SerializeField] Button buttonEvolutionApply;
    [SerializeField] Button buttonEvolutionCancel;

    // ゲーム設定
    const int PlayerMax = 2;
    int boardWidth;
    int boardHeight;

    // タイルのプレハブ
    [SerializeField] GameObject prefabTile;

    // ユニットのプレハブ
    [SerializeField] List<GameObject> prefabUnits;

    // 初期配置
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

    // フィールドデータ
    Dictionary<Vector2Int, GameObject> tiles;
    UnitController[,] units;

    // 現在選択中のユニット
    UnitController selectUnit;

    // 移動可能範囲
    Dictionary<GameObject, Vector2Int> movableTiles;

    // Start is called before the first frame update
    void Start()
    {
        // UI関連初期設定
        buttonTitle.gameObject.SetActive(false);
        buttonRematch.gameObject.SetActive(false);
        buttonEvolutionApply.gameObject.SetActive(false);
        buttonEvolutionCancel.gameObject.SetActive(false);
        textResultInfo.text = "";

        // ボードサイズ
        boardWidth = boardSetting.GetLength(0);
        boardHeight = boardSetting.GetLength(1);

        // フィールド初期化
        tiles = new Dictionary<Vector2Int, GameObject>();
        units = new UnitController[boardWidth, boardHeight];

        // 移動可能範囲
        movableTiles = new Dictionary<GameObject, Vector2Int>();

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                // タイルとユニットのポジション
                float x = i - boardWidth / 2;
                float y = j - boardHeight / 2;

                // ポジション
                Vector3 pos = new Vector3(x, 0, y);

                // タイルのインデックス
                Vector2Int tileindex = new Vector2Int(i, j);

                // タイル作成
                GameObject tile = Instantiate(prefabTile, pos, Quaternion.identity);
                tiles.Add(tileindex, tile);

                //TODO// 移動可能範囲を仮で設定
                //movableTiles.Add(tile, tileindex);

                // ユニット作成
                int type = boardSetting[i, j] % 10;
                int player = boardSetting[i, j] / 10;

                if (0 == type) continue;

                // 初期化
                pos.y = 0.7f;

                GameObject prefab = prefabUnits[type - 1];
                GameObject unit = Instantiate(prefab, pos, Quaternion.Euler(90, player * 180, 0));
                unit.AddComponent<Rigidbody>();

                UnitController unitctrl = unit.AddComponent<UnitController>();
                unitctrl.Init(player, type, tile, tileindex);

                // ユニットデータセット
                units[i, j] = unitctrl;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject tile = null;
        UnitController unit = null;

        // プレイヤー処理
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 手前のユニットにも当たり判定があるのでヒットした全てのオブジェクト情報を取得
            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                UnitController hitunit = hit.transform.GetComponent<UnitController>();

                // 持ち駒
                if (hitunit && FieldStatus.Captured == hitunit.FieldStatus)
                {
                    unit = hitunit;
                }
                // タイル選択と上に乗っているユニット
                else if (tiles.ContainsValue(hit.transform.gameObject))
                {
                    tile = hit.transform.gameObject;
                    // タイルからユニットを探す
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

        // なにも選択されていなければ処理をしない
        if (null == tile && null == unit) return;

        // 移動先選択
        if (tile && selectUnit && movableTiles.ContainsKey(tile))
        {
            moveUnit(selectUnit, movableTiles[tile]);
            selectUnit = null;
        }
        // ユニット選択
        else if (unit)
        {
            setSelectCursors(unit);
        }

    }

    // 選択時
    void setSelectCursors(UnitController unit = null, bool playerunit = true)
    {
        // 選択ユニットの非選択状態
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


        // TODO: 移動範囲の表示

        // 選択状態
        if (playerunit)
        {
            unit.Select();
            selectUnit = unit;
        }
    }

    // ユニット移動
    void moveUnit(UnitController unit, Vector2Int tileindex)
    {
        // 現在地
        Vector2Int oldpos = unit.Pos;

        // ユニット移動
        unit.Move(tiles[tileindex], tileindex);

        // 内部データ更新(新しい場所)
        units[tileindex.x, tileindex.y] = unit;

        // ボード上の駒を更新
        if (FieldStatus.OnBard == unit.FieldStatus)
        {
            // 内部データ更新
            units[oldpos.x, oldpos.y] = null;
        }
        // 持ち駒の更新
        else
        {
            // TODO 持ち駒の更新
        }

        // ユニットの状態を更新
        unit.FieldStatus = FieldStatus.OnBard;
    }

    // 移動可能範囲取得
    List<Vector2Int> getMovableTiles(UnitController unit)
    {
        List<Vector2Int> ret = unit.GetMovableTiles(units);

        return ret;
    }
}
