
void Memo()
{ 
//▼Awakeのインスタンス生成と初期化処理--------------------------------------------------------------------------------------------
 void Awake()
{
    // 他のクラスからプレイヤーを参照できるようにstatic 変数にインスタンス情報を格納する
    m_instance = this;

    Init();

}

//初期化
 void Init()
{
    //位置など
    transform.localPosition = DefPosition;
    transform.localEulerAngles = DefRotation;

    //m_hp = m_hpMax; // HP

    //m_level = 1; // レベル
    //m_exp = 0;
    //m_needExp = GetNeedExp(1); // 次のレベルに必要な経験値


    //ゲームオーバーを通知
    _gameMgr.StartGameOver();
    //または
    GM_Main.m_instance.GameResult();
}

    //コンポーネントなどの取得
    void Start()
{
    // 物理挙動コンポーネントを取得
    _rigidbody = GetComponent<Rigidbody2D>();
    // スプライト描画コンポーネントを取得
    _renderer = GetComponent<SpriteRenderer>();
    // ③ゲーム管理スクリプトを取得
    _gameMgr = gameMgr.GetComponent<GameMgr>();
}

//▼Transformを使った移動--------------------------------------------------------------------------------------------
    // ゲームを 60 FPS 固定にする
    Application.targetFrameRate = 60;

    // 矢印キーの入力情報を取得する
    var h = Input.GetAxisRaw("Horizontal");
    var v = Input.GetAxisRaw("Vertical");

    // 矢印キーが押されている方向にプレイヤーを移動する
    // 移動方向を示すベクトル
    var moveDirection = new Vector3(h, v);

    // ベクトルを正規化
    moveDirection.Normalize();

    var velocity = moveDirection * m_speed;
    transform.localPosition += velocity;

    // プレイヤーのスクリーン座標を計算する
    var screenPos = characterCamera.WorldToScreenPoint(transform.position);


    if (h < 0)
    {

        playerRenderer.flipX = false;

    }
    else if (h > 0)
    {
        playerRenderer.flipX = true;

    }

//▼Transformを使った移動　ワープ--------------------------------------------------------------------------------------------
    // 左矢印が押された時
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
        transform.Translate(-3, 0, 0); // 左に「3」動かす
    }

    // 右矢印が押された時
    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
        transform.Translate(3, 0, 0); // 右に「3」動かす
    }




//▼Addforceを使った移動--------------------------------------------------------------------------------------------
    // ジャンプする          
    if (Input.GetKeyDown(KeyCode.Space) /*&& this.rigid2D.velocity.y == 0*/)
    {
        //this.animator.SetTrigger("JumpTrigger");
        //this.rigidbody.AddForce(transform.up * this.jumpForce);
        rb.AddForce(new Vector3(0, upForce, 0));

    }

    // 左右移動
    int key = 0;
    if (Input.GetKey(KeyCode.RightArrow)) key = 1;
    if (Input.GetKey(KeyCode.LeftArrow)) key = -1;

    // プレイヤの速度
    float speedx = Mathf.Abs(this.rigid2D.velocity.x);

    // スピード制限
    if (speedx < this.maxWalkSpeed)
    {
        this.rigid2D.AddForce(transform.right * key * this.walkForce);
    }

    // 動く方向に応じて反転
    if (key != 0)
    {
        transform.localScale = new Vector3(key, 1, 1);
    }

//▼一気に生成＆非アクティブに--------------------------------------------------------------------------------------------
    m_weapons = new Weapon[m_weaponPrefabs.Length]; // 武器のインスタンスを格納する配列を初期化

    // 各武器プレハブに対して m_level を設定し、インスタンスを生成して m_weapons に格納する
    for (int i = 0; i < m_weaponPrefabs.Length; i++)
    {
        m_weaponPrefabs[i].m_level = 0; // レベル

        m_weapons[i] = Instantiate(m_weaponPrefabs[i]);
        m_weapons[i].gameObject.SetActive(false); // 生成した武器を非アクティブにする


    }

//▼ボタンを生成＆配置--------------------------------------------------------------------------------------------
    private void Awake()
    {
        // 他のクラスから参照できるように
        // static 変数にインスタンス情報を格納する
        m_instance = this;

        // タグを指定
        gameObject.tag = MiniGameTagName;

        weaponNum = WeaponManager.m_instance.m_weaponPrefabs.Length;

        powerupButton = new Button[weaponNum];

        for (int i = 0; i < weaponNum; i++)
        {
            CreatePowerupButton(i);

        }
    }

    void OnEnable()
    {
        RepositionButtons();
    }

    public void RepositionButtons()
    {

        for (int i = 0; i < weaponNum; i++)
        {
            powerupButton[i].transform.parent = null;

        }

        GridLayoutGroup gridLayout = panelPrefab.GetComponent<GridLayoutGroup>();
        //float cellSizeX = gridLayout.cellSize.x;
        gridLayout.constraintCount = numBtns;

        //AからB個の連番を取得
        var ary = Enumerable.Range(0, weaponNum).OrderBy(n => Guid.NewGuid()).Take(numBtns).ToArray();


        for (int j = 0; j < ary.Length; j++)
        {

            powerupButton[ary[j]].transform.SetParent(panelPrefab, false);
        }


    }

    void CreatePowerupButton(int i)
    {
        powerupButton[i] = Instantiate(buttonPrefab/*, this.transform*/);
        powerupButton[i].GetComponentInChildren<Text>().text = i.ToString();
        powerupButton[i].onClick.AddListener(() => WeaponManager.m_instance.LevelUp(i));

    }

//▼for文による繰り返し行動--------------------------------------------------------------------------------------------
    for (int i = 0; i < count; ++i)
    {
        // 弾の発射角度を計算する
        var angle = angleBase +
            angleRange * ((float)i / (count - 1) - 0.5f);

        {
            ShootBullet(pos, rot, angle, speed, m_shotPrefab);
        }
    }

//▼ManagerからCanvasを動かす処理のイメージ--------------------------------------------------------------------------------------------

    [SerializeField]
    GameObject powerupCanvasPrefab;
    GameObject powerupCanvas;
    private void Start()
    {
        if (powerupCanvas != null) Destroy(powerupCanvas);
        powerupCanvas = Instantiate(powerupCanvasPrefab);
        powerupCanvas.SetActive(false);

    }

    public void Powerup()
    {
        powerupCanvas.SetActive(true);

    }

    public void PowerupDone()
    {
        powerupCanvas.SetActive(false);

    }

//▼MainCanvasのイメージ--------------------------------------------------------------------------------------------
    public Image m_hpGauge; // HP ゲージ
    public Image m_expGauge; // 経験値ゲージ

    public Text m_levelText;// レベルのテキスト

    public GameObject m_gameOverText; // ゲームオーバーのテキスト

    private void Update()
    {
        if (!GM_Main.m_instance.isGamePlay()) return;

        // プレイヤーを取得する
        var player = Player.m_instance;

        // HP のゲージの表示を更新する
        var hp = player.m_hp;
        var hpMax = player.m_hpMax;
        m_hpGauge.fillAmount = (float)hp / hpMax;

        // レベルのテキストの表示を更新する
        m_levelText.text = player.m_level.ToString();

        // プレイヤーが非表示ならゲームオーバーと表示する
        m_gameOverText.SetActive(!player.gameObject.activeSelf);
}

//▼GAMEOVERを告知するGameManagerのイメージ--------------------------------------------------------------------------------------------

    GameObject car;
    GameObject flag;
    GameObject distance;

    void Start()
    {
    this.car = GameObject.Find("car");
    this.flag = GameObject.Find("flag");
    this.distance = GameObject.Find("Distance");
    }

    void Update()
    {
    float length = this.flag.transform.position.x - this.car.transform.position.x;
    this.distance.GetComponent<Text>().text = "ゴールまで" + length.ToString("F2") + "m";

    if (length <= 0)
    {
    this.distance.GetComponent<Text>().text = "ゲームオーバー";
    }


//▼Gamemanager　Flappy--------------------------------------------------------------------------------------------

// 状態定数
enum State
    {
        Main, // メインゲーム
        GameOver, // ゲームオーバー
    }

    // スコア
    int _score = 0;
    // 状態
    State _state = State.Main;

    // ゲームオーバーの開始
    public void StartGameOver()
    {
        _state = State.GameOver;
    }


    private void FixedUpdate()
    {
        if (_state == State.Main)
        {
            // メインゲーム中のみスコア上昇
            _score += 1;
        }
    }

    private void OnGUI()
    {
        // スコアを描画
        _DrawScore();

        // 画面の中心座標を計算する
        float CenterX = Screen.width / 2;
        float CenterY = Screen.height / 2;
        if (_state == State.GameOver)
        {
            // ゲームオーバーの描画
            _DrawGameOver(CenterX, CenterY);
            // ②リトライボタンの描画
            if (_DrawRetryButton(CenterX, CenterY))
            {
                // ③クリックしたらやり直しする
                SceneManager.LoadScene("Flappy");
            }
        }
    }

    // ④リトライボタンの描画
    bool _DrawRetryButton(float CenterX, float CenterY)
    {
        float ofsY = 40;
        float w = 100;
        float h = 64;
        Rect rect = new Rect(CenterX - w / 2, CenterY + ofsY, w, h);
        if (GUI.Button(rect, "RETRY"))
        {
            // ボタンを押した
            return true;
        }
        return false;
    }

    // ゲームオーバーの描画
    void _DrawGameOver(float CenterX, float CenterY)
    {
        // 中央揃え
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        float w = 400;
        float h = 100;
        Rect position = new Rect(CenterX - w / 2, CenterY - h / 2, w, h);
        GUI.Label(position, "GAME OVER");
    }

    // スコアの描画
    void _DrawScore()
    {
        // 文字を大きくする
        GUI.skin.label.fontSize = 32;
        // 左揃え
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        Rect position = new Rect(8, 8, 400, 100);
        GUI.Label(position, string.Format("score:{0}", _score));
    }
    }

//▼ファクトリー系　シンプル--------------------------------------------------------------------------------------------
        void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span)
        {
            this.delta = 0;
            GameObject go = Instantiate(arrowPrefab);
            int px = Random.Range(-6, 7);
            go.transform.position = new Vector3(px, 7, 0);
        }
    }

//▼ファクトリー系--------------------------------------------------------------------------------------------
   private void Update()
{
    if (!GM_Main.m_instance.isGamePlay()) return;

    // 経過時間を更新する
    m_elapsedTime += Time.deltaTime;

    // 出現タイミングを管理するタイマーを更新する
    m_timer += Time.deltaTime;

    // ゲームの経過時間から出現間隔（秒）を算出する
    // ゲームの経過時間が長くなるほど、敵の出現間隔が短くなる
    var t = m_elapsedTime / m_elapsedTimeMax;
    var interval = Mathf.Lerp(m_intervalFrom, m_intervalTo, t);

    // まだ敵が出現するタイミングではない場合、
    // このフレームの処理はここで終える
    if (m_timer < interval) return;

    // 出現タイミングを管理するタイマーをリセットする
    m_timer = 0;

    // 出現する敵をランダムに決定する
    var enemyIndex = Random.Range(0, m_enemyPrefabs.Length);

    // 出現する敵のプレハブを配列から取得する
    var enemyPrefab = m_enemyPrefabs[enemyIndex];

    // 敵のゲームオブジェクトを生成する
    var enemy = Instantiate(enemyPrefab);

    // 敵を画面外のどの位置に出現させるかランダムに決定する
    var respawnType = (RESPAWN_TYPE)Random.Range(
        0, (int)RESPAWN_TYPE.SIZEOF);

    // 敵を初期化する
    enemy.Init(respawnType);

    //Factory系　速度アップ
    void Update()
    {
        // 経過時間を差し引く
        _timer -= Time.deltaTime;
        // トータル時間を加算
        _totalTime += Time.deltaTime;

        if (_timer < 0)
        {
            // 0になったのでBlock生成
            // BlockMgrの場所から生成
            Vector3 position = transform.position;
            // ※上下(±3)のランダムな位置に出現させる
            position.y = Random.Range(-4, 4);
            // プレハブをもとにBlock生成
            GameObject obj = Instantiate(block, position, Quaternion.identity);
            // Blockオブジェクトの「Block」スクリプトを取得する
            Block blockScript = obj.GetComponent<Block>();
            // 速度を計算して設定
            // 基本速度100に、経過時間x10を加える
            float speed = 300 + (_totalTime * 10);
            blockScript.SetSpeed(-speed); // 左方向なのでマイナス

            // ②生成回数をカウントアップ
            _cnt++;
            if (_cnt % 10 < 3)
            {
                // 0.1秒後にまた生成する
                _timer += 0.1f;
            }
            else
            {
                // 1秒後にまた生成する
                _timer += 1;
            }
        }
    }

//▼画面外に出ると除去--------------------------------------------------------------------------------------------
    void Update()
    {
        Vector2 position = transform.position;
        if (position.x < GetLeft())
        {
            Destroy(gameObject); // 画面外に出たので消す.
        }
    }

    float GetLeft()
    {
        // 画面の左下のワールド座標を取得する
        Vector2 min = cam.ViewportToWorldPoint(Vector2.zero);
        return min.x;
    }

//▼スコア表示とタイトル表示を行う基本的なゲームマネージャー--------------------------------------------------------------------------------------------

 public class GM_Main : MonoBehaviour
{
    [SerializeField]
    GameObject MainCanvasPrefab;
    GameObject MainCanvas;

    public static GM_Main m_instance;


    private List<int> usedNumbers = new List<int>();

    private enum MiniGamePhase
    {
        TITLE = 0,
        MAIN,
        RESULT
    }
    private int GamePhase = 0;

    [SerializeField] GameObject Obj_MiniGameCamera;
    [SerializeField] GameObject Obj_Player_rirum;
    [SerializeField] GameObject Obj_EnemyManager;


    int _score;


    private void OnEnable()
    {
        // 他のクラスからプレイヤーを参照できるように
        // static 変数にインスタンス情報を格納する
        m_instance = this;

        //Title処理
        GameTitle();

    }
    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        if (GamePhase == (int)MiniGamePhase.MAIN)
        {
            // メインゲーム中のみスコア上昇
            _score += 1;
        }
    }


    public void GameMainStart()
    {
        GamePhase = (int)MiniGamePhase.MAIN;
        GamePhaseInit();
    }

    public void GameTitle()
    {
        GamePhase = (int)MiniGamePhase.TITLE;
        GamePhaseInit();
    }

    public void GameResult()
    {
        GamePhase = (int)MiniGamePhase.RESULT;
        GamePhaseInit();
    }


    private void GamePhaseInit()
    {
        //多分ここをプロジェクト毎に更新する
        ResetMiniGameResetTag();
        Obj_Player_rirum.SetActive(false);
        Obj_EnemyManager.SetActive(false);
        MainCanvas.SetActive(false);

        switch (GamePhase)
        {
            case (int)MiniGamePhase.TITLE:

                _score = 0;

                CheckOneObject();

                break;
            case (int)MiniGamePhase.MAIN:

                _score = 0;

                Obj_Player_rirum.SetActive(true);
                Obj_EnemyManager.SetActive(true);
                Obj_Player_rirum.GetComponent<Player>().Init();
                break;
            case (int)MiniGamePhase.RESULT:
                break;
        }

    }

    private void OnGUI()
    {
        // スコアを描画
        _DrawScore();

        //フェイズを描写
        _DrawPhase();


        // 画面の中心座標を計算する
        float CenterX = Screen.width / 2;
        float CenterY = Screen.height / 2;

        if (GamePhase == (int)MiniGamePhase.TITLE)
        {
            // ゲームオーバーの描画
            _DrawTitle(CenterX, CenterY);

        }

        if (GamePhase == (int)MiniGamePhase.RESULT)
        {
            // ゲームオーバーの描画
            _DrawGameOver(CenterX, CenterY);
            // ②リトライボタンの描画
            if (_DrawRetryButton(CenterX, CenterY))
            {
                // ③クリックしたらやり直しする
                //SceneManager.LoadScene("Flappy");
                GameMainStart();
            }
        }

    }
   




    // ④リトライボタンの描画
    bool _DrawRetryButton(float CenterX, float CenterY)
    {
        float ofsY = 40;
        float w = 100;
        float h = 64;
        Rect rect = new Rect(CenterX - w / 2, CenterY + ofsY, w, h);
        if (GUI.Button(rect, "RETRY"))
        {
            // ボタンを押した
            return true;
        }
        return false;
    }

    // ゲームオーバーの描画
    void _DrawGameOver(float CenterX, float CenterY)
    {
        // 中央揃え
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        float w = 400;
        float h = 100;
        Rect position = new Rect(CenterX - w / 2, CenterY - h / 2, w, h);
        GUI.Label(position, "GAME OVER");
    }

    void _DrawTitle(float CenterX, float CenterY)
    {
        // 中央揃え
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        float w = 400;
        float h = 100;
        Rect position = new Rect(CenterX - w / 2, CenterY - h / 2, w, h);
        //GUI.Label(position, "Flappy");
        GUI.Label(position, SceneManager.GetActiveScene().name);
    }

    // スコアの描画
    void _DrawScore()
    {
        // 文字を大きくする
        GUI.skin.label.fontSize = 32;
        // 左揃え
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        Rect position = new Rect(8, 8, 400, 100);
        GUI.Label(position, string.Format("score:{0}", _score));
    }

    void _DrawPhase()
    {
        // 文字を大きくする
        GUI.skin.label.fontSize = 32;
        // 左揃え
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        Rect position = new Rect(300, 8, 600, 100);
        GUI.Label(position, string.Format("phase:{0}", Enum.GetName(typeof(MiniGamePhase), GamePhase)));
    }
}



}
}
