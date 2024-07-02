using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class CheckFontMissChar : MonoBehaviour
{
    [Button]
    public void FontUniCode()
    {
        if (TryGetComponent(out Text UiText))
        {
            //将3500常用字粘贴到text的文本框中
            string input = UiText.text.Trim();
            //得到text的字体font
            var font = UiText.font;
            //要输出的文本内容
            var output = string.Empty;
            //丢失的字符串个数
            int missCount = 0;
            //判断字体是否是动态的
            bool isDynamic = font.dynamic;
            //如果是动态的字体现改为unicode来方便判断缺失的字体
            if (isDynamic)
            {
#if UNITY_EDITOR
                TrueTypeFontImporter fontData =
                    (TrueTypeFontImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(font));
                fontData.fontTextureCase = FontTextureCase.Unicode;
                fontData.SaveAndReimport();
#endif
            }

            List<char> uniChars = new List<char>();
            //遍历3500个常用字
            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];
                //如果无法得到对应字的信息
                if (!font.GetCharacterInfo(character, out var info) && !uniChars.Contains(character)&&!string.IsNullOrWhiteSpace(character.ToString()))
                {
                    uniChars.Add(character);
                    missCount += 1;
                    //将文字转化为unicode
                    var hexStrings = char.ConvertToUtf32(s: character.ToString(), index: 0).ToString(format: "X4");
                    //添加到输出文本中,比如$963F,每个字符的unicode用,来问隔
                    output += "$" + hexStrings + ",";
                    Debug.Log(message: missCount + "_missing: " + character + ":" + hexStrings);
                }
            }

            Debug.Log(missCount); //输出
            //复制到系统的粘贴板
            GUIUtility.systemCopyBuffer = output;

            //如果是动态的字体，再改回动态的
            if (isDynamic)
            {
#if UNITY_EDITOR
                TrueTypeFontImporter fontData =
                    (TrueTypeFontImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(font));
                fontData.fontTextureCase = FontTextureCase.Dynamic;
                fontData.SaveAndReimport();
#endif
            }
        }
    }
}
