using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Playerの攻撃を制御するクラス
public class HogePlayer_FireControlSystem : MonoBehaviour {

    // エネミーのタグ
    private const string m_enemy_tag = "Enemy";

    // ターゲットのTransform
    public Transform m_target = null;

    // 武器スロット数
    public int m_weaponSlot = 4;

    // 武器スロットにある武器List
    public List<WeaponBase> m_weaponList;

    // 現在装備中のスロット番号
    public int m_attachWeaponIndex;

    // ロックオンフラグ
    private bool m_lookOn;

    void Start () {
        LookOn = false;
        m_attachWeaponIndex = 0;
    }

    void Update () {

        // 攻撃ボタン入力検知
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire!!!");
            m_weaponList[m_attachWeaponIndex].Fire();
        }

        // ロックオンボタン入力検知
        if (Input.GetButtonDown("Lookon"))
        {
            // ロックオンのON/OFF
            LookOn = !LookOn;
        }

        // 左武器切り替えボタン入力検知
        if (Input.GetButtonDown("LeftChange"))
        {
            Debug.Log("WeaponChange Left!!");
        }
        // 右武器切り替えボタン入力検知
        if (Input.GetButtonDown("RightChange"))
        {
            Debug.Log("WeaponChange Right!!");
        }

    }

    // ロックオンフラグプロパティ
    public bool LookOn
    {
        get { return m_lookOn; }
        set
        {
            if (value) { ShortRengeSearch(); }
            else{ m_target = null; }
            m_lookOn = value;
        }
    }

    // 一番近い距離にいるエネミーをターゲットにする
    public void ShortRengeSearch()
    {
        // エネミーのタグが付いているオブジェクトを検索
        var bogeys = GameObject.FindGameObjectsWithTag(m_enemy_tag);

        Transform target = bogeys[0].transform;
        float currentdistance = Vector3.SqrMagnitude(transform.localPosition - target.localPosition);

        foreach (var bogey in bogeys)
        {
            float distance = Vector3.SqrMagnitude(transform.localPosition - bogey.transform.localPosition);
            // 距離がより近い方をターゲットに
            if (currentdistance > distance)
            {
                target = bogey.transform;
                currentdistance = distance;
            }
        }

        m_target = target;
        Debug.Log(m_target.name + "ロックオン!!");
    }
}
