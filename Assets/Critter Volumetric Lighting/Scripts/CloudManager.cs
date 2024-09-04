using UnityEngine;

namespace CritterVolumetricLighting
{
	public class CloudManager : MonoBehaviour
	{
		[Header("Materials with clouds")]
		[SerializeField] public Material[] Materials = new Material[0];
		[SerializeField] public bool overrideMatValues = true;
		
		[Header("General")]
		[SerializeField] float cloudCoverage = 0.6f;
		[SerializeField] float cloudScale = 0.025f;
		[SerializeField] float cloudChange = 0.005f; // 0.015f
		[SerializeField] Vector2 cloudMovement = new Vector2(-0.25f, 0.25f);
		[SerializeField] Vector2 cloudStep = new Vector2(13, 17);

		[Header("Visuals")]
		[SerializeField] int shades = 5;
		[SerializeField] float cloudStrength = 2f;
		[SerializeField] float brightness = -1f;
		[SerializeField] float miminumDarkness = 0.2f;

		private VolumetricLightManager _VLManager;
		private bool _hasStarted = false;

		/**
		Push cloud values to VLManager and also to materials if 'overrideMatValues' is true.
		*/
		public void ApplyChanges()
		{
			if (overrideMatValues && Materials.Length > 0)
				SetMaterialValues();
			
			SetVLManagerValues();
		}
		
		
		private void SetMaterialValues()
		{
			foreach (Material mat in Materials)
			{
				mat.SetFloat("_Cloud_Density", cloudScale);
				mat.SetVector("_Cloud_Movement", cloudMovement);
				mat.SetFloat("_Cloud_Strength", cloudStrength);
				mat.SetFloat("_Cloud_Cover", cloudCoverage);
				mat.SetFloat("_Cloud_Change", cloudChange);
				mat.SetVector("_Cloud_Step", cloudStep);
				mat.SetFloat("_Shades", shades);
				mat.SetFloat("_Brightness", brightness);
				mat.SetFloat("_MinimumDarkness", miminumDarkness);
			}
		}
		
		
		private void SetVLManagerValues()
		{				
			_VLManager.dataWriterAdditive = ((1.12f + (brightness / shades)) - cloudStrength)+0.005f;
			_VLManager.dataWriterMultiplier = cloudStrength;
			_VLManager.brightness = 0.25f * (-0.49f*shades+1.95f);
		
			_VLManager.lightRamps = (int)shades;	
			_VLManager.cloudCoverage = cloudCoverage;
			_VLManager.cloudScale = cloudScale;
			_VLManager.cloudChange = cloudChange;
			_VLManager.cloudMovement = cloudMovement;
			_VLManager.cloudStep = cloudStep;	
		}


		void OnEnable()
		{
			if (_hasStarted)
			{
				_VLManager.SetCloudManager(this, true);
			}
		}
		
		
		void OnDisable()
		{
			_VLManager.SetCloudManager(this, false);
		}


		void Start()
		{
			_VLManager = FindObjectOfType<VolumetricLightManager>();
			ApplyChanges();
			_hasStarted = true;
			_VLManager.SetCloudManager(this, true);
		}
	}
}