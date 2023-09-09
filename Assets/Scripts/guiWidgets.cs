using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiWidgets 
{
        public static float fSlider(Rect rect, float val, float minVal, float maxVal, string text, string style = "default")
        {
            if (style != "default") { GUI.Label(rect, text, style); } else { GUI.Label(rect, text); }

            rect.y += rect.height * 1.2f;
            val = GUI.HorizontalSlider(rect, val, minVal, maxVal);

            return val;
        }

        public static int iSlider(Rect rect, int val, int minVal, int maxVal, string text, string style = "default")
        {
            if (style != "default") { GUI.Label(rect, text, style); } else { GUI.Label(rect, text); }

            rect.y += rect.height * 1.2f;
            val = (int)GUI.HorizontalSlider(rect, val, minVal, maxVal);
            return val;
        }

        public static Vector3 rgbSlider(Rect rect, Vector3 val, string text, string style = "default")
        {
            float r = val.x;
            float g = val.y;
            float b = val.z;

            if (style != "default") { GUI.Label(rect, text, style); } else { GUI.Label(rect, text); }

            rect.y += rect.height * 1.2f;
            r = GUI.HorizontalSlider(rect, r, 0.0f, 1.0f);
            rect.y += rect.height * .8f;
            g = GUI.HorizontalSlider(rect, g, 0.0f, 1.0f);
            rect.y += rect.height * .8f;
            b = GUI.HorizontalSlider(rect, b, 0.0f, 1.0f);

            return new Vector3(r, g, b);
        }

        public static Vector3 vec3Slider(Rect rect, Vector3 val, Vector2 xRange, Vector2 yRange, Vector2 zRange, string text, string style = "default")
        {
            float r = val.x;
            float g = val.y;
            float b = val.z;

            if (style != "default") { GUI.Label(rect, text, style); } else { GUI.Label(rect, text); }

            rect.y += rect.height * 1.0f;
            r = GUI.HorizontalSlider(rect, r, xRange.x, xRange.y); 
            rect.y += rect.height;
            g = GUI.HorizontalSlider(rect, g, yRange.x, yRange.y); 
            rect.y += rect.height;
            b = GUI.HorizontalSlider(rect, b, zRange.x, zRange.y); 

            return new Vector3(r, g, b);
        }
}
