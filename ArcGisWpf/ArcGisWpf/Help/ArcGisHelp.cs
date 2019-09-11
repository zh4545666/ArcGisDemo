using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcGisWpf
{
    class ArcGisHelp
    {

        /// <summary>
        /// 获取矢量对象
        /// </summary>
        /// <param name="WorkspaceFactoryProgID">
        /// 
        /// esriDataSourcesGDB.AccessWorkspaceFactory
        /// esriDataSourcesFile.ArcInfoWorkspaceFactory
        /// esriDataSourcesFile.CadWorkspaceFactory
        /// esriDataSourcesGDB.FileGDBWorkspaceFactory
        /// esriDataSourcesOleDB.OLEDBWorkspaceFactory
        /// esriDataSourcesFile.PCCoverageWorkspaceFactory
        /// esriDataSourcesRaster.RasterWorkspaceFactory
        /// esriDataSourcesGDB.SdeWorkspaceFactory
        /// esriDataSourcesFile.ShapefileWorkspaceFactory
        /// esriDataSourcesOleDB.TextFileWorkspaceFactory
        /// esriDataSourcesFile.TinWorkspaceFactory
        /// esriDataSourcesFile.VpfWorkspaceFactory
        /// 
        /// </param>
        /// <param name="GDBPath"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        private IFeatureClass getFeatureClass(string WorkspaceFactoryProgID, string path, string featureClassName)
        {
            IWorkspaceName workspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = WorkspaceFactoryProgID,
                PathName = path
            };

            IName workspaceIName = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)workspaceIName.Open();

            IFeatureClass featureClass = (workspace as IFeatureWorkspace).OpenFeatureClass(featureClassName);

            return featureClass;
        }

        /// <summary>
        /// 复制要素集（ConvertFeatureDataset）
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <param name="targetWorkspace"></param>
        /// <param name="nameOfSourceFeatureDataset"></param>
        /// <param name="nameOfTargetFeatureDataset"></param>        
        public void IFeatureDataConverter_ConvertFeatureDataset(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureDataset, string nameOfTargetFeatureDataset)
        {
            //create source workspace name
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

            //create source dataset name
            IFeatureDatasetName sourceFeatureDatasetName = new FeatureDatasetNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureDatasetName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureDataset;

            //create target workspace name
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;

            //create target dataset name
            IFeatureDatasetName targetFeatureDatasetName = new FeatureDatasetNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureDatasetName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureDataset;

            //Convert feature dataset
            IFeatureDataConverter featureDataConverter = new FeatureDataConverterClass();
            featureDataConverter.ConvertFeatureDataset(sourceFeatureDatasetName, targetFeatureDatasetName, null, "", 1000, 0);
            //Console.WriteLine("Conversion Complete");
        }

        /// <summary>
        /// 复制导入要素（ConvertFeatureDataset）
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <param name="targetWorkspace"></param>
        /// <param name="nameOfSourceFeatureClass"></param>
        /// <param name="nameOfTargetFeatureClass"></param>
        /// <param name="pQueryFilter"></param>
        public static void ConvertFeatureClass(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureClass, string nameOfTargetFeatureClass, IQueryFilter pQueryFilter)
        {
            //create source workspace name
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;


            //create source dataset name
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureClass;


            //create target workspace name
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;


            //create target dataset name
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureClass;


            //Open input Featureclass to get field definitions.
            ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();


            //Validate the field names because you are converting between different workspace types.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields targetFeatureClassFields;
            IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
            IEnumFieldError enumFieldError;


            // Most importantly set the input and validate workspaces!
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;
            fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);


            // Loop through the output fields to find the geomerty field
            IField geometryField;
            for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
            {
                if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    geometryField = targetFeatureClassFields.get_Field(i);
                    // Get the geometry field's geometry defenition
                    IGeometryDef geometryDef = geometryField.GeometryDef;


                    //Give the geometry definition a spatial index grid count and grid size
                    IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;


                    targetFCGeoDefEdit.GridCount_2 = 1;
                    targetFCGeoDefEdit.set_GridSize(0, 0); //Allow ArcGIS to determine a valid grid size for the data loaded
                    targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;

                    // Load the feature class
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, pQueryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);

                    break;
                }
            }
        }


        /// <summary>
        /// 复制导入要素（ConvertFeatureDataset）
        /// </summary>
        /// <param name="pDataSet"></param>
        /// <param name="strFeatFileDir"></param>
        /// <param name="strFeatFileName"></param>
        /// <param name="strOutName"></param>
        /// <param name="isWorkspace"></param>
        public void FeatureClassToFeatureClass(IDataset pDataSet, string strFeatFileDir, string strFeatFileName, string strOutName, bool isWorkspace)
        {
            try
            {
                IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWK = (IFeatureWorkspace)pWSF.OpenFromFile(strFeatFileDir, 0);
                IFeatureClass pInFeatureClass = pFeatureWK.OpenFeatureClass(strFeatFileName);
                if (pInFeatureClass == null || pDataSet == null)
                {
                    MessageBox.Show("创建失败");
                    return;
                }
                IFeatureClassName pInFeatureclassName;
                IDataset pIndataset = (IDataset)pInFeatureClass;
                pInFeatureclassName = (IFeatureClassName)pIndataset.FullName;
                //如果名称已存在  
                IWorkspace2 pWS2 = null;
                if (isWorkspace)
                    pWS2 = pDataSet as IWorkspace2;
                else
                    pWS2 = pDataSet.Workspace as IWorkspace2;
                if (pWS2.get_NameExists(esriDatasetType.esriDTFeatureClass, strOutName))
                {
                    MessageBoxResult result = MessageBox.Show(null, "矢量文件名  " + strOutName + "  在数据库中已存在!" + "/r是否覆盖?", "相同文件名", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
                    //覆盖原矢量要素  
                    if (result == MessageBoxResult.Yes)
                    {
                        IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS2;
                        IDataset pDataset = pFWS.OpenFeatureClass(strOutName) as IDataset;
                        pDataset.Delete();
                        pDataset = null;
                    }
                }
                IFields pInFields, pOutFields;
                IFieldChecker pFieldChecker = new FieldCheckerClass();
                IEnumFieldError pError;
                pInFields = pInFeatureClass.Fields;
                pFieldChecker.Validate(pInFields, out pError, out pOutFields);
                IField geoField = null;
                for (int i = 0; i < pOutFields.FieldCount; i++)
                {
                    IField pField = pOutFields.get_Field(i);
                    if (pField.Type == esriFieldType.esriFieldTypeOID)
                    {
                        IFieldEdit pFieldEdit = (IFieldEdit)pField;
                        pFieldEdit.Name_2 = pField.AliasName;
                    }
                    if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        geoField = pField;
                        break;
                    }
                }
                IGeometryDef geometryDef = geoField.GeometryDef;
                IFeatureDataConverter one2another = new FeatureDataConverterClass();
                IFeatureClassName pOutFeatureClassName = new FeatureClassNameClass();
                IDatasetName pOutDatasetName = (IDatasetName)pOutFeatureClassName;
                if (isWorkspace)
                    pOutDatasetName.WorkspaceName = (IWorkspaceName)pDataSet.FullName;
                else
                    pOutDatasetName.WorkspaceName = (IWorkspaceName)((IDataset)pDataSet.Workspace).FullName;
                pOutDatasetName.Name = strOutName;
                if (isWorkspace)
                {
                    one2another.ConvertFeatureClass(pInFeatureclassName, null, null, pOutFeatureClassName, geometryDef, pOutFields, "", 1000, 0);
                }
                else
                {
                    IFeatureDataset pFeatDS = (IFeatureDataset)pDataSet;
                    IFeatureDatasetName pOutFeatDSName = pFeatDS.FullName as IFeatureDatasetName;
                    one2another.ConvertFeatureClass(pInFeatureclassName, null, pOutFeatDSName, pOutFeatureClassName, geometryDef, pOutFields, "", 1000, 0);
                    pOutFeatDSName = null;
                    pFeatDS = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

    }


}
