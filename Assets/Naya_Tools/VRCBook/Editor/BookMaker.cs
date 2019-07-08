using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BookMaker : EditorWindow
{
    

    private GameObject VRCWoald;

    private GameObject R_page;
    private GameObject R_Bar;

    private GameObject L_page;
    private GameObject L_Bar;

    private DefaultAsset page_fold;

    private bool local = false;
    private bool gimic = false;

    private int page = 100;

    

    [MenuItem("GameObject/Naya_Tools/Books/BookMaker")]

    static void init()
    {
        EditorWindow.GetWindow<BookMaker>();
    }

    private void OnGUI()
    {
        
        try
        {

            EditorGUILayout.LabelField("ページ");
            R_page = EditorGUILayout.ObjectField("右ページ", R_page, typeof(GameObject), true) as GameObject;
            L_page = EditorGUILayout.ObjectField("左ページ", L_page, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField("ページ送りボタン");
            R_Bar = EditorGUILayout.ObjectField("戻るボタン", R_Bar, typeof(GameObject), true) as GameObject;
            L_Bar = EditorGUILayout.ObjectField("進むボタン", L_Bar, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField(" ");

            VRCWoald = EditorGUILayout.ObjectField("ワールドオブジェクト", VRCWoald, typeof(GameObject) ,true )as GameObject;

            page_fold = (DefaultAsset)EditorGUILayout.ObjectField("ページマテリアルフォルダ", page_fold, typeof(DefaultAsset),true);

            local = EditorGUILayout.Toggle("動作をローカルにする", local);
            gimic = EditorGUILayout.Toggle("ページにギミックを仕込む", gimic);

            if (GUILayout.Button("製本")) Make();
        }
        catch (System.FormatException) { }
    }

    private void Make()
    {



        List<GameObject> R_con = new List<GameObject>();
        List<GameObject> L_con = new List<GameObject>();
        List<GameObject> Gimic_List = new List<GameObject>();
        string path = AssetDatabase.GetAssetOrScenePath(page_fold);
        string[] sp_path = path.Split('/');
        string re_path = sp_path[2];
        for (int i = 3; i < sp_path.Length; i++)
            re_path += "/" + sp_path[i];




        VRCSDK2.VRC_SceneDescriptor vRC_Scene = VRCWoald.GetComponent<VRCSDK2.VRC_SceneDescriptor>();

        for(int i = 0; ; i++)
        {
            if (File.Exists(path + "/p" + i + sp_path[sp_path.Length-2]+".mat"))
            {
                Material mat = Resources.Load(re_path + "/p" + i+ sp_path[sp_path.Length - 2]) as Material;
                if (mat == null)
                    Debug.Log(re_path+ "/p" + i + ".mat"+" mat is null");
                vRC_Scene.DynamicMaterials.Add(mat);
            }
            else
            {
                Debug.Log("end mat");

                break;
            }
        }
        
        
        Material mat1 = Resources.Load(re_path + "/p0"+ sp_path[sp_path.Length - 2]) as Material;
        Material mat2 = Resources.Load(re_path + "/p1"+ sp_path[sp_path.Length - 2]) as Material;

        String R_con_name = R_Bar.name;
        String L_con_name = L_Bar.name;

        R_page.GetComponent<Renderer>().material = mat1;
        L_page.GetComponent<Renderer>().material = mat2;
                        
        GameObject R_con_pac = new GameObject();
        R_con_pac.name = "R_con_pac";
        R_con_pac.transform.parent = R_Bar.transform.parent;
        R_con_pac.transform.localPosition = new Vector3(0, 0, 0);
        R_con_pac.transform.localRotation =new Quaternion(0, 0, 0,0);
        R_con_pac.transform.localScale = new Vector3(1, 1, 1);
        //R_Bar.transform.parent = R_con_pac.transform;
        R_Bar.SetActive(false);


        GameObject L_con_pac = new GameObject();
        L_con_pac.name = "L_con_pac";
        L_con_pac.transform.parent = L_Bar.transform.parent;
        L_con_pac.transform.localPosition = new Vector3(0, 0, 0);
        L_con_pac.transform.localRotation = new Quaternion(0, 0, 0, 0);
        L_con_pac.transform.localScale = new Vector3(1, 1, 1);
        //L_Bar.transform.parent = L_con_pac.transform;
        L_Bar.SetActive(false);


        /*ページ遷移イベント取得まで
         * 前提としてTriggerのEventsの
         * 0に右ページ
         * 1に左ページ
         * 2に右のコントロールバー入れ替え
         * 3に左のコントロールバー入れ替え
         * をが入っている
         */


        for (int i = 0; ; i++) //右コントローラー作成
        {
            
            Debug.Log("Rpage" + (2 * i) + "_" + 2 * i);
            if (File.Exists(path + "/p" + (i * 2) + sp_path[sp_path.Length-2]+".mat"))
            {
                Debug.Log(path + "/p" + (i * 2 ) + sp_path[sp_path.Length-2]+".mat"+" true");
            }
            else
            {
                Debug.Log(path + "/p" + (i * 2 ) + sp_path[sp_path.Length-2]+".mat"+ " false");
                break;
            }
            
            R_con.Add(Instantiate(R_Bar) as GameObject);

            R_con[i].name = "R_con_" + (2 * i) + "_" + (2 * i+1);
            Debug.Log("リネーム");
            R_con[i].transform.parent = R_con_pac.transform;
            Debug.Log("親変更");
            R_con[i].transform.localPosition = R_Bar.transform.localPosition;
            R_con[i].transform.localRotation = R_Bar.transform.localRotation;
            R_con[i].transform.localScale = R_Bar.transform.localScale;
            Debug.Log("トランスフォーム調整");

            VRCSDK2.VRC_Trigger R_bar_trg = R_con[i].GetComponent(typeof(VRCSDK2.VRC_Trigger)) as VRCSDK2.VRC_Trigger;
            if(R_bar_trg == null)
            {
                Debug.Log("トリガーが無いが");
                R_bar_trg = R_con[i].AddComponent<VRCSDK2.VRC_Trigger>();
            }
                
            Debug.Log("トリガー取得");
            VRCSDK2.VRC_Trigger.TriggerEvent r_trg = new VRCSDK2.VRC_Trigger.TriggerEvent();
            Debug.Log("トリガーイベント作成");

            R_bar_trg.interactText = R_con_name;

            R_bar_trg.Triggers.Add(r_trg);
            Debug.Log("トリガーイベント追加");

            r_trg.TriggerType = VRCSDK2.VRC_Trigger.TriggerType.OnInteract;

            VRCSDK2.VRC_EventHandler.VrcEvent evr = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent evl = new VRCSDK2.VRC_EventHandler.VrcEvent();

            Debug.Log("イベントインスタンス作成");
            if(local)r_trg.BroadcastType = VRCSDK2.VRC_EventHandler.VrcBroadcastType.Local; //GUIから切り変えれるように
            


            evr.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetMaterial;
            evl.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetMaterial;

            Debug.Log("イベントタイプ指定");

            evr.ParameterString = path + "/p" + (i * 2 ) + sp_path[sp_path.Length-2]+".mat";
            evr.ParameterObject = R_page ; //ここに右ページオブジェクト

            Debug.Log("右ページマテリアル指定");

            evl.ParameterString = path + "/p" + (i * 2 + 1) +sp_path[sp_path.Length-2]+".mat";
            evl.ParameterObject = L_page;

            Debug.Log("左ページマテリアル指定");


            r_trg.Events.Add(evr);
            r_trg.Events.Add(evl);

        }

        for (int i = 0; ; i++)　//左コントローラー作成
        {
            Debug.Log("Lpage" + (2 * i) + "_" + 2 * i);
            if (File.Exists(path + "/p" + (i * 2) + sp_path[sp_path.Length-2]+".mat"))
            {
                Debug.Log(path + "/p" + (i * 2) + sp_path[sp_path.Length-2]+".mat" + " true");
            }
            else
            {
                Debug.Log(path + "/p" + (i * 2) + sp_path[sp_path.Length-2]+".mat" + " false");
                break;
            }

            L_con.Add(Instantiate(L_Bar) as GameObject);

            L_con[i].name = "L_con_" + (2 * i) + "_" + (2 * i + 1);
            Debug.Log("リネーム");
            L_con[i].transform.parent = L_con_pac.transform;
            Debug.Log("親変更");
            L_con[i].transform.localPosition = L_Bar.transform.localPosition;
            L_con[i].transform.localRotation = L_Bar.transform.localRotation;
            L_con[i].transform.localScale = L_Bar.transform.localScale;
            Debug.Log("トランスフォーム調整");

            VRCSDK2.VRC_Trigger L_bar_trg = L_con[i].GetComponent(typeof(VRCSDK2.VRC_Trigger)) as VRCSDK2.VRC_Trigger;
            Debug.Log("トリガー取得");
            if (L_bar_trg == null)
            {
                Debug.Log("トリガーが無いが");
                L_bar_trg = L_con[i].AddComponent<VRCSDK2.VRC_Trigger>();
            }

            L_bar_trg.interactText = L_con_name;


            VRCSDK2.VRC_Trigger.TriggerEvent l_trg = new VRCSDK2.VRC_Trigger.TriggerEvent();
            Debug.Log("トリガーイベント作成");

            L_bar_trg.Triggers.Add(l_trg);
            Debug.Log("トリガーイベント追加");

            l_trg.TriggerType = VRCSDK2.VRC_Trigger.TriggerType.OnInteract;

            VRCSDK2.VRC_EventHandler.VrcEvent evr = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent evl = new VRCSDK2.VRC_EventHandler.VrcEvent();

            Debug.Log("イベントインスタンス作成");
            if (local) l_trg.BroadcastType = VRCSDK2.VRC_EventHandler.VrcBroadcastType.Local; //GUIから切り変えれるように




            evr.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetMaterial;
            evl.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetMaterial;

            Debug.Log("イベントタイプ指定");

            evr.ParameterString = path + "/p" + (i * 2) + sp_path[sp_path.Length-2]+".mat";
            evr.ParameterObject = R_page; //ここに右ページオブジェクト

            Debug.Log("右ページマテリアル指定");

            evl.ParameterString = path + "/p" + (i * 2 + 1) + sp_path[sp_path.Length-2]+".mat";
            evl.ParameterObject = L_page;

            Debug.Log("左ページマテリアル指定");


            l_trg.Events.Add(evr);
            l_trg.Events.Add(evl);


        }

        Debug.Log("オブジェクト作成終了");
        Debug.Log("遷移システム開始");

        if (gimic)
        {
            GameObject Gimic_pac = new GameObject();
            Gimic_pac.name = "Gimic_pac";
            Gimic_pac.transform.parent = L_Bar.transform.parent;
            Gimic_pac.transform.localPosition = new Vector3(0, 0, 0);
            Gimic_pac.transform.localRotation = new Quaternion(0, 0, 0, 0);
            Gimic_pac.transform.localScale = new Vector3(1, 1, 1);
            //L_Bar.transform.parent = L_con_pac.transform;

            for (int i = 0; i < R_con.Count; i++)
            {
                Gimic_List.Add(new GameObject());
                Gimic_List[i].name = "p" + (i * 2) + "_" + (i * 2 + 1);
                Gimic_List[i].transform.localScale = new Vector3(1, 1, 1);
                Gimic_List[i].transform.parent = Gimic_pac.transform;
                Gimic_List[i].transform.localPosition = new Vector3(0, 0, 0);
                Gimic_List[i].transform.localRotation = new Quaternion(0, 0, 0, 0);

                if (i != 0) Gimic_List[i].SetActive(false);
            }
        }


        Debug.Log("R_con表示設定");
        for (int i = 0; i < R_con.Count; i++) //右コン操作 戻る操作（）
        {
            VRCSDK2.VRC_Trigger trg = R_con[i].GetComponent<VRCSDK2.VRC_Trigger>();
            VRCSDK2.VRC_Trigger.TriggerEvent intract = new VRCSDK2.VRC_Trigger.TriggerEvent();

            Debug.Log("トリガーインスタンス作成");


            intract.TriggerType = VRCSDK2.VRC_Trigger.TriggerType.OnInteract;
            Debug.Log("トリガータイプ指定");

            if (local) intract.BroadcastType = VRCSDK2.VRC_EventHandler.VrcBroadcastType.Local; //GUIから切り替えれるように

            VRCSDK2.VRC_EventHandler.VrcEvent r_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent r_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent l_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent l_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();
            Debug.Log("イベントインスタンス作成");


            r_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            r_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            l_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            l_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            Debug.Log("イベントタイプ指定");


            r_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
            r_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;
            l_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
            l_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;
            Debug.Log("Bool設定");

            //戻るボタンで戻った先がi r_next i-1 l_next i+1 r_pre i l_pre i+2

            Debug.Log("r_next");
            if (i > 0) r_next.ParameterObject = R_con[i-1];

            Debug.Log("r_pre");
            r_pre.ParameterObject = R_con[i];

            Debug.Log("l_next");
            if (i < L_con.Count - 1) l_next.ParameterObject = L_con[i + 1];

            Debug.Log("l_pre");
            if (i < L_con.Count - 2) l_pre.ParameterObject = L_con[i + 2];
            Debug.Log("遷移先指定");


            if (i > 0) intract.Events.Add(r_next);
            intract.Events.Add(r_pre);
            if (i < L_con.Count - 1) intract.Events.Add(l_next);
            if (i < L_con.Count - 2) intract.Events.Add(l_pre);
            if (gimic)  //ギミック出し入れ部分
            {
                VRCSDK2.VRC_EventHandler.VrcEvent G_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
                VRCSDK2.VRC_EventHandler.VrcEvent G_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();

                G_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
                G_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;

                G_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
                G_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;

                Debug.Log("G_next");
                G_next.ParameterObject = Gimic_List[i];

                Debug.Log("G_pre");
                if(i<Gimic_List.Count-1)G_pre.ParameterObject = Gimic_List[i + 1];

                intract.Events.Add(G_pre);
                intract.Events.Add(G_next);
            }
            Debug.Log("イベント割り当て");


            trg.Triggers.Add(intract);
            Debug.Log("トリガー割り当て");

        }
        Debug.Log("L_con表示設定");

        for (int i = 0; i < R_con.Count; i++) //左コン操作
        {
            VRCSDK2.VRC_Trigger trg = L_con[i].GetComponent<VRCSDK2.VRC_Trigger>();
            VRCSDK2.VRC_Trigger.TriggerEvent intract = new VRCSDK2.VRC_Trigger.TriggerEvent();

            Debug.Log("トリガーインスタンス作成");


            intract.TriggerType = VRCSDK2.VRC_Trigger.TriggerType.OnInteract;
            Debug.Log("トリガータイプ指定");
            if (local) intract.BroadcastType = VRCSDK2.VRC_EventHandler.VrcBroadcastType.Local; //GUIから切り替えれるように

            VRCSDK2.VRC_EventHandler.VrcEvent r_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent r_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent l_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
            VRCSDK2.VRC_EventHandler.VrcEvent l_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();
            Debug.Log("イベントインスタンス作成");


            r_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            r_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            l_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            l_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
            Debug.Log("イベントタイプ指定");


            r_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
            r_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;
            l_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
            l_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;
            Debug.Log("Bool設定");

            //進むボタン 進んだ先がiだからnextはrがi-1 lがi+1　preはrがi-2 lがi
            Debug.Log("r_next");
            if (i > 0) r_next.ParameterObject = R_con[i-1];

            Debug.Log("r_pre");
            if (i > 1)r_pre.ParameterObject = R_con[i-2];

            Debug.Log("l_next");
            if (i < L_con.Count - 1) l_next.ParameterObject = L_con[i + 1];

            Debug.Log("l_pre");
            l_pre.ParameterObject = L_con[i];
            Debug.Log("遷移先指定");

            if (i > 0) intract.Events.Add(r_next);
            if (i > 1)intract.Events.Add(r_pre);
            if (i < L_con.Count - 1) intract.Events.Add(l_next);
            intract.Events.Add(l_pre);
            if (gimic)  //ギミック出し入れ部分
            {
                VRCSDK2.VRC_EventHandler.VrcEvent G_next = new VRCSDK2.VRC_EventHandler.VrcEvent();
                VRCSDK2.VRC_EventHandler.VrcEvent G_pre = new VRCSDK2.VRC_EventHandler.VrcEvent();

                G_next.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;
                G_pre.EventType = VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive;

                G_next.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.True;
                G_pre.ParameterBoolOp = VRCSDK2.VRC_EventHandler.VrcBooleanOp.False;

                Debug.Log("G_next");
                G_next.ParameterObject = Gimic_List[i];

                Debug.Log("G_pre");
                if(0 < i)G_pre.ParameterObject = Gimic_List[i - 1];

                intract.Events.Add(G_next);
                intract.Events.Add(G_pre);
            }
            Debug.Log("イベント割り当て");


            trg.Triggers.Add(intract);
            Debug.Log("トリガー割り当て");

        }

        L_con[1].SetActive(true);
        Debug.Log(L_con.Count*2 +"ページ");

    }

}

/*
 * 処理順序
 * ページ数を取得する
 * 右ページに1ページ目、左ページに2ページ目のマテリアルを割り当てる
 * 1-2ページを作成するコントロールバーを作成する
 *      トリガーコンポーネントを作成
 *      割り当て
 * 作ったバーをコピーしてリスト化
 * 
 * 
 * 
 * 
 */ 