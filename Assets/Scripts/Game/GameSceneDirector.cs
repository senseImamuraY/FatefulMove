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

    // ゲーム設定
    const int PlayerMax = 2;
    int boardWidth;
    int boardHeight;

    [SerializeField]
    GameObject prefabTile;

    [SerializeField]
    List<GameObject> prefabUnits;

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

        // ボードサイズ
        boardWidth = boardSetting.GetLength(0);
        boardHeight = boardSetting.GetLength(1);

        //　フィールド初期化
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

                // ユニット作成
                int type = boardSetting[i, j] % 10;
                int player = boardSetting[i, j] / 10;

                if (0 == type) continue;

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
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
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

        // なにも選択しなければ処理しない
        if (tile == null && null == unit) return;

        // ユニット選択
        if(unit)
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
    }
}
