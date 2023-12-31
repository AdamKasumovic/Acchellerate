﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GeNa.Core
{
    /// <summary>
    /// Decorator for converting GameObject to Terrain Tree
    /// </summary>
    public class GeNaMaterialDecorator : MonoBehaviour, IDecorator
    {
        #region Variables
        public bool UnpackPrefab => false;
        [SerializeField] protected bool m_enabled = true;
        [SerializeField] protected GetMaterialIDType m_getMaterialType = GetMaterialIDType.Name;
        [SerializeField] protected bool m_showRenderData = true;
        [SerializeField] protected int m_materialInstanceID = 0;
        [SerializeField] protected int m_materialDataPaletteID = int.MinValue;
        [SerializeField] protected GeNaMaterialDecoratorDatabase m_materialData;
        [SerializeField] protected int m_selectedPreset;
        [SerializeField] protected List<MaterialDecoratorRendererData> m_rendererData = new List<MaterialDecoratorRendererData>();
        [SerializeField] protected List<string> m_materialNames = new List<string>();
        #endregion
        #region Parameters
        public bool Enabled
        {
            get => m_enabled;
            set => m_enabled = value;
        }
        public GetMaterialIDType GetMaterialType
        {
            get => m_getMaterialType;
            set => m_getMaterialType = value;
        }
        public bool ShowRenderData
        {
            get => m_showRenderData;
            set => m_showRenderData = value;
        }
        public int MaterialInstanceID
        {
            get => m_materialInstanceID;
            set => m_materialInstanceID = value;
        }
        public GeNaMaterialDecoratorDatabase MaterialData
        {
            get => m_materialData;
            set => m_materialData = value;
        }
        public int SelectedPreset
        {
            get => m_selectedPreset;
            set => m_selectedPreset = value;
        }
        public List<MaterialDecoratorRendererData> RendererData
        {
            get => m_rendererData;
            set => m_rendererData = value;
        }
        public List<string> MaterialNames
        {
            get => m_materialNames;
            set => m_materialNames = value;
        }
        #endregion
        public void OnIngest(Resource resource)
        {
            var palette = resource.Palette;
            if (palette != null)
            {
                m_materialDataPaletteID = palette.AddObject(m_materialData);
            }
        }
        public IEnumerator OnSelfSpawned(Resource resource)
        {
            yield break;
        }
        public void OnChildrenSpawned(Resource resource)
        {
            if (MaterialData != null && Enabled)
            {
                RendererData.Clear();

                //Setup LOD groups first
                LODGroup[] lodGroups = gameObject.GetComponentsInChildren<LODGroup>();
                if (lodGroups.Length > 0)
                {
                    foreach (LODGroup lodGroup in lodGroups)
                    {
                        LOD[] lods = lodGroup.GetLODs();
                        List<Renderer> lodRenderers = new List<Renderer>();
                        foreach (LOD lod in lods)
                        {
                            lodRenderers.AddRange(lod.renderers);
                            foreach (Renderer render in lod.renderers)
                            {
                                MeshBeenProcessed scriptCheck = render.GetComponent<MeshBeenProcessed>();
                                if (scriptCheck == null)
                                {
                                    render.gameObject.AddComponent<MeshBeenProcessed>();
                                }
                            }
                        }
                        List<int> ids = MaterialData.GetMaterialIDFromType(lodRenderers, MaterialNames, MaterialInstanceID, GetMaterialType, out lodRenderers);
                        RendererData.Add(new MaterialDecoratorRendererData
                        {
                            m_meshRenderers = lodRenderers,
                            m_materialIDs = ids
                        });
                    }
                }
                MeshRenderer[] renderers = gameObject.GetComponents<MeshRenderer>();
                if (renderers.Length < 1)
                {
                    renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
                }
                if (renderers.Length > 0)
                {
                    foreach (MeshRenderer renderer in renderers)
                    {
                        if (renderer.GetComponent<MeshBeenProcessed>() == null)
                        {
                            List<Renderer> renders = new List<Renderer>
                            {
                                renderer
                            };
                            List<int> ids = MaterialData.GetMaterialIDFromType(renders, MaterialNames, MaterialInstanceID, GetMaterialType, out renders);
                            RendererData.Add(new MaterialDecoratorRendererData
                            {
                                m_meshRenderers = renders,
                                m_materialIDs = ids
                            });
                        }
                    }
                }
                if (SelectedPreset == 0)
                {
                    foreach (MaterialDecoratorRendererData rendererData in RendererData)
                    {
                        MaterialData.Apply(rendererData, MaterialData.GetByChanceOf());
                    }
                }
                else
                {
                    foreach (MaterialDecoratorRendererData rendererData in RendererData)
                    {
                        MaterialData.Apply(rendererData, MaterialData.GetPreset(SelectedPreset));
                    }
                }
            }
            GeNaEvents.Destroy(this);
        }
        public void LoadReferences(Palette palette)
        {
            m_materialData = palette.GetObject<GeNaMaterialDecoratorDatabase>(m_materialDataPaletteID);
        }
    }
}