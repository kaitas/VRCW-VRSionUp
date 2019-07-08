using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class mat_combart : EditorWindow
{
    private DefaultAsset png_folder;
    private Material mat_sample;

    [MenuItem("GameObject/Naya_Tools/Books/mat_combart")]
    static void init()
    {
        EditorWindow.GetWindow<mat_combart>();
    }

    private void OnGUI()
    {
        try
        {
            png_folder = (DefaultAsset)EditorGUILayout.ObjectField("画像フォルダ", png_folder, typeof(DefaultAsset), true);
            mat_sample = (Material)EditorGUILayout.ObjectField("参考マテリアル", mat_sample, typeof(Material), true);
            if (GUILayout.Button("コンバート")) Make();
        }
        catch (System.FormatException) { }
    }

    private void Make()
    {
        string path = AssetDatabase.GetAssetOrScenePath(png_folder);
        string[] sp_path = path.Split('/');
        string con_path = "";
        string mat_path = "";
        for (int i = 0; i < sp_path.Length - 2; i++)
        {
            con_path += (sp_path[i] + "/");
        }
        con_path += (sp_path[sp_path.Length - 2]);

        AssetDatabase.CreateFolder(con_path, "material");
        for (int i = 0; i < sp_path.Length - 1; i++)
        {
            mat_path += (sp_path[i] + "/");
        }
        mat_path += (sp_path[sp_path.Length-1] + "/" + sp_path[sp_path.Length - 2]); //ここのフォルダ名を入れられたフォルダ名から取得に変更する
        for (int i = 0; ; i++)
        {
            string mat_path_p = "-";

            if (i < 9)
            {
                mat_path_p += "00" + (i + 1);
            }
            else if (i < 99)
            {
                mat_path_p += "0" + (i + 1);
            }
            else if (i < 999)
            {
                mat_path_p += (i + 1);
            }
            else
            {
                Debug.Log("999までじゃ");
                return;
            }

            Debug.Log(mat_path + mat_path_p);
            if (File.Exists(mat_path + mat_path_p + ".png"))
            {
                mat_path_p += ".png";
                Debug.Log("pngあるで");
            }
            else if (File.Exists(mat_path + mat_path_p + ".jpg"))
            {
                mat_path_p += ".jpg";
                Debug.Log("jpgあるで");

            }
            else
            {
                Debug.Log("ないで");
                mat_path_p = "-";
                if (i < 9)
                {
                    mat_path_p += "0" + (i + 1);
                }
                else if (i < 99)
                {
                    mat_path_p += (i + 1);
                }
                else
                {
                    Debug.Log("99までじゃ");
                    return;
                }
                //mat_path_p += ".png";

                Debug.Log(mat_path + mat_path_p);
                if (File.Exists(mat_path + mat_path_p + ".png"))
                {
                    mat_path_p += ".png";
                    Debug.Log("pngあるで");
                }
                else if (File.Exists(mat_path + mat_path_p + ".jpg"))
                {
                    mat_path_p += ".jpg";
                    Debug.Log("jpgあるで");
                }
                else
                {
                    Debug.Log("ないで");
                    mat_path_p = "-";

                    if (i < 9)
                    {
                        mat_path_p += (i + 1);
                    }
                    else
                    {
                        Debug.Log("9までじゃ");
                        return;
                    }

                    Debug.Log(mat_path + mat_path_p);
                    if (File.Exists(mat_path + mat_path_p + ".png"))
                    {
                        mat_path_p += ".png";
                        Debug.Log("あるで");
                    }
                    else if (File.Exists(mat_path + mat_path_p + ".jpg"))
                    {
                        mat_path_p += ".jpg";
                        Debug.Log("jpgあるで");
                    }
                    else
                    {
                        Debug.Log("ないで");
                        break;
                    }
                }
            }
            Debug.Log(mat_path + mat_path_p);

            Texture2D texture = AssetDatabase.LoadAssetAtPath(mat_path + mat_path_p, typeof(Texture2D)) as Texture2D;
            

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            if (mat_sample != null)
            {
                mat.shader = mat_sample.shader;
                mat.CopyPropertiesFromMaterial(mat_sample);
            }
            mat.SetTexture("_MainTex", texture);
            Debug.Log((con_path + "/material/p" + i + sp_path[sp_path.Length - 2] + ".mat"));
            AssetDatabase.CreateAsset(mat, (con_path + "/material/p" + i + sp_path[sp_path.Length - 2] + ".mat"));
            
        }


    }
}
