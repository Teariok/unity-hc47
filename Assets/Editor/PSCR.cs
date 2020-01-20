using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSCR : Sector
{
    private enum ValueType:uint
    {
        INT = 14,
        FLOAT = 15,
        ARRAY = 38
    };

    /*public override void Unpack( ref uint offset, byte[] data )
    {
        base.Unpack( ref offset, data );

        // Unknown 1 for HK is 1056964608 so probably some kind of flag field or mask, it's bits 24-29 on and the rest off (3F00 0000).
        // That places it just outside the range for the mask 0x00FFFFFF.
        // Alternatively it's also the float val 0.5 but I'm not sure what that would indicate

        uint dataSize = GetUInt( ref offset );
        uint unknown1 = GetUInt( ref offset );
        uint scriptCount = GetUInt( ref offset );

        Debug.Log( "PSCR Data Size: " + dataSize );
        Debug.Log( "Script Unknown 2: " + unknown1 );
        Debug.Log( "Script Count: " + scriptCount );

        if( scriptCount == 0 )
        {
            return;
        }

        // The next set of data seems like compiled sdl scripts.
        // They seem to start with the script name.
        // Immediately after the script name is a uint32.
        // sdl scripts have a NativeImport structure and the values for them seem
        // to be what follows this number, so the number is likely to indicate how many values there are to follow. The Pedestrian.sdl
        // file contains 3 native imports but also imports other scripts and if all the native imports are added up, there are 10 in total.

        // This leaves the values themselves. The structure appears to be an int32 indicating the value type, 4 bytes for the value and then
        // the name of the variable. There is a complication though in that some of the values have 11 bytes before them. In these cases, the 
        // first 3 bytes are all null and then we get the expected structure of 4 byte type and 4 byte value. Not sure if the 3 bytes are actually
        // padding on the end of the variable name.

        // The value type indicators have the following meanings:
        // 0E 00 00 00 (14) - int32 (or REF which is actually just a typedef int32)
        // 0F 00 00 00 (15) - float
        // 26 00 00 00 (38) - list/array (or ConditionList which is a typedef I think for an array)

        // Lists work slightly differently. There is still another 4 bytes following the type indicator but I'm not sure what it means yet.
        // The list data follows after the variable name just as a sequence of strings.

        // For HK C1_1 then, the data should look something like this:

        // [53 63 72 69 70 74 73 5C 41 6C 6C 4C 65 76 65 6C 73 5C 53 74 64 5C 50 65 64 65 73 74 72 69 61 6E 2E 73 64 6C 00]
        // Script Name: Sripts\AllLevels\Std\Pedestrian.sdl

        // [0A 00 00 00]
        // Variable Count: 10

        // [0E 00 00 00]
        // Variable Type: 14 (int)

        // [00 00 00 00]
        // Variable Value?: 0

        // [4D 61 78 48 69 74 70 6F 69 6E 74 73 00]
        // Variable Name: MaxHitpoints (defined in being.sdl as an int)

        // [00 00 00 00]
        // Padding?

        // [0E 00 00 00]
        // Variable Type: 14 (int)

        // [04 00 00 00]
        // Variable Value?: 4

        // [44 4E 41 5F 4F 76 65 72 72 69 64 65 00]
        // Variable Name: DNA_Override (defined in being.sdl as an int)

        // [00 00 00]
        // Padding?

        // [0E 00 00 00]
        // Variable Type: 14 (int)

        // [08 00 00 00]
        // Variable Value?: 8

        // [4C 65 76 65 6C 43 6F 6E 74 72 6F 6C 00]
        // Variable Name: LevelControl (defined in being.sdl as a Ref)

        // [00 00 00]
        // Padding?

        // [0F 00 00 00]
        // Variable Type: 15 (float)

        // [40 01 00 00]
        // Variable Value?: 4.4841

        // [4D 69 6E 4D 6F 72 61 6C 65 00]
        // Variable Name: MinMorale (defined in man.sdl as a float)

        // [00 00 00]
        // Padding?

        // [0F 00 00 00]
        // Variable Type: 15 (float)

        // [44 01 00 00]
        // Variable Value?: 4.5402

        // [4D 61 78 52 65 61 63 74 69 6F 6E 00]
        // Variable Name: MaxReaction (defined in man.sdl as a float)

        // [00 00 00]
        // Padding?
        // [26 00 00 00]
        // Variable Type: 38 (list/array/ConditionList)

        // [6E 2E 73 64]
        // Unidentified

        // [48 65 6C 70 50 6F 69 6E 74 73 00]
        // Variable Name: HelpPoints (defined in Neutral.sdl as a ConditionList)

        // [5A 47 45 4F 4D 52 45 46 00]
        // String: ZGEOMREF (a valid value for the HelpPoints variable)

        // [5A 43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 00]
        // String: ZConditionList (a valid value for the HelpPoints variable)

        // [43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 49 46 00]
        // String: ConditionListIF (a valid value for the HelpPoints variable)

        // !Note! The 3 null bytes are not present this time

        // [26 00 00 00]
        // Variable Type: 38 (list/array/ConditionList)

        // [68 72 65 61]
        // Unidentified

        // [53 61 66 65 50 6F 69 6E 74 73 00]
        // Variable Name: SafePoints (defined in Neutral.sdl as a ConditionList)

        // [5A 47 45 4F 4D 52 45 46 00]
        // String: ZGEOMREF

        // [5A 43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 00]
        // String: ZConditionList

        // [43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 49 46 00]
        // String: ConditionListIF

        // [26 00 00 00]
        // Variable Type: 38 (list/array/ConditionList)

        // [65 20 34 39]
        // Unidentified

        // [50 61 74 68 4C 69 73 74 32 00]
        // Variable Name: PathList2 (defined in Pedestrian.sdl as a ConditionList)

        // [5A 47 45 4F 4D 52 45 46 00]
        // String: ZGEOMREF

        // [5A 43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 00]
        // String: ZConditionList

        // [43 6F 6E 64 69 74 69 6F 6E 4C 69 73 74 49 46 00]
        // String: ConditionListIF

        // [0E 00 00 00]
        // Variable Type: 14 (int)

        // [88 02 00 00]
        // Variable Value?: 648

        // [50 61 74 68 00]
        // Variable Name: Path (defined in Pedestrian.sdl as a REF)

        // [00 00 00]
        // Padding?

        // [0E 00 00 00]
        // Variable Type: 14 (int)

        // [8C 02 00 00]
        // Variable Value?: 652

        // [4C 6F 6F 70 69 6E 67 00]
        // Variable Name: Looping (defined in Pedestrian.sdl as an int)

        // Following this is a section I can't identify:
        // [00 00 00 44 43 00 00 44 43 00 00 00 00 00 3F D4 2D 00 00 5B]
        // 00 00 00 44
        // 43 00 00 44
        // 43 00 00 00
        // 00 00 3F D4
        // 2D 00 00 5B

        // But the next section also has some parts that seem related (however it needs the 5B from the section above).
        // It doesn't nicely map to groups of 2, 4, 8 etc though
        // [5B 24 00 00 00 00 00 00 00]
        // [5B 28 00 00 00 00 00 00 00]
        // [5B 60 00 00 00 00 00 00 00]
        // [5B 64 00 00 00 00 00 00 00]
        // [5B 68 00 00 00 01 00 00 00]
        // [5B 6C 00 00 00 01 00 00 00]
        // [5B 70 00 00 00 00 00 00 00]
        // [5B C8 00 00 00 00 00 00 00]
        // [5B CC 00 00 00 00 00 00 00]
        // [5B D0 00 00 00 00 00 00 00]
        // [5B D4 00 00 00 00 00 00 00 15]

        // After that block there is this:
        // [48 69 74 6D 61 6E 47 72 6F 75 6E 64 00 39 43 6F 6D 00 47 65 74 53 63 65 6E 65 49 6E 74 00]
        // Which translates into this:
        // HitmanGround [39] Com.GetSceneInt.
        // I'm not sure what the 0x39 is, but the rest is the "being" constructor (being.sdl line 78).
        // It's followed by more data and then "TrackLinkObj.Ref" which is line 81 of the being constructor.
        // The rest of the block follows this same pattern



        string scriptPath = GetString( ref offset );
        uint valueCount = GetUInt( ref offset );

        for( int i = 0; i < valueCount; i++ )
        {
            uint valueType = GetUInt( ref offset );
            if( valueType == (uint)ValueType.INT )
            {
                uint valueData = GetUInt( ref offset );
            }
            else if( valueType == (uint)ValueType.FLOAT )
            {
                //float valueData = GetFloat( ref offset );
                GetByte( ref offset );
                GetByte( ref offset );
                GetByte( ref offset );
                GetByte( ref offset );
            }
            else if( valueType == (uint)ValueType.ARRAY )
            {
                uint unknown = GetUInt( ref offset );
            }

            string valueName = GetString( ref offset );

            if( valueType == (uint)ValueType.ARRAY )
            {
                string entry1 = GetString( ref offset );
                string entry2 = GetString( ref offset );
                string entry3 = GetString( ref offset );
            }
            else
            {
                GetByte( ref offset );
                GetByte( ref offset );
                GetByte( ref offset );
            }

            Debug.Log( "Variable: " + valueName );
        }
    }*/
}
