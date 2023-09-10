
	private void Awake()
	{
	    // 他のクラスからプレイヤーを参照できるように
	    // static 変数にインスタンス情報を格納する
	    m_instance = this;

	    Init();

	}

	//初期化
	public void Init()
	{
	    //位置など
	    transform.localPosition = DefPosition;
	    transform.localEulerAngles = DefRotation;

	    //m_hp = m_hpMax; // HP

	    //m_level = 1; // レベル
	    //m_exp = 0;
	    //m_needExp = GetNeedExp(1); // 次のレベルに必要な経験値

	    //m_shotCount = m_shotCountFrom; // 弾の発射数
	    //m_shotInterval = m_shotIntervalFrom; // 弾の発射間隔（秒）
	    //m_magnetDistance = m_magnetDistanceFrom; // 宝石を引きつける距離


        // ④ゲームオーバーを通知
        //_gameMgr.StartGameOver();
        GM_Main.m_instance.GameResult();
