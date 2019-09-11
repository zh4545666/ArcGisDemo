using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Data;
using System.Windows.Controls;

namespace ArcGisWpf.Help
{
    public class OpenFeatureClass
    {
        private static void InitDataTable(IFeatureClass featureClass, DataTable dataTable)
        {
            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

            for(int i = 0;i < featureClass.Fields.FieldCount;i++)
            {
                dataTable.Columns.Add(featureClass.Fields.Field[i].Name);
            }
        }
        private static void InitDataTable(ITable table, DataTable dataTable)
        {
            dataTable.Rows.Clear();
            dataTable.Columns.Clear();

            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                dataTable.Columns.Add(table.Fields.Field[i].Name);
            }
        }
        

        private static string GetFeatureValue(Feature feature, string name)
        {
            string strValue = null;
            int index = feature.Fields.FindField(name);
            if (index < 0)
                index = feature.Fields.FindFieldByAliasName(name);
            if (index >= 0)
                strValue = feature.get_Value(index).ToString();


            return strValue;
        }

        public static void OpenFeatureClassFunction(AxMapControl MapControl, IFeatureClassName pFcName, DataTable dt = null)
        {
            try
            {
                MapControl.Map.ClearLayers();
                MapControl.SpatialReference = null;
                IName pName = pFcName as IName;
                IFeatureClass pFc = pName.Open() as IFeatureClass;

                if(null != dt)
                {
                    InitDataTable(pFc, dt);
                    
                    IFeatureCursor pCursor = pFc.Search(null, true);
                    IFeature pfea = pCursor.NextFeature();
                    int j = 0;
                    while (pfea != null)
                    {
                        DataRow dataRow = dt.NewRow();

                        for (int i = 0; i < pfea.Fields.FieldCount; i++)
                        {
                            dataRow[i] = pfea.get_Value(i).ToString();  
                        }

                        dt.Rows.Add(dataRow);
                        pfea = pCursor.NextFeature();
                        j++;
                    }
                    ComReleaser.ReleaseCOMObject(pCursor);
                }
                
                if (pFcName.FeatureType == esriFeatureType.esriFTRasterCatalogItem)
                {
                    IGdbRasterCatalogLayer pGdbRCLayer = new GdbRasterCatalogLayerClass();
                    pGdbRCLayer.Setup(pFc as ITable);
                    MapControl.Map.AddLayer(pGdbRCLayer as ILayer);
                }
                else if ((pFcName.FeatureType == esriFeatureType.esriFTSimple) ||
                     (pFcName.FeatureType == esriFeatureType.esriFTComplexEdge) ||
                    (pFcName.FeatureType == esriFeatureType.esriFTComplexJunction) ||
                    (pFcName.FeatureType == esriFeatureType.esriFTSimpleEdge) ||
                     (pFcName.FeatureType == esriFeatureType.esriFTSimpleJunction))
                {

                    IFeatureLayer pLayer = new FeatureLayerClass
                    {
                        FeatureClass = pFc,
                        Name = (pFc as IDataset).Name
                    };
                    MapControl.Map.AddLayer(pLayer as ILayer);
                }
                else if (pFcName.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    ILayer pLayer = OpenAnnotationLayer(pFc);
                    pLayer.Name = (pFc as IDataset).Name;
                    MapControl.Map.AddLayer(pLayer as ILayer);
                }

                MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
            catch (Exception ex)
            { }
        }

        public static void OpenRasterDataset(AxMapControl MapControl, IRasterDatasetName pRdName, DataTable dt = null)
        {
            MapControl.ClearLayers();
            MapControl.SpatialReference = null;
            dt.Rows.Clear();
            dt.Columns.Clear();
            IDatasetName pDsName = pRdName as IDatasetName;
            string sName = pDsName.Name;
            IName pName = pRdName as IName;
            IRasterDataset pRds = pName.Open() as IRasterDataset;
            IRasterLayer pRL = new RasterLayerClass();
            pRL.CreateFromDataset(pRds);
            pRL.Name = sName;
            MapControl.AddLayer(pRL as ILayer);
            MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }

        public static void OpenMosaicDataset(AxMapControl MapControl,IMosaicDatasetName pMdName, DataTable dt = null)
        {
            MapControl.ClearLayers();
            MapControl.SpatialReference = null;
            IDatasetName pDsName = pMdName as IDatasetName;
            string sName = pDsName.Name;
            IName pName = pMdName as IName;
            IMosaicDataset pMds = pName.Open() as IMosaicDataset;

            if(null != dt)
            {
                IFeatureClass pFc = pMds.Catalog;
                InitDataTable(pFc, dt);

                IFeatureCursor pCursor = pFc.Search(null, false);
                IFeature pfea = pCursor.NextFeature();
                int j = 0;
                while (pfea != null)
                {
                    DataRow dataRow = dt.NewRow();

                    for (int i = 0; i < pfea.Fields.FieldCount; i++)
                    {
                        dataRow[i] = pfea.get_Value(i).ToString();
                     }

                    dt.Rows.Add(dataRow);
                    pfea = pCursor.NextFeature();
                    j++;
                }

                ComReleaser.ReleaseCOMObject(pCursor);
            }
            IMosaicLayer pML = new MosaicLayerClass();
            pML.CreateFromMosaicDataset(pMds);

            MapControl.AddLayer(pML as ILayer);
            MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);

        }

        public static void OpenTable(AxMapControl MapControl,ITableName pTName, DataTable dt = null)
        {
            try
            {
                MapControl.Map.ClearLayers();
                MapControl.SpatialReference = null;
                MapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null);
                IName pName = pTName as IName;
                ITable pFc = pName.Open() as ITable;

                if(null != dt)
                {
                    InitDataTable(pFc, dt);

                    ICursor pCursor = pFc.Search(null, false);
                    IRow pfea = pCursor.NextRow();
                    int j = 0;
                    while (pfea != null)
                    {
                        DataRow dataRow = dt.NewRow();

                        for (int i = 0; i < pfea.Fields.FieldCount; i++)
                        {
                            dataRow[i] = pfea.get_Value(i).ToString();
                        }

                        dt.Rows.Add(dataRow);
                        pfea = pCursor.NextRow();
                        j++;
                    }
                    ComReleaser.ReleaseCOMObject(pCursor);
                }                
            }
            catch { }
        }
  
        public static ILayer OpenAnnotationLayer(IFeatureClass pfc)
        {
            IFDOGraphicsLayerFactory pfdof = new FDOGraphicsLayerFactoryClass();
            IFeatureDataset pFDS = pfc.FeatureDataset;
            IWorkspace pWS = pFDS.Workspace;
            IFeatureWorkspace pFWS = pWS as IFeatureWorkspace;
            ILayer pLayer = pfdof.OpenGraphicsLayer(pFWS, pFDS, (pfc as IDataset).Name);
            return pLayer;
        }
    }
}
