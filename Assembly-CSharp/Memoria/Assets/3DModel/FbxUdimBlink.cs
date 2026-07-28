using System;
using Memoria.Data;
using UnityEngine;

namespace Memoria.Assets
{
    public sealed class FbxUdimBlink : MonoBehaviour
    {
        private const Single MinimumBlinkDelay = 1.5f;
        private const Single MaximumBlinkDelay = 4f;
        private const Single ClosedEyeDuration = 0.12f;

        [SerializeField]
        private BlinkTarget[] _targets;
        private Single _timeUntilNextBlink;
        private Single _closedEyeTimeRemaining;
        private Boolean _eyesClosed;
        private EyeMode _eyeMode;

        public void Initialize(SkinnedMeshRenderer[] renderers, Mesh[] meshes, Vector2[][] openUVs, Vector2[][] closedUVs)
        {
            Int32 targetCount = 0;
            for (Int32 i = 0; i < meshes.Length; i++)
                if (meshes[i] != null)
                    targetCount++;

            _targets = new BlinkTarget[targetCount];
            Int32 targetIndex = 0;
            for (Int32 i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] == null)
                    continue;

                _targets[targetIndex++] = new BlinkTarget(renderers[i], meshes[i], openUVs[i], closedUVs[i]);
            }
            SetEyeMode(EyeMode.Automatic, true);
        }

        public static Boolean Transfer(GameObject sourceRoot, GameObject destinationRoot, SkinnedMeshRenderer[] sourceRenderers, SkinnedMeshRenderer[] destinationRenderers)
        {
            if (sourceRoot == null || destinationRoot == null || sourceRenderers == null || destinationRenderers == null || sourceRenderers.Length != destinationRenderers.Length)
                return false;

            FbxUdimBlink sourceBlink = sourceRoot.GetComponent<FbxUdimBlink>();
            FbxUdimBlink destinationBlink = destinationRoot.GetComponent<FbxUdimBlink>();
            if (sourceBlink == null || sourceBlink._targets == null)
            {
                Remove(destinationBlink);
                return false;
            }

            BlinkTarget[] transferredTargets = new BlinkTarget[sourceRenderers.Length];
            Int32 transferredTargetCount = 0;
            for (Int32 i = 0; i < sourceRenderers.Length; i++)
            {
                BlinkTarget sourceTarget = sourceBlink.FindTarget(sourceRenderers[i]);
                SkinnedMeshRenderer destinationRenderer = destinationRenderers[i];
                if (sourceTarget == null || destinationRenderer == null || destinationRenderer.sharedMesh == null)
                    continue;

                transferredTargets[transferredTargetCount++] = new BlinkTarget(destinationRenderer, destinationRenderer.sharedMesh, sourceTarget.OpenUVs, sourceTarget.ClosedUVs);
            }
            if (transferredTargetCount == 0)
            {
                Remove(destinationBlink);
                return false;
            }
            if (transferredTargetCount != transferredTargets.Length)
                Array.Resize(ref transferredTargets, transferredTargetCount);

            if (destinationBlink == null)
                destinationBlink = destinationRoot.AddComponent<FbxUdimBlink>();
            destinationBlink.InitializeTransferredTargets(transferredTargets, sourceBlink._eyeMode);
            if (sourceBlink != destinationBlink)
            {
                // Stop the temporary model from changing the copied meshes.
                sourceBlink._targets = null;
                sourceBlink.enabled = false;
            }
            return true;
        }

        internal static void SynchronizeBattleState(BTL_DATA battle)
        {
            if (battle == null)
                return;

            SetBattleEyesClosed(battle, btl_stat.CheckStatus(battle, BattleStatus.Death));
        }

        internal static void SetBattleEyesClosed(BTL_DATA battle, Boolean eyesClosed)
        {
            if (battle == null)
                return;

            EyeMode eyeMode = eyesClosed ? EyeMode.ForcedClosed : EyeMode.Automatic;
            SetEyeMode(battle.originalGo, eyeMode);
            SetEyeMode(battle.tranceGo, eyeMode);
            SetEyeMode(battle.gameObject, eyeMode);
        }

        internal static void SynchronizeTextureAnimation(GameObject root, GEOTEXHEADER textureAnimation)
        {
            if (root == null || textureAnimation?.geotex == null || textureAnimation.geotex.Length <= 2)
                return;

            if (IsTextureAnimationActive(textureAnimation, 2))
                SetEyeMode(root, EyeMode.Automatic);
            else if (IsTextureAnimationActive(textureAnimation, 0))
                SetEyeMode(root, EyeMode.ForcedClosed);
            else
                SetEyeMode(root, EyeMode.ForcedOpen);
        }

        private void Update()
        {
            if (_targets == null)
                return;
            if (_eyeMode != EyeMode.Automatic)
                return;
            UIManager uiManager = PersistenSingleton<UIManager>.Instance;
            if (uiManager != null && uiManager.IsPause)
                return;

            Single elapsedTime = Time.unscaledDeltaTime;
            if (_eyesClosed)
            {
                _closedEyeTimeRemaining -= elapsedTime;
                if (_closedEyeTimeRemaining > 0f)
                    return;

                // Keep the open UVs until the next blink.
                ApplyUVs(false);
                _eyesClosed = false;
                ScheduleNextBlink();
            }
            else
            {
                _timeUntilNextBlink -= elapsedTime;
                if (_timeUntilNextBlink > 0f)
                    return;

                // Show the closed eyes for a moment.
                ApplyUVs(true);
                _eyesClosed = true;
                _closedEyeTimeRemaining = ClosedEyeDuration;
            }
        }

        private void OnEnable()
        {
            if (_targets == null)
                return;

            ApplyEyeMode();
        }

        private void OnDisable()
        {
            if (_targets == null)
                return;

            ApplyUVs(false);
            _eyesClosed = false;
        }

        private void OnDestroy()
        {
            _targets = null;
        }

        private void InitializeTransferredTargets(BlinkTarget[] targets, EyeMode sourceEyeMode)
        {
            _targets = targets;
            _eyeMode = sourceEyeMode;
            GeoTexAnim textureAnimation = GetComponent<GeoTexAnim>();
            if (textureAnimation?.TextureAnim != null)
                SynchronizeTextureAnimation(gameObject, textureAnimation.TextureAnim);
            SetEyeMode(_eyeMode, true);
            enabled = true;
        }

        private void ApplyEyeMode()
        {
            Boolean useClosedUVs = _eyeMode == EyeMode.ForcedClosed;
            ApplyUVs(useClosedUVs);
            _eyesClosed = useClosedUVs;
            if (_eyeMode == EyeMode.Automatic)
                ScheduleNextBlink();
        }

        private void SetEyeMode(EyeMode eyeMode, Boolean forceApply)
        {
            if (!forceApply && _eyeMode == eyeMode)
                return;

            _eyeMode = eyeMode;
            ApplyEyeMode();
        }

        private void ScheduleNextBlink()
        {
            _timeUntilNextBlink = UnityEngine.Random.Range(MinimumBlinkDelay, MaximumBlinkDelay);
        }

        private void ApplyUVs(Boolean useClosedUVs)
        {
            for (Int32 i = 0; i < _targets.Length; i++)
            {
                BlinkTarget target = _targets[i];
                Mesh mesh = target.Renderer != null && target.Renderer.sharedMesh != null ? target.Renderer.sharedMesh : target.ImportedMesh;
                Vector2[] uvs = useClosedUVs ? target.ClosedUVs : target.OpenUVs;
                if (mesh != null && uvs != null && mesh.vertexCount == uvs.Length)
                    mesh.uv = uvs;
            }
        }

        private BlinkTarget FindTarget(SkinnedMeshRenderer renderer)
        {
            if (renderer == null)
                return null;

            for (Int32 i = 0; i < _targets.Length; i++)
                if (_targets[i] != null && _targets[i].Renderer == renderer)
                    return _targets[i];
            return null;
        }

        private static Boolean IsTextureAnimationActive(GEOTEXHEADER textureAnimation, Int32 animationIndex)
        {
            return animationIndex >= 0
                && animationIndex < textureAnimation.geotex.Length
                && textureAnimation.geotex[animationIndex] != null
                && (textureAnimation.geotex[animationIndex].flags & 1) != 0;
        }

        private static void SetEyeMode(GameObject root, EyeMode eyeMode)
        {
            FbxUdimBlink blink = root != null ? root.GetComponent<FbxUdimBlink>() : null;
            if (blink != null && blink._targets != null)
                blink.SetEyeMode(eyeMode, false);
        }

        private static void Remove(FbxUdimBlink blink)
        {
            if (blink == null)
                return;

            blink._targets = null;
            blink.enabled = false;
        }

        private enum EyeMode
        {
            Automatic,
            ForcedOpen,
            ForcedClosed
        }

        [Serializable]
        private sealed class BlinkTarget
        {
            public SkinnedMeshRenderer Renderer;
            public Mesh ImportedMesh;
            public Vector2[] OpenUVs;
            public Vector2[] ClosedUVs;

            public BlinkTarget(SkinnedMeshRenderer renderer, Mesh importedMesh, Vector2[] openUVs, Vector2[] closedUVs)
            {
                Renderer = renderer;
                ImportedMesh = importedMesh;
                OpenUVs = openUVs;
                ClosedUVs = closedUVs;
            }
        }
    }
}
