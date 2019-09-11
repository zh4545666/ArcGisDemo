using ESRI.ArcGIS.Geodatabase;
using System;
using System.Windows.Forms;

namespace ArcGisWpf.Help
{
    class ImportFeatureClass
    {
        public static bool ConvertFeatureClassToFeatureDataset(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureClass, string nameOfTargetFeatureClass, IFeatureDatasetName pName)
        {
            try
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
                fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError,
                    out targetFeatureClassFields);
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
                        targetFCGeoDefEdit.set_GridSize(0, 0);
                        //Allow ArcGIS to determine a valid grid size for the data loaded       
                        targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                        // we want to convert all of the features      
                        IQueryFilter queryFilter = new QueryFilterClass();
                        queryFilter.WhereClause = "";
                        // Load the feature class              
                        IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                        IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName,
                            queryFilter, pName, targetFeatureClassName,
                            geometryDef, targetFeatureClassFields, "", 1000, 0);
                        break;
                    }
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
