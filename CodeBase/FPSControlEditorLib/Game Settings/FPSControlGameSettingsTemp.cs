using UnityEngine;
using System.Collections;
using UnityEditor;
using FPSControl;
using FPSControlEditor;

namespace FPSControlEditor
{
    public class FPSControlGameSettingsTemp : ScriptableObject
    {
        //Key Binding
        public KeyCode runLeft = KeyCode.A;
        public KeyCode runRight = KeyCode.D;
        public KeyCode interact = KeyCode.E;
        public KeyCode reload = KeyCode.Q;
        public KeyCode escape = KeyCode.Escape;

        //Mouse Settings
        public float sensitivityX = 1F;
        public float sensitivityY = 1F;
        public float mouseFilterBufferSize = .1F;
        public float gunLookDownOffsetThreshold = 0F;
        public float minimumX = 0F;
        public float minimumY = 0F;
        public float maximumX = 0F;
        public float maximumY = 0F;

        static string PATH = FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.TEMP + "_TempGameSettings.asset";

        public FPSControlGameSettingsTemp(GameSettingsModule module)
        {
            runLeft = module.runLeft;
            runRight = module.runRight;
            interact = module.interact;
            reload = module.reload;
            escape = module.escape;

            sensitivityX = module.sensitivityX;
            sensitivityY = module.sensitivityY;
            mouseFilterBufferSize = module.mouseFilterBufferSize;
            gunLookDownOffsetThreshold = module.gunLookDownOffsetThreshold;
            minimumX = module.minimumX;
            minimumY = module.minimumY;
            maximumX = module.maximumX;
            maximumY = module.maximumY;

            AssetDatabase.CreateAsset(this, PATH);
        }

        public void Project(ref RBFPSControllerLogic target)
        {
            target._runKeyLeft = runLeft;
            target._runKeyRight = runRight;
            target._interactionKey = interact;
            target._reloadKey = reload;
            target._escapeKey = escape;

            target._sensitivityX = sensitivityX;
            target._sensitivityY = sensitivityY;
            target._mouseFilterBufferSize = (int)mouseFilterBufferSize;
            target._gunLookDownOffsetThreshold = gunLookDownOffsetThreshold;
            target._minimumX = minimumX;
            target._minimumY = minimumY;
            target._maximumX = maximumX;
            target._maximumY = maximumY;
        }

        internal static void Load(ref RBFPSControllerLogic target)
        {
            FPSControlGameSettingsTemp tmp = (FPSControlGameSettingsTemp) AssetDatabase.LoadAssetAtPath(PATH, typeof(FPSControlGameSettingsTemp));
            tmp.Project(ref target);
            AssetDatabase.DeleteAsset(PATH);
        }
    }
}
