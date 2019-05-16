using SA.Foundation.PropertyDrawers.Attributes;
using UnityEngine;

namespace SA.Foundation.EditorStylesCollection
{

    public class SA_ESC_PropertyDrawerExample : MonoBehaviour
    {

        [Header("Unity Default Property Drawers")]
        [Space(10)]

            [Multiline]
            [SerializeField] string m_multilineString;

            [TextArea]
            public string m_textAreaString;

            [Range(0, 100)]
            [SerializeField] int m_intRange;

            [Range(0, 1)]
            [SerializeField] float m_floatRange;

            [Tooltip("Health value between 0 and 100.")]
            [SerializeField] float m_hoverMouseToSeeTooltip;

        [Header("Sliders")]
        [Space(10)]

            [SA_PD_SimpleSlider(4, 23, r: 0, g: 255, b: 0)]
            public float m_simpleSlider;

            [SA_PD_MinMaxSlider(0, 3, r: 255, g: 0, b: 0)]
            public Vector2 m_MinMaxSlider;

            [Header("Help Boxes with text message")]

            [SA_PD_HelpBox(SA_PD_MessageType.Info)]
            public string myHelpBox = "test";


            [SA_PD_HelpBox(SA_PD_MessageType.Warning)]
            public string myString = "Text Message";

        [Header("Help Boxes Decorators")]
        [Space(10)]

            [SA_PD_HelpboxDecorator(SA_PD_MessageType.Info, "Pre-defined text")]
            [SA_PD_HelpboxDecorator(SA_PD_MessageType.None)]

        [Header("Insertions")]
        [Space(10)]

            [SA_PD_ThingInsertionDecorator]

            [SA_PD_PRInsertionDecorator]

            [SA_PD_StandartInsertionDecorator]

        [Header("Headers")]
        [Space(10)]

            [SA_PD_HeaderDecorator("Indent Header")]

        [Header("Indent level")]
        [Space(10)]

            [SA_PD_IndentLevelAttribute(1)]
            public string indentLevel = "IndentLevel";
            [SA_PD_IndentLevelAttribute(2)]
            public string indentLevel1 = "IndentLevel";

            [SA_PD_IndentLevelAttribute(0)]
            public string indentLevel2 = "IndentLevel";

        [Header("Conditional Hide / Show fields")]
        [Space(10)]

            public bool showBelowSettings = false;
            [SA_PD_Conditional("showBelowSettings", true)]
            public float range = 0.0f;
            [SA_PD_Conditional("showBelowSettings", true)]
            public bool isExists = true;
            [SA_PD_Conditional("showBelowSettings", true)]
            public AnimationCurve curve;

        [Header("Conditional Disable / Enable fields")]
        [Space(10)]

            public bool enableBelowSettings = true;
            [SA_PD_Conditional("enableBelowSettings")]
            public bool firstSettings = true;
            [SA_PD_Conditional("enableBelowSettings")]
            public float secondSettigs = 100.0f;

        [Header("Label with Icon")]
        [Space(10)]

            [SA_PD_LabelWithIcon("Plugins/StansAssets/Support2018/Modules/Editor/EditorStylesCollection/Resources/Icons/gameObjectIcon.png")]
            public string myLabel;

            [SA_PD_LabelWithIcon(SA_PD_EditorIcons.IconType.Light)]
            public string favoriteLabel;
    }

}

