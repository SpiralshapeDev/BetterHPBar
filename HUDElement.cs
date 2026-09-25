using HarmonyLib;
using Thor;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterHPBar
{
    public class HUDElement
    {
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;
        private static GameObject HUD_GameObject;
        private static TextMeshProUGUI _textMeshProUGUI;

        private static string formatHPText(HealthExt healthExt)
        {
            if (!healthExt)
            {
                Debug.LogError($"{BetterHPBarBase.modGUID} Failed To Set HPBar Text");
                return string.Empty;
            }

            return BetterHPBarBase.healthPercent.Value ? $"{Mathf.Round((float)healthExt.CurrentHP / (float)healthExt.MaxHP * 100)}%" : $"{healthExt.CurrentHP} / {healthExt.MaxHP}";
        }

        private static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            BetterHPBarBase.Instance.Config.Reload();
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            HUD_GameObject = new GameObject
            {
                name = BetterHPBarBase.modGUID + ".BetterHP_GUI"
            };

            HUD_GameObject.transform.SetParent(canvas.transform);
            HUD_GameObject.AddComponent<RectTransform>();
            _textMeshProUGUI = HUD_GameObject.AddComponent<TextMeshProUGUI>();
            RectTransform RT = _textMeshProUGUI.rectTransform;

            RT.anchoredPosition3D = new Vector3(0f, -420f, 0.0f);
            RT.SetParent(_textMeshProUGUI.transform);
            _textMeshProUGUI.fontSize = 36f;

            _textMeshProUGUI.text = string.Empty;
            _textMeshProUGUI.color = Color.white;
            _textMeshProUGUI.overflowMode = TextOverflowModes.Overflow;
            _textMeshProUGUI.alignment = TextAlignmentOptions.Center;
            _textMeshProUGUI.enabled = true;
        }

        private static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            Object.Destroy(HUD_GameObject);
            HUD_GameObject = null;
            _textMeshProUGUI = null;
        }

        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void Awake()
        {
            Debug.Log($"{BetterHPBarBase.modGUID}: Creating UI Events");
            player.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
            player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
            Debug.Log($"{BetterHPBarBase.modGUID}: Done Creating UI Events");
        }

        [HarmonyPatch(typeof(HPBar))]
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void ChangeHP(ref LocalizedText ___m_text, ref HealthExt ___mHealthExt, ref LocalizedText ___m_nameText, ref Entity ___mEntity)
        {
            if (!___m_text || !___mHealthExt || !___m_nameText || !___mEntity) return;
            if (!_textMeshProUGUI) return;
            bool isBoss = ___mEntity.HasTag("boss") || ___mEntity.HasTag("rpm"); // rpm: Rockpile Mimic's special tag

            if (!isBoss || player.AvatarName == ___m_nameText.Text)
            {
                ___m_text.Text = formatHPText(___mHealthExt);
                _textMeshProUGUI.text = string.Empty;
                return;
            }

            if (!BetterHPBarBase.showBossHP.Value) return;
            _textMeshProUGUI.text = formatHPText(___mHealthExt);
        }
    }
}
