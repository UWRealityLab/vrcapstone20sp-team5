using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class MeshingScript : MonoBehaviour {
   #region Public Variables
   public Material BlackMaterial;
   public Material GroundMaterial;
   public Material InactiveMaterial;
   public MLSpatialMapper Mapper;
   #endregion

   #region Private Variables
   private bool _visible = true;
   #endregion

   #region Unity Methods
   private void Update() {
       UpdateMeshMaterial();
   }
   #endregion

   #region Public Methods
   public void ToggleMeshVisibility() {
       _visible = _visible ? false : true;
   }
   public void ToggleMeshScanning() {
       Mapper.enabled = Mapper.enabled ? false : true;
   }
   #endregion

   #region Private Methods
   /// Switch mesh material based on whether meshing is active and mesh is visible
   /// visible & active = ground material
   /// visible & inactive = meshing off material
   /// invisible = black mesh
   private void UpdateMeshMaterial() {
     // Loop over all the child mesh nodes created by MLSpatialMapper script
     for (int i = 0; i < transform.childCount; i++) {        
        // Get the child gameObject
        GameObject gameObject = transform.GetChild(i).gameObject;
        // Get the meshRenderer component
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        // Get the assigned material
        Material material = meshRenderer.sharedMaterial;
        if (_visible) {
           if (Mapper.enabled) {
               if (material != GroundMaterial) {
                   meshRenderer.material = GroundMaterial;
               }
           }
           else if (material != InactiveMaterial) {
               meshRenderer.material = InactiveMaterial;
           }
        }
        else if (material != BlackMaterial) {
           meshRenderer.material = BlackMaterial;
        }
     }
   }
   #endregion
}