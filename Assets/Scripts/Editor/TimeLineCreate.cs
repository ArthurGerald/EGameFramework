using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class TimeLineCreate 
{
    public static string TIMELINE_SAVE_PATH = "Assets/TimeLine/";
    [MenuItem("Tools/创建TimeLine")]
    public static void CreateTime()
    {
        TimelineAsset timeline = ScriptableObject.CreateInstance<TimelineAsset>();
        timeline.name = "test_timeline";

        if (!Directory.Exists(TIMELINE_SAVE_PATH))
        {
            Directory.CreateDirectory(TIMELINE_SAVE_PATH);
        }
        string assetPath =  TIMELINE_SAVE_PATH + timeline.name + ".playable";
        Debug.LogError(assetPath);
        AssetDatabase.CreateAsset(timeline, assetPath);                        //先将timeline保存到assetdatabase中

        AnimationTrack track = timeline.CreateTrack<AnimationTrack>();            //再创建track
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
