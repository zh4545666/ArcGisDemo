using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Windows.Forms;

namespace ArcGisWpf.Help
{
    class ExportFeatureClass
    {
        /// <summary>
        /// 导出Shp文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apFeatureClass"></param>
        /// <returns></returns>
        public static bool ExportFeatureClassToShp(string path, IFeatureClass apFeatureClass)
        {
            try
            {
                string exportFileShortName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (exportFileShortName == "")
                {
                    exportFileShortName = (apFeatureClass as IDataset).Name;
                    if (exportFileShortName.LastIndexOf('.') >= 0)
                    {
                        exportFileShortName = exportFileShortName.Substring(exportFileShortName.LastIndexOf('.') + 1);
                    }
                }
                string exportFilePath = System.IO.Path.GetDirectoryName(path);
                if (exportFilePath == null)
                {
                    exportFilePath = path;
                }
                //设置导出要素类的参数  
                IFeatureClassName pOutFeatureClassName = new FeatureClassNameClass();
                IDataset pOutDataset = (IDataset)apFeatureClass;
                pOutFeatureClassName = (IFeatureClassName)pOutDataset.FullName;
                //创建一个输出shp文件的工作空间  
                IWorkspaceFactory pShpWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspaceName pInWorkspaceName = new WorkspaceNameClass();
                pInWorkspaceName = pShpWorkspaceFactory.Create(exportFilePath, exportFileShortName, null, 0);

                //创建一个要素集合  
                IFeatureDatasetName pInFeatureDatasetName = null;
                //创建一个要素类  
                IFeatureClassName pInFeatureClassName = new FeatureClassNameClass();
                IDatasetName pInDatasetClassName;
                pInDatasetClassName = (IDatasetName)pInFeatureClassName;
                pInDatasetClassName.Name = exportFileShortName;
                pInDatasetClassName.WorkspaceName = pInWorkspaceName;
                //通过FIELDCHECKER检查字段的合法性，为输出SHP获得字段集合
                IFields pInFields = apFeatureClass.Fields;
                pInFields = apFeatureClass.Fields;
                IFieldChecker pFieldChecker = new FieldChecker();
                pFieldChecker.Validate(pInFields, out IEnumFieldError pEnumFieldError, out IFields pOutFields);
                //通过循环查找几何字段  
                IField pGeoField = null;
                for (long iCounter = 0; iCounter < pOutFields.FieldCount; iCounter++)
                {
                    if (pOutFields.get_Field((int)iCounter).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pGeoField = pOutFields.get_Field((int)iCounter);
                        break;
                    }
                }
                //得到几何字段的几何定义  
                IGeometryDef pOutGeometryDef;
                IGeometryDefEdit pOutGeometryDefEdit;
                pOutGeometryDef = pGeoField.GeometryDef;
                //设置几何字段的空间参考和网格  
                pOutGeometryDefEdit = (IGeometryDefEdit)pOutGeometryDef;
                pOutGeometryDefEdit.GridCount_2 = 1;
                pOutGeometryDefEdit.set_GridSize(0, 1500000);

                //开始导入  
                IFeatureDataConverter pShpToClsConverter = new FeatureDataConverterClass();
                pShpToClsConverter.ConvertFeatureClass(pOutFeatureClassName, null, pInFeatureDatasetName, pInFeatureClassName, pOutGeometryDef, pOutFields, "", 1000, 0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClass"></param>
        public static void ExportFeatureClassToShpDlg(IFeatureClass featureClass )
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Shapefile文件(.shp)|*.shp";
            if(DialogResult.OK == saveFileDialog.ShowDialog())
            {
                string strPath = saveFileDialog.FileName;
                ExportFeatureClassToShp(strPath, featureClass);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClass"></param>
        public static void ExportFeatureClassToShpDlg(IFeatureClassName featureClassName)
        {
            IName name = featureClassName as IName;
            IFeatureClass featureClass = name.Open() as IFeatureClass;
            ExportFeatureClassToShpDlg(featureClass);
        }


        /// <summary>
        /// 导出Gdb文件
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <param name="targetWorkspace"></param>
        /// <param name="nameOfSourceFeatureDataset"></param>
        /// <param name="nameOfTargetFeatureDataset"></param>
        /// <returns></returns>        
        public static bool ConvertFeatureDataset(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureDataset, string nameOfTargetFeatureDataset)
        {
            try
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
