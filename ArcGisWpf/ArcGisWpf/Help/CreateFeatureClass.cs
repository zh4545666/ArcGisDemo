using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;

namespace ArcGisWpf.Help
{
    class CreateFeatureClass
    {
        /// <summary>
        /// 创建数据集
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="strDataName"></param>
        /// <param name="spatialReference"></param>
        /// <returns></returns>
        public IFeatureDataset CreateDataset(IWorkspace pWorkspace, string strDataName, ISpatialReference spatialReference)
        {
            try
            {
                if (pWorkspace == null)
                    return null;
                IFeatureWorkspace aFeaWorkspace = pWorkspace as IFeatureWorkspace;
                if (aFeaWorkspace == null)
                    return null;

                string dsName = strDataName;
                ISpatialReference aSR = spatialReference;
                IFeatureDataset aDS = aFeaWorkspace.CreateFeatureDataset(dsName, aSR);
                return aDS;

            }
            catch (Exception ex) { }
            return null;
        }

        /// <summary>
        /// 创建栅格数据集
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="sName"></param>
        /// <returns></returns>
        public IRasterDataset CreateRasterDataset(IWorkspace pWorkspace, string sName)
        {
            IRasterDataset rasterDataset = null;
            try
            {
                IRasterWorkspaceEx rasterWorkspace = pWorkspace as IRasterWorkspaceEx;
                 rasterDataset = rasterWorkspace.CreateRasterDataset(sName, 3, rstPixelType.PT_CHAR, null, null, null, null);
            }
            catch (Exception ex)
            {
            }
            return rasterDataset;
        }

        /// <summary>
        /// 创建矢量数据
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <returns></returns>       
        public IFeatureClass CreateFeatureClassClass(IWorkspace pWorkspace , string strName, string strAliasName , IFields fields )
        {
            if (pWorkspace == null)
                return null;
            IFeatureWorkspace featureWorkspace = pWorkspace as IFeatureWorkspace;
            if (featureWorkspace == null)
                return null;

            IFeatureClass featureClass = null;
            try
            {
                featureClass = featureWorkspace.CreateFeatureClass(strName, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", null);
                if (!string.IsNullOrEmpty(strAliasName))
                {
                    IClassSchemaEdit classSchemaEdit = featureClass as IClassSchemaEdit;
                    if (null != classSchemaEdit)
                        classSchemaEdit.AlterAliasName(strAliasName);
                }
             }
            catch(Exception ex)
            {

            }
            return featureClass;
        }

        /// <summary>
        /// 创建矢量数据
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="strName"></param>
        /// <param name="strAliasName"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public IFeatureClass CreateFeatureClassClass(IFeatureDataset pFeatureDataset, string strName, string strAliasName, IFields fields)
        {
            if (pFeatureDataset == null)
                return null;
            return CreateFeatureClassClass((pFeatureDataset as IDataset).Workspace, strName, strAliasName, fields);
        }

        /// <summary>
        /// 创建属性表
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <returns></returns>
        public ITable CreateTable(IWorkspace pWorkspace,string strName,string strAliasName, IFields fields)
        {
            if (pWorkspace == null) return null;
            IFeatureWorkspace featureWorkspace = pWorkspace as IFeatureWorkspace;
            if (featureWorkspace == null) return null;
            ITable table = null;

            try
            {
                table = featureWorkspace.CreateTable(strName, fields, null, null, null);
                if (!string.IsNullOrEmpty(strAliasName))
                {
                    IClassSchemaEdit classSchemaEdit = table as IClassSchemaEdit;
                    if (null != classSchemaEdit)
                    {
                        classSchemaEdit.RegisterAsObjectClass("OBJECTID", null);
                        classSchemaEdit.AlterAliasName(strAliasName);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return table; 
        }

        /// <summary>
        /// 转换为字段
        /// </summary>
        /// <returns></returns>
        public IFields ConvertToFields()
        {
            IFields fields = new FieldsClass();

            return fields;
        }
    }
}
