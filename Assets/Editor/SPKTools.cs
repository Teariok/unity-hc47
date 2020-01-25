using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SPKTools
{
    [MenuItem("Hitman Tools/Build Scene")]
    public static void BuildScene()
    {
        string path = EditorUtility.OpenFilePanel( "Select SPK File", "", "spk" );
        if( string.IsNullOrEmpty(path) )
        {
            return;
        }

        SPKUnpacker unpacker = new SPKUnpacker();
        unpacker.Unpack( path );
        unpacker.BuildScene();
    }
}
