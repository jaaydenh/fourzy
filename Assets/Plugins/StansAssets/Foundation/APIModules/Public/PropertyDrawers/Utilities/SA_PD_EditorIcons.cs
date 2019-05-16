using System.Collections.Generic;

public static class SA_PD_EditorIcons
{
    public enum IconType
    {
        GameObject,
        Favorite,
        Prefab,
        GameManager,
        Dragdot,
        DragdotActive,
        DragdotDimmed,
        Dropdown,
        d_winbtn_mac_close,
        d_winbtn_mac_close_a,
        d_winbtn_mac_close_h,
        d_winbtn_mac_inact,
        d_winbtn_mac_max,
        d_winbtn_mac_max_h,
        d_winbtn_mac_min,
        d_winbtn_mac_min_h,
        d_winbtn_win_close,
        DefaultAsset,
        DllScript,
        DistanceJoint2D,
        FilterByLabel,
        FilterByType,
        Fixedjoint,
        FlareLayer,
        FrictionJoint2D,
        GUIText,
        HingeJoint2D,
        Image,
        LightProbeProxyVolume,
        LightProbeGroup,
        LineRenderer,
        LayoutElement,
        Light,
        LensFlare
    }

    private static Dictionary<IconType, string> m_internalIcons = new Dictionary<IconType, string>() {
        { IconType.DefaultAsset,            "DefaultAsset Icon"          },
        { IconType.GameObject,              "GameObject Icon"            },
        { IconType.Favorite,                "Favorite Icon"              },
        { IconType.Prefab,                  "Prefab Icon"                },
        { IconType.GameManager,             "GameManager Icon"           },
        { IconType.Dragdot,                 "dragdot"                    },
        { IconType.DragdotActive,           "dragdot_active"             },
        { IconType.DragdotDimmed,           "dragdotDimmed"              },
        { IconType.Dropdown,                "Dropdown Icon"              },
        { IconType.d_winbtn_mac_close,      "d_winbtn_mac_close"         },
        { IconType.d_winbtn_mac_close_a,    "d_winbtn_mac_close_a"       },
        { IconType.d_winbtn_mac_close_h,    "d_winbtn_mac_close_h"       },
        { IconType.d_winbtn_mac_inact,      "d_winbtn_mac_inact"         },
        { IconType.d_winbtn_mac_max,        "d_winbtn_mac_max"           },
        { IconType.d_winbtn_mac_max_h,      "d_winbtn_mac_max_h"         },
        { IconType.d_winbtn_mac_min,        "d_winbtn_mac_min"           },
        { IconType.d_winbtn_mac_min_h,      "d_winbtn_mac_min_h"         },
        { IconType.d_winbtn_win_close,      "d_winbtn_win_close"         },
        { IconType.DllScript,               "dll Script Icon"            },
        { IconType.DistanceJoint2D,         "DistanceJoint2D Icon"       },
        { IconType.FilterByLabel,           "FilterByLabel"              },
        { IconType.FilterByType,            "FilterByType"               },
        { IconType.Fixedjoint,              "Fixedjoint Icon"            },
        { IconType.FlareLayer,              "FlareLayer Icon"            },
        { IconType.FrictionJoint2D,         "FrictionJoint2D Icon"       },
        { IconType.GUIText,                 "GUIText Icon"               },
        { IconType.HingeJoint2D,            "HingeJoint2D Icon"          },
        { IconType.Image,                   "Image Icon"                 },
        { IconType.LightProbeProxyVolume,   "LightProbeProxyVolume Icon" },
        { IconType.LightProbeGroup,         "LightProbeGroup Icon"       },
        { IconType.LineRenderer,            "LineRenderer Icon"          },
        { IconType.LayoutElement,           "LayoutElement Icon"         },
        { IconType.Light,                   "Light Icon"                 },
        { IconType.LensFlare,               "LensFlare Icon"             }
    };

    public static string GetInternalStringPathOfEnumValue(IconType type) {
        if (m_internalIcons.ContainsKey(type)) {
            return m_internalIcons[type];
        }

        return m_internalIcons[IconType.DefaultAsset];
    }
}