using ActionFrame.Runtime;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ActionFrame.Editor
{
	[CustomEditor(typeof(ESkeletonAnimation))]
	[CanEditMultipleObjects]
	public class ESkeletonAnimationInspector : SkeletonRendererInspector
	{
		protected SerializedProperty animationName, loop, timeScale, autoReset, eSpineCtrJsonFile;
		protected bool wasAnimationParameterChanged = false;
		protected bool requireRepaint;

		readonly GUIContent LoopLabel = new GUIContent("Loop",
			"Whether or not .AnimationName should loop. This only applies to the initial animation specified in the inspector, or any subsequent Animations played through .AnimationName. Animations set through state.SetAnimation are unaffected.");

		readonly GUIContent TimeScaleLabel = new GUIContent("Time Scale",
			"The rate at which animations progress over time. 1 means normal speed. 0.5 means 50% speed.");

		readonly GUIContent ESkeletonJsonDataLabel = new GUIContent("SkeletonJsonFile",
			"Record all information of this skeletonAnimation state.");
		protected override void OnEnable()
		{
			base.OnEnable();
			animationName = serializedObject.FindProperty("_animationName");
			loop = serializedObject.FindProperty("loop");
			timeScale = serializedObject.FindProperty("timeScale");
			eSpineCtrJsonFile = serializedObject.FindProperty("eSpineCtrJsonFile");
		}

		protected override void DrawInspectorGUI(bool multi)
		{
			base.DrawInspectorGUI(multi);
			if (!TargetIsValid) return;
			bool sameData = SpineInspectorUtility.TargetsUseSameData(serializedObject);

			foreach (var o in targets)
				TrySetAnimation(o as SkeletonAnimation);

			EditorGUILayout.Space();
			if (!sameData)
			{
				EditorGUILayout.DelayedTextField(animationName);
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(animationName);
				wasAnimationParameterChanged |= EditorGUI.EndChangeCheck(); // Value used in the next update.
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(loop, LoopLabel);
			wasAnimationParameterChanged |= EditorGUI.EndChangeCheck(); // Value used in the next update.
			EditorGUILayout.PropertyField(timeScale, TimeScaleLabel);
			foreach (var o in targets)
			{
				var component = o as SkeletonAnimation;
				component.timeScale = Mathf.Max(component.timeScale, 0);
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(eSpineCtrJsonFile, ESkeletonJsonDataLabel);
			if (EditorGUI.EndChangeCheck())
			{
				((ESkeletonAnimation)this.target).JsonFileChangeEvent?.Invoke(eSpineCtrJsonFile.objectReferenceValue);
			}
			EditorGUILayout.Space();
			SkeletonRootMotionParameter();

			serializedObject.ApplyModifiedProperties();

			if (!isInspectingPrefab)
			{
				if (requireRepaint)
				{
					UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
					requireRepaint = false;
				}
			}
		}

		private void TrySetAnimation(SkeletonAnimation skeletonAnimation)
		{
			if (skeletonAnimation == null) return;
			if (!skeletonAnimation.valid)
				return;

			TrackEntry current = skeletonAnimation.AnimationState.GetCurrent(0);
			if (!isInspectingPrefab)
			{
				string activeAnimation = (current != null) ? current.Animation.Name : "";
				bool activeLoop = (current != null) ? current.Loop : false;
				bool animationParameterChanged = this.wasAnimationParameterChanged &&
				                                 ((activeAnimation != animationName.stringValue) ||
				                                  (activeLoop != loop.boolValue));
				if (animationParameterChanged)
				{
					this.wasAnimationParameterChanged = false;
					var skeleton = skeletonAnimation.Skeleton;
					var state = skeletonAnimation.AnimationState;

					if (!Application.isPlaying)
					{
						if (state != null) state.ClearTrack(0);
						skeleton.SetToSetupPose();
					}

					Spine.Animation animationToUse = skeleton.Data.FindAnimation(animationName.stringValue);

					if (!Application.isPlaying)
					{
						if (animationToUse != null)
						{
							skeletonAnimation.AnimationState.SetAnimation(0, animationToUse, loop.boolValue);
						}

						skeletonAnimation.Update(0);
						skeletonAnimation.LateUpdate();
						requireRepaint = true;
					}
					else
					{
						if (animationToUse != null)
							state.SetAnimation(0, animationToUse, loop.boolValue);
						else
							state.ClearTrack(0);
					}
				}

				// Reflect animationName serialized property in the inspector even if SetAnimation API was used.
				if (Application.isPlaying)
				{
					if (current != null && current.Animation != null)
					{
						if (skeletonAnimation.AnimationName != animationName.stringValue)
							animationName.stringValue = current.Animation.Name;
					}
				}
			}
		}
	}
}
