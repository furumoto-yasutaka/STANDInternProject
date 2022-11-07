using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // プレイヤーステート
    public enum KickStateId
    {
        None = 0,
        Kick,
        LegReturn,
        Length,
    }

    //=====内部から取得するもの
    [Header("プレハブ取得")]
    private PlayerId playerId;
    private PlayerFace playerFace;
    private PlayerEffect playerEffect;
    private PlayerInvincible playerInvincible;
    private Transform body;
    private Transform leg; 
    private Rigidbody2D rb;

    [SerializeField, RenameField("プレイヤーの移動速度")]
    [Header("移動関係")]
    private float moveSpeed = 2.0f;
    [SerializeField, RenameField("入力と逆方向へ加速時の減衰速度")]
    private float moveDecaySpeed = 0.8f;
    [SerializeField, RenameField("プレイヤーの移動入力のしきい値")]
    private float moveThreshold = 2.0f;

    [SerializeField, RenameField("蹴りを発動させる入力しきい値(スティック)")]
    [Header("蹴り関係")]
    private float kickExecStickFall = 0.7f;
    [SerializeField, RenameField("足を伸ばす距離")]
    private float kickRange = 0.85f;
    [SerializeField, RenameField("足を伸ばす時間")]
    private float kickTime = 0.4f;
    [SerializeField, RenameField("足をひっこめる時間")]
    private float kickReturnTime = 0.6f;
    [SerializeField, RenameField("キック力")]
    private float kickPower = 1.0f;
    [SerializeField, RenameField("敵プレイヤーキック時の自分への反動")]
    private float kickPlayerPower = 2.0f;
    [SerializeField, RenameField("キック倍率(X)")]
    private float kickMagX = 1.3f;
    [SerializeField, RenameField("キック時の回転速度")]
    private float kickAngularPower = 100.0f;
    [SerializeField, RenameField("蹴られた時のキック力基準点(最小)")]
    private float kickedAddPowerMin = 0.85f;
    [SerializeField, RenameField("蹴られた時のキック力基準点(最大)")]
    private float kickedAddPowerMax = 1.7f;

    //=====ステート関係
    // 蹴りのステート
    [ReadOnly]
    public KickStateId kickState = KickStateId.None;
    // 死亡したかどうか
    private bool isDeath = false;
    // 敵に飛ばされたかどうか
    private bool isBlow = false;
    // ステートに応じた関数
    private Action[] kickAction;

    //=====入力関係
    // スティックを倒しているか
    private bool fallStick = false;

    //=====その他
    // ジャンプの加速度を反映したか(一度のキックで複数加速度が発生しないようにする)
    private bool isJump = false;
    // 蹴り方向
    private Vector2 kickDirection;
    // キックの時間計測関係
    private float kickTimeCount = 0.0f;
    // 操作対象のデバイス
    private Gamepad pad;


    public int PlayerId { get { return playerId.Id; } }
    public Vector2 KickDirection { get { return kickDirection; } }
    public bool IsDeath { get { return isDeath; } }


    void Start()
    {
        body = transform.GetChild(0);
        leg = transform.GetChild(1);
        leg.gameObject.SetActive(false);

        playerId = GetComponent<PlayerId>();
        playerFace = GetComponent<PlayerFace>();
        playerEffect = GetComponent<PlayerEffect>();
        playerInvincible = GetComponent<PlayerInvincible>();
        rb = body.GetComponent<Rigidbody2D>();

        kickState = KickStateId.None;
        isDeath = true;
        isJump = false;
        kickAction = new Action[(int)KickStateId.Length]
            { Kick_NoneAction, Kick_KickAction, Kick_LegReturnAction };

        // コリジョンを止めておく
        Stop();

        DeviceManager.Instance.Add_RemoveDevicePartsCallBack(PlayerNotActive, playerId.Id);
        pad = DeviceManager.Instance.GetDevice_FromPlayerIndex(PlayerId);
    }

    void Update()
    {
        if (!DeviceManager.Instance.GetIsConnect(playerId.Id) || pad == null) { PlayerNotActive(); }
        if (isDeath) { return; }

        // 移動入力
        MoveAction();
        // 蹴り入力
        kickAction[(int)kickState]();
        // 蹴られ状態継続確認
        CheckBlowEnd();
    }

    /// <summary>
    /// 左右移動処理
    /// </summary>
    void MoveAction()
    {
        //=====入力状態取得
        float stickHorizontal = pad.leftStick.ReadValue().x;
        float dpadHorizontal = pad.dpad.ReadValue().x;

        //=====速度計算
        float horizontal = Mathf.Abs(stickHorizontal) > Mathf.Abs(dpadHorizontal) ? stickHorizontal : dpadHorizontal;
        float decaySpeed = rb.velocity.x * moveDecaySpeed * horizontal * Time.deltaTime;    // 減衰する場合の計算
        float defaultSpeed = moveSpeed * horizontal * Time.deltaTime;                       // 加速をする場合の計算

        //=====速度補正・反映
        if (horizontal < 0.0f && rb.velocity.x >= -moveThreshold)
        {
            // 絶対値の大きい速度を反映する
            if (decaySpeed < defaultSpeed)
            {
                rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
        else if (horizontal > 0.0f && rb.velocity.x <= moveThreshold)
        {
            // 絶対値の大きい速度を反映する
            if (decaySpeed > defaultSpeed)
            {
                rb.velocity += new Vector2(decaySpeed, 0.0f);
            }
            else
            {
                rb.velocity += new Vector2(defaultSpeed, 0.0f);
            }
        }
    }

    /// <summary>
    /// 足を出す(入力受付状態)処理
    /// </summary>
    void Kick_NoneAction()
    {
        //=====入力・方向取得
        Vector2 moveInput = pad.rightStick.ReadValue();
        float sqrMagnitude = moveInput.sqrMagnitude;
        Vector2 normal = moveInput.normalized;

        if (sqrMagnitude < kickExecStickFall * kickExecStickFall)
        {//=====スティックの傾き状況を更新
            fallStick = false;
        }
        else if (!fallStick)
        {//=====蹴り実行
            fallStick = true;

            kickState = KickStateId.Kick;

            kickTimeCount = 0.0f;
            kickDirection = normal;

            leg.position = body.position;
            // 足を出す方向に回転
            leg.rotation = Quaternion.identity * Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.down, kickDirection), Vector3.forward);
            leg.gameObject.SetActive(true);

            // 顔を蹴り状態に遷移
            playerFace.SetState((int)PlayerFace.FaceState.Kick, 999.0f);
        }
    }

    /// <summary>
    /// 足を出す(足出し状態)処理
    /// </summary>
    void Kick_KickAction()
    {
        if (kickTimeCount < kickTime)
        {
            kickTimeCount += Time.deltaTime;

            //=====線形補間で足の位置を更新
            Vector2 vec = kickDirection * kickRange * kickTimeCount / kickTime;
            leg.position = body.position + (Vector3)vec;
        }
        else
        {
            //=====足を戻すステートを遷移する
            kickState = KickStateId.LegReturn;

            kickTimeCount = kickReturnTime;
        }
    }

    /// <summary>
    /// 足を出す(足ひっこめ状態)処理
    /// </summary>
    void Kick_LegReturnAction()
    {
        if (kickTimeCount > 0.0f)
        {
            kickTimeCount -= Time.deltaTime;

            //=====線形補間で足の位置を更新
            Vector2 vec = kickDirection * kickRange * kickTimeCount / kickReturnTime;
            leg.position = body.position + (Vector3)vec;
        }
        else
        {
            //=====何もない状態に遷移する
            kickState = KickStateId.None;

            isJump = false;
            leg.gameObject.SetActive(false);
            leg.position = body.position;

            // 顔の蹴り状態を解除
            playerFace.LiftState((int)PlayerFace.FaceState.Kick);
        }
    }

    /// <summary>
    /// 地形を蹴る処理
    /// </summary>
    public void KickPlatformAddForce(Collider2D collision)
    {
        if (!isJump)
        {
            // 1度の蹴りで複数回力がかからないようにするためのフラグ変更
            isJump = true;

            // 速度を計算
            Vector2 vel = CalcKickForce(-kickDirection, kickPower, 1.0f, 0.0f);

            // 速度を反映
            rb.velocity = vel;
            // 回転速度を速度を元に計算
            rb.angularVelocity = rb.velocity.x * -kickAngularPower;

            // エフェクト再生
            PlayImpactEffect(collision.ClosestPoint(leg.position));

            // 効果音再生
            AudioManager.Instance.PlaySe("ジャンプ");
        }
    }

    /// <summary>
    /// プレイヤーを蹴る処理
    /// </summary>
    public void KickPlayerAddForce()
    {
        if (!isJump)
        {
            // 1度の蹴りで複数回力がかからないようにするためのフラグ変更
            isJump = true;

            // 速度を計算
            Vector2 vel = CalcKickForce(-kickDirection, kickPlayerPower, 1.0f, 0.0f);

            // 速度を反映
            rb.velocity = vel;
            // 回転速度を速度を元に計算
            rb.angularVelocity = rb.velocity.x * -kickAngularPower;
        }
    }

    /// <summary>
    /// プレイヤーに蹴られた処理
    /// </summary>
    /// <param name="distance"> 相手の足と自分の体との距離ベクトル </param>
    /// <param name="direction"> 飛んでいく方向 </param>
    public void KickedAddForce(Vector2 distance, Vector2 direction, Vector2 closestPoint)
    {
        // 速度を計算
        Vector2 vel = CalcKickForce(direction, kickPower * 2.0f, 1.0f, 0.5f);
        float distanceSqrMag = distance.sqrMagnitude;  // 距離の長さ(2乗)を計算
        float addPowerRate = 0.0f;
        
        //=====相手との距離が近いほど蹴る力が増加するように調整する
        if (distanceSqrMag < kickedAddPowerMin * kickedAddPowerMin)
        {
            addPowerRate = 1.0f;
        }
        else if (distanceSqrMag > kickedAddPowerMax * kickedAddPowerMax)
        {
            addPowerRate = 0.0f;
        }
        else
        {
            addPowerRate = (distanceSqrMag - kickedAddPowerMin) / (kickedAddPowerMin - kickedAddPowerMax);
        }

        // 速度を反映
        rb.velocity = vel + distance.normalized * addPowerRate;
        // 回転速度を速度を元に計算
        rb.angularVelocity = rb.velocity.x * -kickAngularPower;

        // 顔を蹴られた状態に遷移
        playerFace.SetState((int)PlayerFace.FaceState.Kicked, 2.0f);

        // ぶっ飛びエフェクトを発生させるか確認
        CheckBlowStart();

        // 蹴られたエフェクトを発生させるか確認
        CheckKickedImpact(closestPoint);
    }

    /// <summary>
    /// プレイヤーに蹴られた処理
    /// </summary>
    /// <param name="direction"> 飛んでいく方向 </param>
    /// <param name="kickPower"> キック力 </param>
    /// <param name="reverseXPowerRate"> 現在の速度とジャンプの速度のX方向が逆だった場合の加算補正倍率 </param>
    /// <param name="reverseYPowerRate"> 現在の速度とジャンプの速度のY方向が逆だった場合の加算補正倍率 </param>
    private Vector2 CalcKickForce(Vector2 direction, float kickPower,
        float reverseXPowerRate, float reverseYPowerRate)
    {
        Vector2 resultVel;
        Vector2 jumpVel = direction * kickPower;
        // 左右の速度に補正を加える
        jumpVel.x *= kickMagX;

        //=====現在のプレイヤーの左右速度が必ずジャンプの左右への加速度へ
        //=====足される状態になるようにベクトルを計算する
        if ((rb.velocity.x > 0 && jumpVel.x > 0) ||
            (rb.velocity.x < 0 && jumpVel.x < 0))
        {
            resultVel.x = jumpVel.x + rb.velocity.x;
        }
        else
        {
            resultVel.x = jumpVel.x - rb.velocity.x * reverseXPowerRate;
        }

        //=====現在の速度とジャンプの速度のY軸が同じ符号の場合は足し合わせ、
        //=====逆の場合は現在の速度の半分をジャンプの速度に足す
        if ((rb.velocity.y > 0 && jumpVel.y > 0) ||
            (rb.velocity.y < 0 && jumpVel.y < 0))
        {
            resultVel.y = jumpVel.y + rb.velocity.y;
        }
        else
        {
            resultVel.y = jumpVel.y - rb.velocity.y * reverseYPowerRate;
        }

        return resultVel;
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Death()
    {
        isDeath = true;

        // コリジョンを止める
        Stop();

        // 死亡エフェクト発生
        Vector2 normal = -new Vector2(rb.velocity.x, rb.velocity.y).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, normal);
        playerEffect.PlayDeathEff(body.position + (Vector3)normal * 2.0f, Quaternion.AngleAxis(angle, Vector3.forward));

        // 吹っ飛びエフェクトを停止
        EndBlow();
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public void Stop()
    {
        // リジッドボディを無効
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0.0f;

        // 当たり判定を無効
        body.GetChild(0).GetComponent<CircleCollider2D>().enabled = false;

        // キックを強制終了
        kickState = KickStateId.None;
        isJump = false;
        leg.gameObject.SetActive(false);
        leg.position = body.position;
    }

    /// <summary>
    /// 復活処理
    /// </summary>
    public void Revival()
    {
        isDeath = false;

        // リジッドボディを有効
        rb.bodyType = RigidbodyType2D.Dynamic;
        body.GetChild(0).GetComponent<CircleCollider2D>().enabled = true;

        // 無敵状態を設定
        playerInvincible.StartSpownInvincible();

        // 顔の状態を初期化
        playerFace.ResetParam();
    }

    /// <summary>
    /// 衝突エフェクト再生
    /// </summary>
    private void PlayImpactEffect(Vector3 createPos)
    {
        float sqrMag = rb.velocity.sqrMagnitude;
        float angle = Vector2.SignedAngle(Vector2.up, leg.position - (Vector3)createPos);
        float threshold = PlayerEffect.hitEffectStrongThreshold * PlayerEffect.hitEffectStrongThreshold;

        if (sqrMag >= threshold)
        {
            playerEffect.PlayImpactStrongEff(createPos, Quaternion.AngleAxis(angle, Vector3.back));
        }
        else
        {
            playerEffect.PlayImpactMiddleEff(createPos, Quaternion.AngleAxis(angle, Vector3.back));
        }
    }

    /// <summary>
    /// 吹っ飛びエフェクト再生確認
    /// </summary>
    public void CheckBlowStart()
    {
        if (!isBlow)
        {
            if (rb.velocity.sqrMagnitude >= PlayerEffect.blowEffStartThreshold * PlayerEffect.blowEffStartThreshold)
            {
                StartBlow();
            }
        }
    }

    /// <summary>
    /// 吹っ飛びエフェクト再生
    /// </summary>
    public void StartBlow()
    {
        isBlow = true;
        playerEffect.PlayBlowEff();
    }

    /// <summary>
    /// 吹っ飛びエフェクト停止確認
    /// </summary>
    public void CheckBlowEnd()
    {
        if (isBlow)
        {
            if (rb.velocity.sqrMagnitude <= PlayerEffect.blowEffEndThreshold * PlayerEffect.blowEffEndThreshold)
            {
                EndBlow();
            }
        }
    }

    /// <summary>
    /// 吹っ飛びエフェクト停止
    /// </summary>
    public void EndBlow()
    {
        isBlow = false;
        playerEffect.StopBlowEff();
    }

    /// <summary>
    /// 蹴られエフェクト・効果音再生確認
    /// </summary>
    public void CheckKickedImpact(Vector2 createEffectPos)
    {
        float threshold = PlayerEffect.kickedImpactWaveStrongThreshold * PlayerEffect.kickedImpactWaveStrongThreshold;

        if (rb.velocity.sqrMagnitude >= threshold)
        {
            playerEffect.PlayKickedImpactStrongEff(createEffectPos);
            AudioManager.Instance.PlaySe("蹴りを受けた(重)");
        }
        else
        {
            playerEffect.PlayKickedImpactNormalEff(createEffectPos);
            AudioManager.Instance.PlaySe("蹴りを受けた(軽)");
        }
    }

    /// <summary>
    /// コントローラー切断時用非アクティブ化処理
    /// </summary>
    public void PlayerNotActive()
    {
        gameObject.SetActive(false);
    }
}
