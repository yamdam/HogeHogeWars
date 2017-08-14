using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Playerの移動関連を制御するクラス
public class HogePlayer_ReactionControlSystem : MonoBehaviour
{
    // ターゲットとなるカメラ
    [SerializeField]
    private Camera m_TargetCamera;

    // 入力制御フラグ
    public bool m_InputControl;

    // ダッシュ力
    private const float m_dash_power = 1.0f;

    // Playerの攻撃制御クラス
    private HogePlayer_FireControlSystem m_pFireCtrl;

    // X.Z軸の移動量
    private float m_vX;
    private float m_vZ;

    // 追加の加速量
    private Vector3 m_velocity;
    private float m_velocityPower;

    // 押されている方向
    private Vector3 m_inputDirection;

    // キャラクターが向いている方向
    private Vector3 m_playerDirection;

    // カメラが向いている方向
    private Vector3 m_cameraForward;

    // ダッシュのツイーン
    private Tween m_dashTween;

    // コンポーネントゲット処理
    private void Awake()
    {
        m_pFireCtrl = GetComponent<HogePlayer_FireControlSystem>();
    }

    public void Start()
    {
        // 値の初期化
        m_InputControl = true;
    }

    public void Update()
    {
        // 入力の検知
        UpdateCheckInput();
        
        // カメラの法線ベクトル
        m_cameraForward = Vector3.Scale(m_TargetCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        // 入力とカメラの向きに対応した方向
        m_inputDirection = m_cameraForward * m_vZ + m_TargetCamera.transform.right * m_vX;

    }

    public void LateUpdate()
    {
        // Playerの移動を制御
        if (m_inputDirection.x != 0 || m_inputDirection.y != 0)
        {
            // 移動入力があれば対応した方向に移動
            transform.localPosition += m_inputDirection * 0.1f;

            // 移動最中である、尚且つロックオン中でなければ向きを移動方向に向ける
            if(!m_pFireCtrl.LookOn)
                transform.localRotation = Quaternion.LookRotation(m_inputDirection);
        }

        // ロックオン中であれば
        if (m_pFireCtrl.LookOn)
        {
            // XZでのターゲット方向のベクトルを取得
            var lookDirection = m_pFireCtrl.m_target.localPosition - transform.localPosition;
            lookDirection.y = 0;

            // 向きを常にかえる
            transform.localRotation = Quaternion.LookRotation(lookDirection);
        }

        // ダッシュ力が０でなければ
        if(m_velocityPower != 0)
        {
            // ダッシュ方向にダッシュ力で移動
            transform.localPosition += m_velocity * m_velocityPower;
        }
    }

    // プレイヤーの入力検知
    public void UpdateCheckInput()
    {
        // 入力制御フラグがたっていなければリターン
        if(!m_InputControl) { return; }

        // Playerの入力を検知
        m_vX = Input.GetAxisRaw("Horizontal");
        m_vZ = Input.GetAxisRaw("Vertical");

        // ダッシュボタン入力検知
        if (Input.GetButtonDown("Dash"))
        {
            // ダッシュ力を代入
            m_velocityPower = m_dash_power;
            // ダッシュ方向はキーを押していればキー方向、押していなければ今向いている方向にダッシュ
            m_velocity = m_inputDirection.x != 0 || m_inputDirection.y != 0 ? m_inputDirection.normalized : transform.forward;

            // ダッシュ力を時間で減らす
            if(m_dashTween != null) { m_dashTween.Kill(); m_dashTween = null; }
            m_dashTween = DOTween.To(() => m_velocityPower, (x) => m_velocityPower = x, 0, 0.5f);
        }
    }

    // 入力制御フラグをスイッチ
    public void ChangeInputControl()
    {
        m_InputControl = !m_InputControl;
        m_vX = 0;
        m_vZ = 0;
    }

    // 入力制御フラグを変更
    public void ChangeInputControl(bool value)
    {
        m_InputControl = value;
        m_vX = 0;
        m_vZ = 0;
    }
}
