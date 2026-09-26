/**
 * @file    ChairDebugOverlay1.cs
 * @brief   WIZMOの値をエディタ上に表示
 * @author  中田樹希
 * @data    2026/09/02
 */

using System.Text;
using UnityEngine;

/// <summary>
/// 椅子なし環境でもChairControllerとWIZMOControllerの値を画面上で確認するためのDebug UI。
/// Canvasを使わずOnGUIで表示する。
/// マスター版ChairController対応・軽量化版。
/// </summary>
public class ChairDebugOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChairController m_chairController;
    [SerializeField] private VehicleController m_vehicleController;
    [SerializeField] private WIZMOController m_wizmoController;
    [SerializeField] private Rigidbody m_vehicleRigidbody;
    [SerializeField] private ChairSafetyController m_chairSafetyController;

    [Header("Display")]
    [SerializeField] private bool m_showOverlay = true;
    [SerializeField] private KeyCode m_toggleKey = KeyCode.F9;

    [SerializeField] private float m_x = 20.0f;
    [SerializeField] private float m_y = 20.0f;
    [SerializeField] private float m_width = 560.0f;
    [SerializeField] private float m_height = 660.0f;

    [SerializeField, Range(10, 32)]
    private int m_fontSize = 18;

    [SerializeField, Range(0.05f, 2.0f)]
    private float m_refreshInterval = 0.25f;

    [Header("Auto Resolve")]
    [SerializeField] private bool m_autoResolveReferences = true;

    [SerializeField, Range(0.5f, 10.0f)]
    private float m_resolveInterval = 2.0f;

    private float m_refreshTimer = 0.0f;
    private float m_resolveTimer = 0.0f;

    private readonly StringBuilder m_stringBuilder = new StringBuilder(1024);
    private string m_cachedText = "";

    private GUIStyle m_boxStyle;
    private GUIStyle m_labelStyle;

    private bool HasAllReferences
    {
        get
        {
            return m_chairController != null &&
                   m_vehicleController != null &&
                   m_wizmoController != null &&
                   m_vehicleRigidbody != null;
        }
    }

    private void Awake()
    {
        ResolveReferences();
        BuildDebugText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_toggleKey))
        {
            m_showOverlay = !m_showOverlay;
        }

        // 非表示中はDebug文字列を作らない
        if (!m_showOverlay)
        {
            return;
        }

        // 参照が足りない時だけ、低頻度で自動検索する
        if (m_autoResolveReferences && !HasAllReferences)
        {
            m_resolveTimer -= Time.unscaledDeltaTime;

            if (m_resolveTimer <= 0.0f)
            {
                m_resolveTimer = m_resolveInterval;
                ResolveReferences();
            }
        }

        m_refreshTimer -= Time.unscaledDeltaTime;

        if (m_refreshTimer <= 0.0f)
        {
            m_refreshTimer = m_refreshInterval;
            BuildDebugText();
        }
    }

    private void OnGUI()
    {
        if (!m_showOverlay)
        {
            return;
        }

        InitializeGUIStyle();

        Rect rect = new Rect(m_x, m_y, m_width, m_height);

        GUI.Box(rect, GUIContent.none, m_boxStyle);

        Rect labelRect = new Rect(
            m_x + 10.0f,
            m_y + 10.0f,
            m_width - 20.0f,
            m_height - 20.0f
        );

        GUI.Label(labelRect, m_cachedText, m_labelStyle);
    }

    private void ResolveReferences()
    {
        if (m_chairController == null)
        {
#if UNITY_2023_1_OR_NEWER
            m_chairController = FindFirstObjectByType<ChairController>();
#else
            m_chairController = FindObjectOfType<ChairController>();
#endif
        }

        if (m_vehicleController == null)
        {
#if UNITY_2023_1_OR_NEWER
            m_vehicleController = FindFirstObjectByType<VehicleController>();
#else
            m_vehicleController = FindObjectOfType<VehicleController>();
#endif
        }

        if (m_wizmoController == null)
        {
#if UNITY_2023_1_OR_NEWER
            m_wizmoController = FindFirstObjectByType<WIZMOController>();
#else
            m_wizmoController = FindObjectOfType<WIZMOController>();
#endif
        }

        if (m_chairSafetyController == null)
        {
#if UNITY_2023_1_OR_NEWER
            m_chairSafetyController = FindFirstObjectByType<ChairSafetyController>();
#else
            m_chairSafetyController = FindObjectOfType<ChairSafetyController>();
#endif
        }

        if (m_vehicleRigidbody == null && m_vehicleController != null)
        {
            m_vehicleRigidbody = m_vehicleController.GetComponent<Rigidbody>();
        }
    }

    private void BuildDebugText()
    {
        bool chairFound = m_chairController != null;
        bool vehicleFound = m_vehicleController != null;
        bool wizmoFound = m_wizmoController != null;
        bool safetyFound = m_chairSafetyController != null;

        bool chairActive = chairFound && m_chairController.InGameMove;
        bool wizmoOpened = wizmoFound && m_wizmoController.isOpened();
        bool emergencyStopped = safetyFound && m_chairSafetyController.IsEmergencyStopped;

        float kph = vehicleFound ? m_vehicleController.KPH : -1.0f;
        float steering = vehicleFound ? m_vehicleController.Steering : 0.0f;
        float accelInput = vehicleFound ? m_vehicleController.Accel : 0.0f;
        float brakeInput = vehicleFound ? m_vehicleController.Brake : 0.0f;

        float rbSpeed = -1.0f;
        float angularY = 0.0f;

        if (m_vehicleRigidbody != null)
        {
            rbSpeed = m_vehicleRigidbody.linearVelocity.magnitude;
            angularY = m_vehicleRigidbody.angularVelocity.y;
        }

        float accelForce = chairFound ? m_chairController.DebugAccelForce : 0.0f;
        float centrifugal = chairFound ? m_chairController.DebugCentrifugal : 0.0f;
        float yawRate = chairFound ? m_chairController.DebugYawRate : 0.0f;

        float chairRoll = chairFound ? m_chairController.DebugRoll : 0.0f;
        float chairPitch = chairFound ? m_chairController.DebugPitch : 0.0f;
        float chairYaw = chairFound ? m_chairController.DebugYaw : 0.0f;
        float chairHeave = chairFound ? m_chairController.DebugHeave : 0.0f;

        float wizmoRoll = wizmoFound ? m_wizmoController.roll : 0.0f;
        float wizmoPitch = wizmoFound ? m_wizmoController.pitch : 0.0f;
        float wizmoYaw = wizmoFound ? m_wizmoController.yaw : 0.0f;
        float wizmoHeave = wizmoFound ? m_wizmoController.heave : 0.0f;
        float wizmoSway = wizmoFound ? m_wizmoController.sway : 0.0f;
        float wizmoSurge = wizmoFound ? m_wizmoController.surge : 0.0f;
        float wizmoSpeed = wizmoFound ? m_wizmoController.speed1_all : 0.0f;
        float wizmoAccel = wizmoFound ? m_wizmoController.accel : 0.0f;

        m_stringBuilder.Clear();

        m_stringBuilder.AppendLine("CHAIR DEBUG  [F9 Toggle]");
        m_stringBuilder.AppendLine("--------------------------------");

        m_stringBuilder.AppendLine("[STATUS]");
        m_stringBuilder.Append("Chair Found      : ").AppendLine(chairFound.ToString());
        m_stringBuilder.Append("Vehicle Found    : ").AppendLine(vehicleFound.ToString());
        m_stringBuilder.Append("WIZMO Found      : ").AppendLine(wizmoFound.ToString());
        m_stringBuilder.Append("Safety Found     : ").AppendLine(safetyFound.ToString());
        m_stringBuilder.Append("Chair Active     : ").AppendLine(chairActive.ToString());
        m_stringBuilder.Append("Emergency Stop   : ").AppendLine(emergencyStopped.ToString());
        m_stringBuilder.Append("WIZMO Opened     : ").AppendLine(wizmoOpened.ToString());

        m_stringBuilder.AppendLine();
        m_stringBuilder.AppendLine("[VEHICLE]");
        AppendFloatLine("KPH              : ", kph, "F2");
        AppendFloatLine("RbSpeed          : ", rbSpeed, "F2");
        AppendFloatLine("Steering         : ", steering, "F3");
        AppendFloatLine("AccelInput       : ", accelInput, "F3");
        AppendFloatLine("BrakeInput       : ", brakeInput, "F3");
        AppendFloatLine("AngularY         : ", angularY, "F3");

        m_stringBuilder.AppendLine();
        m_stringBuilder.AppendLine("[FORCE]");
        AppendFloatLine("AccelForce       : ", accelForce, "F3");
        AppendFloatLine("Centrifugal      : ", centrifugal, "F3");
        AppendFloatLine("YawRate          : ", yawRate, "F3");

        m_stringBuilder.AppendLine();
        m_stringBuilder.AppendLine("[CHAIR CALCULATED]");
        AppendFloatLine("ROLL             : ", chairRoll, "F3");
        AppendFloatLine("PITCH            : ", chairPitch, "F3");
        AppendFloatLine("YAW              : ", chairYaw, "F3");
        AppendFloatLine("HEAVE            : ", chairHeave, "F3");

        m_stringBuilder.AppendLine();
        m_stringBuilder.AppendLine("[WIZMO CURRENT]");
        AppendFloatLine("WIZMO ROLL       : ", wizmoRoll, "F3");
        AppendFloatLine("WIZMO PITCH      : ", wizmoPitch, "F3");
        AppendFloatLine("WIZMO YAW        : ", wizmoYaw, "F3");
        AppendFloatLine("WIZMO HEAVE      : ", wizmoHeave, "F3");
        AppendFloatLine("WIZMO SWAY       : ", wizmoSway, "F3");
        AppendFloatLine("WIZMO SURGE      : ", wizmoSurge, "F3");
        AppendFloatLine("WIZMO SPEED      : ", wizmoSpeed, "F3");
        AppendFloatLine("WIZMO ACCEL      : ", wizmoAccel, "F3");

        m_cachedText = m_stringBuilder.ToString();
    }

    private void AppendFloatLine(string label, float value, string format)
    {
        m_stringBuilder.Append(label);
        m_stringBuilder.Append(value.ToString(format));
        m_stringBuilder.AppendLine();
    }

    private void InitializeGUIStyle()
    {
        if (m_boxStyle == null)
        {
            m_boxStyle = new GUIStyle(GUI.skin.box);
            m_boxStyle.padding = new RectOffset(10, 10, 10, 10);
        }

        if (m_labelStyle == null)
        {
            m_labelStyle = new GUIStyle(GUI.skin.label);
            m_labelStyle.fontSize = m_fontSize;
            m_labelStyle.normal.textColor = Color.white;
        }
    }
}